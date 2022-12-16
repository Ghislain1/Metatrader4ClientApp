// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.TCP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using ProxyLib.Proxy;
    using System.Net.Http;
    using Metatrader4ClientApp.Infrastructure.Models;
    using TradingAPI.MT4Server;
    using static System.Net.WebRequestMethods;
    using System.Security.Policy;
    using System.Net.Http.Json;
    using System.Diagnostics;
    using System.Net.Security;
    using System.Diagnostics.Tracing;
    using System.Net.Http.Headers;

    internal sealed class HttpEventListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // List of event source names provided by networking in .NET 5.
            if (eventSource.Name == "System.Net.Http" ||
                eventSource.Name == "System.Net.Sockets" ||
                eventSource.Name == "System.Net.Security" ||
                eventSource.Name == "System.Net.NameResolution")
            {
                EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, new Dictionary<string, string>()
                {
                    // These additional arguments will turn on counters monitoring with a reporting interval set to a half of a second. 
                    ["EventCounterIntervalSec"] = TimeSpan.FromSeconds(0.5).TotalSeconds.ToString()
                });
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // It's a counter, parse the data properly.
            if (eventData.EventId == -1)
            {
                var sb = new StringBuilder().Append($"{eventData.TimeStamp:HH:mm:ss.fffffff}  {eventData.EventSource.Name}  ");
                var counterPayload = (IDictionary<string, object>)(eventData.Payload[0]);
                bool appendSeparator = false;
                foreach (var counterData in counterPayload)
                {
                    if (appendSeparator)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(counterData.Key).Append(": ").Append(counterData.Value);
                    appendSeparator = true;
                }
                Debug.WriteLine(sb.ToString());
            }
        }
    }
    public static class ProxiedTcpClient
    {
        /// <summary>
        /// Checks system proxy settings for given destination uri
        /// </summary>
        /// <param name="destination">Destination against which proxy settings are going to be checked</param>
        /// <returns>Proxy status</returns>
        public static ProxyStatus GetProxyStatusFor(Uri destination)
        {
            var proxy = WebRequest.DefaultWebProxy?.GetProxy(destination) ?? throw new ArgumentException(nameof(destination));

            return new ProxyStatus
            {
                IsEnabled = proxy.Host != destination.Host,
                Uri = proxy
            };
        }



        /// <summary>
        /// Create TcpClient that connects to the destination through proxy
        /// </summary>
        /// <param name="proxy">Proxy address</param>
        /// <param name="destination">Destination address</param>
        /// <returns>Tcp client connected through proxy</returns>
        public static TcpClient CreateProxied(Uri proxy, Uri destination)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(proxy.Host, proxy.Port);

            var connectMessage = Encoding.UTF8.GetBytes($"CONNECT {destination.Host}:{destination.Port} HTTP/1.1{Environment.NewLine}{Environment.NewLine}");
            socket.Send(connectMessage);

            byte[] receiveBuffer = new byte[1024];
            var received = socket.Receive(receiveBuffer);

            var response = ASCIIEncoding.ASCII.GetString(receiveBuffer, 0, received);

            if (!response.Contains("200 OK"))
            {
                throw new Exception($"Error connecting to proxy server {destination.Host}:{destination.Port}. Response: {response}");
            }

            return new TcpClient
            {
                Client = socket
            };
        }
        public static TcpClient CreateConnection(int user, string password, string host, int port)
        {
            // var uri = new Uri(host );
            var uriWithCred = new UriBuilder() { Host = host, UserName = user.ToString(), Password = password, Port = port }.Uri;

            IProxyClient proxyClient = MakeProxy(uriWithCred);
            // Setup timeouts
            proxyClient.ReceiveTimeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
            proxyClient.SendTimeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;




            return proxyClient.TcpClient;


        }
        public static TcpClient Connect1(int user, string password, string host, int port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(host, port);

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
            var connectMessage = Encoding.UTF8.GetBytes($"CONNECT {host}:{port} HTTP/1.1\nProxy-Authorization: Basic {auth}\n\n");
            socket.Send(connectMessage);

            byte[] receiveBuffer = new byte[1024];
            var received = socket.Receive(receiveBuffer);

            var response = ASCIIEncoding.ASCII.GetString(receiveBuffer, 0, received);

            if (!response.Contains("200 OK"))
            {
                throw new Exception($"Error connecting to proxy server {host}:{port}. Response: {response}");
            }

            return new TcpClient
            {
                Client = socket
            };
        }
        public static async Task<TcpClient> CreateConnectionAsync1(int user, string password, string host, int port)
        {
            return await Task.Run(() => CreateConnection(user, password, host, port));
        }
        /// <summary>
        /// With MT4API 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<QuoteClient> CreateConnectionAsync_Old(int user, string password, string host, int port)
        {
            var TARGETURL = $"http://{host}";
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            //using Socket client = new(hostEntry.AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            QuoteClient quoteClient = new QuoteClient(user, password, host, port);
            await Task.Run(() => quoteClient.Connect());

            if (!quoteClient.Connected)
            {
                throw new Exception($"Error connecting to proxy server  Response: {host}{quoteClient.AccountName}");
            }

            return quoteClient;


        }

        /// <summary>
        /// Using SocketsHttpHandler by seeting Credentials
        /// System.Net.Http.HttpConnection.<SendAsyncCore>d__64.MoveNext()\r\n   at System.Net.Http.AuthenticationHelper.<SendWithNtAuthAsync>d__53.MoveNext()\r\n   at System.Net.Http.HttpConnectionPool.<SendWithVersionDetectionAndRetryAsync>d__83.MoveNext()\r\n...
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<TcpClient> CreateConnectionAsyncAAAA(int user, string password, string host, int port)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Instantiate the listener which subscribes to the events. 
          //  using var listener = new HttpEventListener();

            var TARGETURL = $"http://{host}";
            var targetUri = new Uri(TARGETURL);
            IPHostEntry hostEntry = Dns.GetHostEntry(host, AddressFamily.InterNetwork);
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = host;


            HttpResponseMessage? httpResponseMessage;
            try
            {
                // Create a SocketsHttpHandler for the URL.
                var socketsHttpHandler = new SocketsHttpHandler();
                CredentialCache myCache = new CredentialCache();


                var myCred = new NetworkCredential(user.ToString(), password);
                myCache.Add(new Uri(TARGETURL), "Negotiate", myCred);
                socketsHttpHandler.PreAuthenticate = true;
                socketsHttpHandler.Credentials = myCache;
                socketsHttpHandler.PooledConnectionLifetime = TimeSpan.FromSeconds(60*10);
                socketsHttpHandler.PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1);
                socketsHttpHandler.MaxConnectionsPerServer = 100;
          

                socketsHttpHandler.PlaintextStreamFilter = (context, token) =>
                {
                    Debug.WriteLine($"[PlaintextStreamFilter]: Request {context.InitialRequestMessage} --> negotiated version {context.NegotiatedHttpVersion}");
                    return ValueTask.FromResult(context.PlaintextStream);
                };
                int i = 0;
                // socketsHttpHandler.UseCookies = Options.TrackPerSessionCookies,      
                socketsHttpHandler.ConnectCallback = async (context, cancellationToken) =>
                {
                    Debug.WriteLine($"[ConnectCallback]: Request {context.InitialRequestMessage} --> negotiated version {i++}");
                    //var ipAddress = Dns.GetHostEntry(host).AddressList[0];
                    Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    socket.NoDelay = true;


                    try
                    {
                        var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, AddressFamily.InterNetwork, cancellationToken);
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 5);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 5);
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.HeaderIncluded, true);
                        await socket.ConnectAsync(entry.AddressList, port, cancellationToken);
                        var networkStream = new NetworkStream(socket, true);
                        return networkStream;
                    }
                    catch (Exception ex)
                    {
                        socket.Dispose();

                        throw;
                    }
                };
                // Create a request for the URL.
                HttpClient httpClient = new HttpClient(socketsHttpHandler);
                byte[] headerBytes = Encoding.ASCII.GetBytes(user + ":" + password);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Negotiate", Convert.ToBase64String(headerBytes));
                var httpRequestMessage =             
                      new HttpRequestMessage
                    {
                        RequestUri = new Uri($"http://{host}"),
                        Method = HttpMethod.Post,
                       
                    };
                
                await httpClient.SendAsync(httpRequestMessage);
                //httpClient.DefaultRequestHeaders.Accept.Clear();
                //httpClient.DefaultRequestHeaders.Accept.Add(
                //    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var target1 = "http://www.mt4-demo.roboforex.com/"; // Der angegebene Host ist unbekannt. (www.mt4-demo.roboforex.com:80)
                var target = "http://mt4-demo.roboforex.com/:443"; // Der angegebene Host ist unbekannt. (www.mt4-demo.roboforex.com:80)
                httpResponseMessage = await httpClient.GetAsync($"http://{host}");
                httpResponseMessage.EnsureSuccessStatusCode();

                // Receive ack.       
                var response = httpResponseMessage.Content;
            }
            catch (HttpRequestException httpRequestException)
            {

                Debug.WriteLine(httpRequestException.InnerException?.Message);
                throw;
            }

            catch (Exception exec)
            {

                throw;
            }





            return new TcpClient()
            {

                // Client = client
            };
        }

        /// <summary>
        /// Using SocketsHttpHandler by seeting Httpclien header
        /// System.Net.Http.HttpConnection.<SendAsyncCore>d__64.MoveNext()\r\n   at System.Net.Http.HttpConnectionPool.<SendWithVersionDetectionAndRetryAsync>d__83.MoveNext()\r\n   at System.Threading.Tasks.ValueTask`1.get_Result()\r\n   at System.Runtime.Compil...
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<TcpClient> CreateConnectionAsync(int user, string password, string host, int port)
        {
            var TARGETURL = $"http://{host}";
            var targetUri = new Uri(TARGETURL);
            IPHostEntry hostEntry = Dns.GetHostEntry(host, AddressFamily.InterNetwork);
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = host;


            HttpResponseMessage? httpResponseMessage;
            try
            {
                // Create a Handler for the URL. 
                var socketsHttpHandler = new SocketsHttpHandler();
                socketsHttpHandler.PreAuthenticate = true;
                socketsHttpHandler.ConnectCallback = async (context, cancellationToken) =>
                {
                    //var ipAddress = Dns.GetHostEntry(host).AddressList[0];
                    Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    socket.NoDelay = true;


                    try
                    {
                        var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, AddressFamily.InterNetwork, cancellationToken);
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 5);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 5);
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.HeaderIncluded, true);
                        await socket.ConnectAsync(entry.AddressList, port, cancellationToken);
                        return new NetworkStream(socket, true);
                    }
                    catch (Exception ex)
                    {
                        socket.Dispose();

                        throw;
                    }
                };

                // Create a request for the URL.
                HttpClient httpClient = new HttpClient(socketsHttpHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string myUser = user.ToString();
                string myPassword = password;
                string userAndPasswordToken =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(myUser + ":" + myPassword));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                    $"Basic {userAndPasswordToken}");
                httpResponseMessage = await httpClient.GetAsync(targetUri+ "/OpenedOrders");
                httpResponseMessage.EnsureSuccessStatusCode();

                // Receive ack.       
                var response = httpResponseMessage.Content;
            }
            catch (Exception exec)
            {

                throw;
            }



           

            return new TcpClient()
            {

                // Client = client
            };
        }
        /// <summary>
        /// HttpClientHandler
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<TcpClient> CreateConnectionAsyncAA(int user, string password, string host, int port)
        {

            IPHostEntry hostEntry = Dns.GetHostEntry(host);

            // Create a httpClientHandler for the URL.
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                 Proxy = new WebProxy("http://127.0.0.1:9998"),
                 UseProxy = false,
            };
            CredentialCache myCache = new CredentialCache();
            var myCred = new NetworkCredential(user.ToString(), password);
            myCache.Add(new Uri($"http://{host}"), "Negotiate", myCred);
           // httpClientHandler.PreAuthenticate = true;
          //  httpClientHandler.Credentials = myCache;

            httpClientHandler.ServerCertificateCustomValidationCallback = (__, xx, context, cancellationToken) => true;
            // Create a HttpClient for the URL.
            HttpClient httpClient = new HttpClient(httpClientHandler);
            byte[] headerBytes = Encoding.ASCII.GetBytes(user + ":" + password);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(headerBytes));
           
           
          
          
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"http://{host}");
            httpResponseMessage.EnsureSuccessStatusCode();

            return new TcpClient()
            {

                // Client = client
            };
        }
        public static IProxyClient MakeProxy(Uri proxyUri)
        {
            // Uses https://github.com/grinay/ProxyLib
            ProxyClientFactory factory = new ProxyClientFactory();
            if (proxyUri.UserInfo == string.Empty)
            {
                return factory.CreateProxyClient(ProxyType.Http, proxyUri.Host, proxyUri.Port);
            }
            else
            {
                if (proxyUri.UserInfo.Contains(':'))
                {
                    var userPass = proxyUri.UserInfo.Split(':');
                    return factory.CreateProxyClient(ProxyType.Socks4, proxyUri.Host, proxyUri.Port, userPass[0], userPass[1]);
                }
                else
                {
                    throw new Exception($"Invalid user info: {proxyUri.UserInfo}");
                }
            }
        }
    }
}
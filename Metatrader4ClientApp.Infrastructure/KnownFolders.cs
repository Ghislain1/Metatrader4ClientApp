

namespace Metatrader4ClientApp.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class KnownFolders
    {

        public const string ParentFolderName = "Metatrader4ClientApp";
        public const string ProductVersion = "1.0.0";
        public static string ApplicationNameForPython { get; set; } = null;

        // C:\ProgramData\
        public static string CommonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
        public static string ConnectionParameterPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "Metatrader4ClientApp");
      



        public static Uri ExportedFolderUri =>

            KnownFolders.GetFolderUri(

                nameof(KnownFolders.ExportedFolderUri),

                new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),

                "Exported");



        public static Uri GroupPermissionsConfigurations =>

            KnownFolders.GetFolderUri(

                nameof(KnownFolders.GroupPermissionsConfigurations),

                KnownFolders.ParentSettingsFolderUri,

                "GroupPermissionsConfigurations");



 



        public static Uri LogFolderUri => KnownFolders.GetFolderUri(nameof(KnownFolders.LogFolderUri), KnownFolders.ParentFolderUri, "Log");



        public static Uri ParentFolderUri => KnownFolders.BuildDefaultParentFolder();



        public static Uri ParentSettingsFolderUri =>

            KnownFolders.GetFolderUri(nameof(KnownFolders.ParentSettingsFolderUri), KnownFolders.ParentFolderUri, "Settings");



    



        public static Uri ReportsFolderUri => KnownFolders.GetFolderUri(nameof(KnownFolders.ReportsFolderUri), KnownFolders.ParentFolderUri, "Reports");


 


 



        private static Uri BuildDefaultParentFolder(string additionalFolder = null)

        {

            string subfoldersPath;

            if (additionalFolder == null || string.IsNullOrWhiteSpace(additionalFolder))

            {

                subfoldersPath = Path.Combine(

                    KnownFolders.ParentFolderName,

                    !string.IsNullOrEmpty(KnownFolders.ApplicationNameForPython)

                        ? KnownFolders.ApplicationNameForPython

                        : KnownFolders.NamespaceToApplicationName(Process.GetCurrentProcess().ProcessName),

                    KnownFolders.ProductVersion);

            }

            else

            {

                subfoldersPath = Path.Combine(

                    KnownFolders.ParentFolderName,

                    !string.IsNullOrEmpty(KnownFolders.ApplicationNameForPython)

                        ? KnownFolders.ApplicationNameForPython

                        : KnownFolders.NamespaceToApplicationName(Process.GetCurrentProcess().ProcessName),

                    KnownFolders.ProductVersion,

                    additionalFolder);

            }



            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))

            {

                return new Uri(Path.Combine("/etc", subfoldersPath));

            }



            return new Uri(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), subfoldersPath));

        }



        public static Uri GetFolderUri(string propertyName, Uri parentFolderURI, string defaultFolderName)

        {

          //  var appSettingsUri = ConfigurationManager.AppSettings.Get(propertyName);
          // TODO@GHIslain
            var folderUri = !string.IsNullOrEmpty(string.Empty)

                ? new Uri(string.Empty)

                : new Uri(Path.Combine(parentFolderURI.LocalPath, defaultFolderName));



            if (!Directory.Exists(folderUri.LocalPath))
            {
                Directory.CreateDirectory(folderUri.LocalPath);

            }



            return folderUri;

        }



        public static string NamespaceToApplicationName(string namespaceName)

        {

            return namespaceName.Split('.').Last();

        }



  

    }

}
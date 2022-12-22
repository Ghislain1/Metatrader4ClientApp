So how will the application connect to the mt4 server? One way is to do it by TCP (need to Reverse Engineer)

Please have a look at this video for an example of the concept:
https://www.youtube.com/watch?v=DjzprEpSR3g&t=303s&ab_channel=MikePapinskiLab

(Note: Using the DDL file from http://mtapi.online/ is not allowed since it's a trial for only 14 days!)


Specification for the application:
- Connect into a MT4 account without running the MT4 terminal
- Can log in to many different MT4 accounts simultaneously and read the current open position and export them into a text file.
- The application will be able to read and export over 100++ different accounts simultaneously. (minimal delay, since this will be used at a copier system (IMPORTANT!))
- The interface will show if account X is connected or offline. (If Offline then will show how many min/hours it was last online).  (Online will display in green color)
- An On/Off option if I want to connect the individual MT4 accounts from the list
- An On/Off option if I want to disable the export trade position function.


- Each MT4 account will export its own text file for their open trade positions

This is the Data format that will be exported into a text file.
GBPJPY,150257979,0.01,171.26,B,171.115,171.395,436333139
(Currency Symbol, Order number, Lot size, Entry price, Order type, Take profit price, Stop loss price,
Magic number)

- Display how many terminals from the list that is online.
Example: 50/60 Terminals are online

So the application will display this in the interface:
Terminal name - Account number - Balance - Daily Trades - Weekly Trades - Status (Online/Offline)

It will have an output directory where all the individual terminal names will be exported with their open trade positions.



Minor updates will surely happen along the way, these must be accepted.


If you have any features or ideas that can make it better, please suggest them.
If any questions, please ask.

Thank you.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using NLog;
using System.Diagnostics;

namespace testApplication
{
    class Program
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        static HubBlockingCollection<UserConnection> userConnectionList = new HubBlockingCollection<UserConnection>();
        public static HubBlockingCollection<UserConnection> userConnectionMonitoringList = new HubBlockingCollection<UserConnection>();
        public static TestSettings settings; // acts like singleton


        static void Main(string[] args)
        {


            Program.settings = new TestSettings(
                connectionsQuantityIn : 300,
                timerRepeatMsIn:        200,
                randomPercentageIn :    10,
                quickTestModeIn :       true,
                checkClicksTestIn :     false,
                checkClicksTest_instancesToMonitorIn : 10,
                checkClicksTest_valueChangeIntervalMsIn : 500,
                checkClicksTest_valueChangeTimeVariabilityPctIn: 50,
                checkClicksTest_rangeUpperIn: 100,
                checkClicksTest_rangeLowerIn: 0
                );


            Program.logger.Debug("Start Program");
            Debug.WriteLine("Start Program");

            if (!Program.settings.quickTestMode) { 
                Console.WriteLine("Press Enter to Connect.");
                Console.ReadLine();
            }

            int quantity = Program.settings.connectionsQuantity;
            for (int i = 1; i <= quantity; i++ )
            {
                var connection = new HubConnection("http://localhost:53398"); //Set connection
                var myHub = connection.CreateHubProxy("chatHub");   //Make proxy to hub based on hub name on server
                connection.Start().Wait(); //Start connection
                string nameInput = "Dave " + i.ToString();
                UserConnection user = new UserConnection(i);

                //TODO: Consolidate these 2 into the UserConnections Class as methods
                myHub.Invoke<string>("ConnectTestUser", nameInput).Wait();
                myHub.On < string,string >("UploadListInfo", (userId, groupId) =>
                {
                    //Console.WriteLine("User ID {0} , Group ID {1}  ", userId, groupId );
                    userConnectionList.FirstOrDefault(o => o.Connection.ConnectionId == userId  ).GroupId = groupId;
                    Program.logger.Debug("List info updated for {0}", userId);
                });

                myHub.On<GameGroup>("UpdateGame", (GameGroup gg) =>
                {
                    bool belongsToThisGroup = false;
                    foreach (UserConnection userCon in Program.userConnectionMonitoringList) 
                    {
                        belongsToThisGroup = user.GroupId == userCon.GroupId ? true : belongsToThisGroup; 
                    }

                    if (belongsToThisGroup) { // user is in the group being iterated through
                        foreach (PlayerState ps in gg.PlayerStates.Values)
                        {
                            if (user.Connection.ConnectionId == ps.Id && 
                                user.ClickChangeState == UserConnection.ClickChangeStatus.Uploaded )
                            {
                                //Found player in group,  now check  
                                //a) time taken for round trip & b) whether the same value submitted has been returned
                                //Program.logger.Debug("Round trip time (ms) : {0} , user id : {1}", user.getRoundTripTimeMs().ToString(), user.Connection.ConnectionId );
                                user.roundtripTimesMsList.Add(user.getRoundTripTimeMs());
                                user.ClickChangeState = UserConnection.ClickChangeStatus.Received; 

                            }
                        }
                    }

                    
                });

                Console.WriteLine("--> {0}", i);
                
                user.Connection = connection;
                user.MyHub = myHub;
                userConnectionList.Add(user);
            }

            if (!Program.settings.quickTestMode)
            {
                Console.WriteLine("Press Enter to Allocate Groups.");
                Console.ReadLine();
            }
            else {
                Thread.Sleep(1000);
            }
            
            //need to simulate uploiad every x secs
            userConnectionList.FirstOrDefault().MyHub.Invoke("AssignTestUsersToGroup").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}", task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Users assigned to groups!");
                }
            }).Wait();

            if (!Program.settings.quickTestMode)
            {
                Console.WriteLine("Show Allocated groups");
                Console.ReadLine();
            }
            else {
                Thread.Sleep(1000);
            }
            List<UserConnection> sortedUserconnections = (from UserConnection userCon in userConnectionList orderby userCon.GroupId select userCon).ToList<UserConnection>();
            string currentGroup = "";
            foreach (UserConnection connectedUser in userConnectionList) { 
                if(connectedUser.GroupId != currentGroup ){
                    currentGroup  = connectedUser.GroupId ;
                    Console.WriteLine("------------ Group {0} ----------", currentGroup);
                }
                Console.WriteLine("     User : {0}", connectedUser.Connection.ConnectionId.ToString());
            }
            Console.WriteLine("Run inter-group communication simulation.");
            Console.ReadLine();
            int j = 0;
            int connectionsSimulatingLimit = userConnectionList.Count(); //currently testing all connections
            foreach (UserConnection userCon in userConnectionList) {
                if (j > connectionsSimulatingLimit)
                {    
                    break;
                }
                userCon.InitTimer();
                j++;
            }

            Console.WriteLine("\n\n\nTesting In Progress...\n\n\n");


            Console.WriteLine("\n\nPress Enter to End, Disconnect and show stats\n\n");
            Console.ReadLine();





            double countCumalative = 0,
                        highestCumalative = 0,
                        lowestCumalative = 0,
                        sdCumalative = 0,
                        averageMeanCumalative = 0;

            foreach (UserConnection userCon in userConnectionMonitoringList)
            {
                //go through each userConnection monitored and give sumary
                //average
                //high 
                //low
                //sd


                double count = findMathsInfo(userCon.roundtripTimesMsList, "count"),
                        highest = findMathsInfo(userCon.roundtripTimesMsList, "highest"),
                        lowest = findMathsInfo(userCon.roundtripTimesMsList, "lowest"),
                        sd = findMathsInfo(userCon.roundtripTimesMsList, "sd"),
                        averageMean = findMathsInfo(userCon.roundtripTimesMsList, "averageMean");

                Console.WriteLine("\n Data for user id : {0} ", userCon.Connection.ConnectionId);
                Console.WriteLine("     count   : {0}", count );
                Console.WriteLine("     highest : {0}",   highest );
                Console.WriteLine("     lowest  : {0}", lowest);
                Console.WriteLine("     sd      : {0}", sd);
                Console.WriteLine("     average : {0}", averageMean);
                Console.WriteLine(" ----------------------------------------------------------- ");

                countCumalative += count;
                highestCumalative += highest;
                lowestCumalative += lowest;
                sdCumalative += sd;
                averageMeanCumalative += averageMean;

            }


            Console.WriteLine("\n\n *****************    TEST INFO    ***************** ");
            Console.WriteLine("instances            {0}", Program.settings.connectionsQuantity);
            Console.WriteLine("monitored instances  {0}", Program.settings.checkClicksTest_instancesToMonitor);
            Console.WriteLine("timer repeat (MS)    {0}", Program.settings.timerRepeatMs);

            Console.WriteLine("User (spoof) updates click amount every Interval (MS)    {0}", Program.settings.checkClicksTest_valueChangeIntervalMs);
            Console.WriteLine("User (spoof) updates click variability (%)               {0}", Program.settings.checkClicksTest_valueChangeTimeVariabilityPct);
            Console.WriteLine(" ----------------------------------------------------------- ");


            Console.WriteLine("\n\n ***************** OVERALL AVERAGE ***************** ");
            Console.WriteLine("     count   : {0}", (countCumalative / userConnectionMonitoringList.Count));
            Console.WriteLine("     highest : {0}", (highestCumalative / userConnectionMonitoringList.Count));
            Console.WriteLine("     lowest  : {0}", (lowestCumalative / userConnectionMonitoringList.Count));
            Console.WriteLine("     sd      : {0}", (sdCumalative / userConnectionMonitoringList.Count));
            Console.WriteLine("     average : {0}", (averageMeanCumalative / userConnectionMonitoringList.Count));
            Console.WriteLine(" ----------------------------------------------------------- ");


            //show aggregated stats
            //put this in a seperate text file that will be generated (with date as name) this will then be saved to a specified folder

            //Garbage collecting - close all connections
            foreach (UserConnection userI in userConnectionList) // disable send/receive data from signalR
            {
                userI.BlockData = true;
            }
            foreach (UserConnection userI in userConnectionList) //close connections
            {
                userI.EndConnection();
                Console.WriteLine("userI._connection.State : " + userI.Connection.State.ToString());
            }



            //if (!Program.settings.quickTestMode) {
                Console.ReadLine();
            //}
        }


        private static double findMathsInfo(HubBlockingCollection<int> list, string infoToFind) {

            if (infoToFind == "count") { return list.Count; }
            if (list.Count <= 0) { return -1; } //stops list with 0 members being submitted

            double averageMean = 0, 
                highest = 0, 
                lowest = list.FirstOrDefault(),
                standardDeviation = 0;

            int cumulativeTotal = 0;
            foreach (int item in list) {
                cumulativeTotal += item;
                if(item > highest){ highest = item;}
                if(item < lowest){ lowest = item; }
            }
            averageMean = cumulativeTotal / list.Count;

            if(infoToFind == "averageMean") { return averageMean; }
            else if(infoToFind == "highest"){ return highest; }
            else if(infoToFind == "lowest"){ return lowest; }
            else if(infoToFind == "sd"){
                double cumulativeVariance = 0;
                foreach (int item in list)
                {
                    double variance = Math.Pow((item - averageMean), 2);
                    cumulativeVariance += variance;
                }
                standardDeviation = Math.Pow((cumulativeVariance / list.Count ), 0.5);
                return standardDeviation;
            }
            else{ return -1; }
        }


        //Methods object dump method
        private static string ObjectToXml(object output)
        {
            string objectAsXmlString;
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(output.GetType());
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                try
                {
                    xs.Serialize(sw, output);
                    objectAsXmlString = sw.ToString();
                }
                catch (Exception ex)
                {
                    objectAsXmlString = ex.ToString();
                }
            }
            return objectAsXmlString;
        }


    }

}

/*
 * 
 ************* EXAMPLE OF SIGNAL R COM<S METHODS ************* 
 
    connection.Start().ContinueWith(task =>
    {
        if (task.IsFaulted) { Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException()); }
        else { Console.WriteLine("Connected"); }
    }).Wait();
 
    myHub.On<string, string>("onConnected", (id, name) =>
    {
        Console.WriteLine("--> {0}", i);
        //Console.WriteLine("{0} - {1}", id, name);
        //Console.WriteLine("this is the 'onConnected' client method ");
    });

    myHub.Invoke<string>("Connect", nameInput.ToString()).ContinueWith(task =>
    {
        if (task.IsFaulted) { Console.WriteLine("There was an error calling send: {0}", task.Exception.GetBaseException()); }
        else { Console.WriteLine(task.Result); }
    }).Wait();
 
 * 
 */



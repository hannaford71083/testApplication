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

        static List<UserConnection> userConnectionList = new List<UserConnection>();

        static void Main(string[] args)
        {

            Program.logger.Debug("Start Program");
            Debug.WriteLine("Start Program");

            Console.WriteLine("Press Enter to Connect.");
            Console.ReadLine();
            int quantity = 12;
            for (int i = 0; i <= quantity; i++ )
            {
                var connection = new HubConnection("http://localhost:53398"); //Set connection
                var myHub = connection.CreateHubProxy("chatHub");   //Make proxy to hub based on hub name on server
                connection.Start().Wait(); //Start connection

                string nameInput = "Dave " + i.ToString();

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
                  //Console.WriteLine("Here is some data:  {0}", astring);
                    //Program.logger.Debug(string.Format("GameGroup id is :  {0}", gg.id ));
                    foreach (PlayerState ps in gg.PlayerStates.Values)
                    {
                        //Program.logger.Debug(string.Format("Player id is :  {0}", ps.Id));
                    }

                });



                Console.WriteLine("--> {0}", i);
                UserConnection user = new UserConnection(i);
                user.Connection = connection;
                user.MyHub = myHub;
                userConnectionList.Add(user);
            }


            Console.WriteLine("Press Enter to Allocate Groups.");
            Console.ReadLine();
            //need to simulate uploiad every x secs

            userConnectionList.FirstOrDefault().MyHub.Invoke("AssignTestUsersToGroup").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Users assigned to groups!");
                    //Console.WriteLine(task.Result);
                }
            }).Wait();



            Console.WriteLine("Show Allocated groups");
            Console.ReadLine();


            List<UserConnection> sortedUserconnections = (from UserConnection userCon in userConnectionList orderby userCon.GroupId select userCon).ToList<UserConnection>();

            //userConnectionList = userConnectionList.OrderBy(element  )

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
            //EXAMPLE of invoking server method with multiple arguments
            //userConnectionList.FirstOrDefault().MyHub.Invoke<object[]>("UploadData", "groupId", "playerId", "presses");

            Console.ReadLine();
            Console.WriteLine("Press Enter to Disconnect.");
            Console.ReadLine();
            //Garbage collecting - close all connections
            foreach(UserConnection userI in userConnectionList ){
                userI.EndConnection();
                Console.WriteLine("userI._connection.State : " + userI.Connection.State.ToString() );
            }

            //need to clear all the timer events 
            //Console.WriteLine("--------------- userConnectionList ---------------");
            //Console.WriteLine(ObjectToXml(userConnectionList)); //TODO: Add this back??
            Console.ReadLine();



        }

        public static void UpdatePlayers(string gg) {

          //  (gameGroup) =>
          //{
          Console.WriteLine("GameGroup id TEST ----> {0} ", gg);
          //var a = "adsasdadsad";
          //});

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



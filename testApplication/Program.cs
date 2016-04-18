using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;


namespace testApplication
{
    class Program
    {


        static List<UserConnection> userConnectionList = new List<UserConnection>();




        static void Main(string[] args)
        {




           /*
            What is test application to acheive:
            *   a) Create multiple X hub connections within y groups
            *   b) Send update messages sent roughly every t secs
            *   
            * 
            */


            /* create an object that will hold the following properties:
             *  1) connection, myHub, 
            */





            //var connection = new Connection("http://localhost:53398");
            //connection.Received += data => Console.WriteLine("Received : " + data);
            //connection.Start().ContinueWith(t => Console.WriteLine("Connected")).Wait();

            //var line = string.Empty;
            //while ((line = Console.ReadLine()) != null)
            //{
            //    connection.Send(line).Wait();
            //}

            Console.WriteLine("Press Enter to Connect.");
            Console.ReadLine();

            int quantity = 10;

            for (int i = 0; i <= quantity; i++ )
            {
                //Set connection
                var connection = new HubConnection("http://localhost:53398"); //http://127.0.0.1:8088/
                //Make proxy to hub based on hub name on server
                var myHub = connection.CreateHubProxy("chatHub");
                //Start connection
                connection.Start().Wait();
                //connection.Start().ContinueWith(task =>
                //{
                //    if (task.IsFaulted)
                //    {
                //        Console.WriteLine("There was an error opening the connection:{0}",
                //                          task.Exception.GetBaseException());
                //    }
                //    else
                //    {
                //        Console.WriteLine("Connected");
                //    }
                //}).Wait();
                string nameInput = "Dave " + i.ToString();
                //myHub.Invoke<string>("Connect", nameInput);

                myHub.Invoke<string>("ConnectTestUser", nameInput).Wait();


                myHub.On < string,string >("UploadListInfo", (userId, groupId) =>
                {
                    Console.WriteLine("User ID {0} , Group ID {1}  ", userId, groupId );
                    userConnectionList.FirstOrDefault(o => o.Connection.ConnectionId == userId  ).GroupId = groupId;
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


            Console.WriteLine("Run inter-group communication simulation.");
            Console.ReadLine();
            /*            simplist thing is run loop that will send user data up every x secs             */

            /* ******* IMPLEMENT THIS ON MONDAY ******** */


            //First Try Please Delete if other approach works

            //Action uploadDataAct = uploadData; //<-- uploadData is static function (bottom) should contain loop upload data
            ////uploadDataAct = delegate() { uploadData(); };
            //Task t1 = new Task(uploadDataAct);
            ////each task will sleep a certain time and then will execute the upload data task
            //t1 = Task.Run(() => Thread.Sleep(1000));

            // Create a timer and set a two second interval.
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Interval = 1000;

            // Create a timer with a X milli second interval.
            aTimer = new System.Timers.Timer(3000);
            aTimer.Elapsed += (sender, e) => uploadData(sender, e, userConnectionList.FirstOrDefault() );// Hook up the Elapsed event for the timer.
            aTimer.AutoReset = true;// Have the timer fire repeated events (true is the default)
            aTimer.Enabled = true;// Start the timer
            aTimer.Start();



            //// Create a timer and set a two second interval.
            //System.Timers.Timer bTimer = new System.Timers.Timer();
            //bTimer.Interval = 1;

            //// Create a timer with a X milli second interval.
            //bTimer = new System.Timers.Timer(1000);
            //bTimer.Elapsed += (sender, e) => uploadData(sender, e, "second");// Hook up the Elapsed event for the timer.
            //bTimer.AutoReset = true;// Have the timer fire repeated events (true is the default)
            //bTimer.Enabled = true;// Start the timer
            //bTimer.Start();


            //userConnectionList.FirstOrDefault().MyHub.Invoke<object[]>("UploadData", "groupId", "playerId", "presses");


            //UploadData(string groupId, string playerId, string presses )



            Console.ReadLine();
            Console.WriteLine("Press Enter to Disconnect.");
            Console.ReadLine();
            //Garbage collecting - close all connections
            foreach(UserConnection userI in userConnectionList ){
                userI.Connection.Stop();
                Console.WriteLine("userI._connection.State : " + userI.Connection.State.ToString() );
            }



            //need to clear all the timer events 


            Console.ReadLine();

            //connection.Stop();

            //myHub.On<string, string>("onConnected", (id, name) =>
            //{
            //    Console.WriteLine("--> {0}", i );
            //    //Console.WriteLine("{0} - {1}", id, name);
            //    //Console.WriteLine("this is the 'onConnected' client method ");
            //});

            //chatHub.client.onConnected = function (id, userName, allUsers, messages) {


            //myHub.Invoke<string>("Connect", nameInput.ToString()).ContinueWith(task =>
            //{
            //    if (task.IsFaulted)
            //    {
            //        Console.WriteLine("There was an error calling send: {0}",
            //                          task.Exception.GetBaseException());
            //    }
            //    else
            //    {
            //        Console.WriteLine(task.Result);
            //    }
            //}).Wait();


            //myHub.On<string>("addMessage", param =>
            //{
            //    Console.WriteLine(param);
            //});

            //myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();

        }




        private static void uploadData(object sender, EventArgs e, UserConnection user)
        {

            //Console.WriteLine("Test Action --> "+ arg1  );

            user.MyHub.Invoke<object[]>("UploadData",  user.GroupId , user.Connection.ConnectionId, 10 );

            //userConnectionList.FirstOrDefault().MyHub.Invoke<object[]>("UploadData", "groupId", "playerId", "presses");

        }






    }


    



}

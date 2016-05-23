using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;


namespace testApplication
{
    class UserConnection
    {


        #region public variables
            public HubConnection Connection;
            public IHubProxy MyHub;
            public string GroupId;
        #endregion

        #region private variables
            private System.Timers.Timer _timer;
        #endregion


        /*
         * Constructor         
         */
        public UserConnection(int indexNumber) {
            init(indexNumber);
            
        }

        #region Methods

        private void init(int indexNumber)
        {
            //this._connection = new HubConnection("http://localhost:53398"); //http://127.0.0.1:8088/
            //this.myHub = this._connection.CreateHubProxy("chatHub");
            //this._connection.Start().Wait();
            //string nameInput = "Dave " + indexNumber.ToString();
            //myHub.Invoke<string>("ConnectTestUser", nameInput).Wait();
            //Console.WriteLine("--> {0}", indexNumber);
        }


        public void InitTimer() {
            Console.WriteLine("- Timer started -");
            _timer = new System.Timers.Timer();
            //=- 10% 
            Random random = new Random();

            int randomrange = random.Next(0, 200);
            int randomInterval = 900 + randomrange;
            _timer.Interval = randomInterval;
            // Create a timer with a X milli second interval.
            //_timer = new System.Timers.Timer(3000);
            _timer.Elapsed += (sender, e) => uploadData(sender, e, this);// Hook up the Elapsed event for the timer.
            _timer.AutoReset = true;// Have the timer fire repeated events (true is the default)
            _timer.Enabled = true;// Start the timer
            _timer.Start();
        }

        private static void uploadData(object sender, EventArgs e, UserConnection user)
        {
            try
            {
                //Console.WriteLine("uploadData for User ID : {0} Group ID : {1}",  user.Connection.ConnectionId ,user.GroupId);
                //user.MyHub.Invoke<object[]>("UploadData", user.GroupId, user.Connection.ConnectionId, 10);
                user.MyHub.Invoke<object[]>("UploadData", new PlayerState{ Id =  user.Connection.ConnectionId, GroupId = user.GroupId, Clicks = 10  } );
                //userConnectionList.FirstOrDefault().MyHub.Invoke<object[]>("UploadData", "groupId", "playerId", "presses");
            }
            catch (Exception ex) {
                Console.WriteLine("uploadData error : " + ex.Message);
            }
        }


        public void EndConnection() {
            this.Connection.Stop();
            this._timer.Stop();
        }




        #endregion 



    }
}

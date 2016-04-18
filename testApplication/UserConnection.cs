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


        #region private variables

        public HubConnection Connection;
        public IHubProxy MyHub;
        public string GroupId;

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


        private void simulateUploadData()
        {
            



        }






        #endregion 



    }
}

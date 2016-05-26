using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using System.Diagnostics;

namespace testApplication
{
    class UserConnection
    {


        #region public variables
            public HubConnection Connection;
            public IHubProxy MyHub;
            public string GroupId;
            public int CurrentClicks;
            
            //public bool HasClicksChangedFlag { get; set; }
            public ClickChangeStatus ClickChangeState { get; set; }
            public Stopwatch StopWatch;
            public HubBlockingCollection<int> roundtripTimesMsList;
            public bool BlockData;
        #endregion

        #region private variables
            private System.Timers.Timer _timer;
            private System.Timers.Timer _clickChangeTimer;
        #endregion


        /*
         * Constructor         
         */
        public UserConnection(int indexNumber) {
            this.CurrentClicks = 0;
            //this.HasClicksChangedFlag = false;
            this.ClickChangeState = ClickChangeStatus.None;
            this.StopWatch = new Stopwatch();
            this.roundtripTimesMsList = new HubBlockingCollection<int>();
            this.BlockData = false;
        }


        public enum ClickChangeStatus 
        {
            None,
            Set,
            Uploaded,
            Received
        }


        #region Methods

        /// <summary>
        /// Start this instance of timer for the upload data periodically
        /// </summary>
        public void InitTimer() 
        {
            this._timer = new System.Timers.Timer();
            int randomInterval = this.generateRandomRange(Program.settings.timerRepeatMs, Program.settings.randomPercentage);
            if (Program.settings.checkClicksTest_instancesToMonitorCounter > 0) { //only start Click test for this instance if specified in settings
                this.checkClickTestInit();
                Program.settings.decrement_instancesToMonitor();
            }
            this._timer.Interval = randomInterval;
            this._timer.Elapsed += (sender, e) => uploadData(sender, e, this);// Hook up the Elapsed event for the timer.
            this._timer.AutoReset = true;// Have the timer fire repeated events (true is the default)
            this._timer.Enabled = true;// Start the timer
            this._timer.Start();
        }


        /// <summary>
        /// Test that will change the ammount of 'clicks' every random interval, the round trip time is measured
        /// </summary>
        private void checkClickTestInit() {
            int intervalMS = this.generateRandomRange(
                                    Program.settings.checkClicksTest_valueChangeIntervalMs, 
                                    Program.settings.checkClicksTest_valueChangeTimeVariabilityPct 
                                    ); //change click ammount every 10 secs, interval 10%

            Program.userConnectionMonitoringList.Add(this);
            this._clickChangeTimer = new System.Timers.Timer();
            this._clickChangeTimer.Interval = intervalMS;
            this._clickChangeTimer.Elapsed += (sender, e) => changeClicks(sender, e, this);// Hook up the Elapsed event for the timer.
            this._clickChangeTimer.AutoReset = true;// Have the timer fire repeated events (true is the default)
            this._clickChangeTimer.Enabled = true;// Start the timer
            this._clickChangeTimer.Start();
        }

        /// <summary>
        /// Change click ammount every random interval, used to measure round trip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="user"></param>
        private static void changeClicks(object sender, EventArgs e, UserConnection user)
        {
            if(user.BlockData) { return; }
            Random random = new Random();
            int value = random.Next(Program.settings.checkClicksTest_rangeLower, Program.settings.checkClicksTest_rangeUpper); //generate random between 0 - 100
            user.CurrentClicks = value;
            //user.HasClicksChangedFlag = true;
            user.ClickChangeState = ClickChangeStatus.Set;
        }
        
        /// <summary>
        /// Static method to upload data to server periodically
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="user"></param>
        private static void uploadData(object sender, EventArgs e, UserConnection user)
        {
            if (user.BlockData) { return; }
            try
            {
                if (user.ClickChangeState == ClickChangeStatus.Set) //user.HasClicksChangedFlag
                {
                    user.StopWatch.Reset();
                    user.StopWatch.Start();
                }
                user.MyHub.Invoke<object[]>("UploadData", new PlayerState { Id = user.Connection.ConnectionId, GroupId = user.GroupId, Clicks = user.CurrentClicks });
                //user.HasClicksChangedFlag = false; //reset flag
                user.ClickChangeState = ClickChangeStatus.Uploaded;
            }
            catch (Exception ex) {
                Console.WriteLine("uploadData error : " + ex.Message);
            }
        }

        /// <summary>
        /// Used to Clean up connections at end
        /// </summary>
        public void EndConnection() {
            this._timer.Stop();
            this.Connection.Stop();
        }


        /// <summary>
        /// Public accessor metod for round trip time taken (from UploadData to UpdateGame)
        /// </summary>
        /// <returns></returns>
        public int getRoundTripTimeMs()
        {
            this.StopWatch.Stop();
            return (int) this.StopWatch.ElapsedMilliseconds;
        }


        // ------------------ Helper Methods ------------------  
        /// <summary>
        /// A time is generated over a range specified, the last percentage (specified by randomPercentage) will be a random timespan within the that percentage
        /// </summary>
        /// <param name="timerRepeatMs"></param>
        /// <param name="randomPercentage"></param>
        /// <returns></returns>
        private int generateRandomRange(int timerRepeatMs, int randomPercentage)
        {
            Random random = new Random();
            float percentageRange = (float)randomPercentage / (float)100;
            //Console.WriteLine("percentageRange : {0}", percentageRange.ToString());
            float msRangeF = (float)timerRepeatMs * (float)percentageRange;
            int msRange = (int)msRangeF;
            int restOfTimeMs = (int)timerRepeatMs - msRange;
            int randomrange = random.Next(0, msRange);
            int randomInterval = restOfTimeMs + randomrange;
            //Console.WriteLine("restOfTimeMs : {0} , randomrange : {1}", restOfTimeMs.ToString(), randomrange.ToString());
            return randomInterval;
        }

        #endregion 



    
        
    
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testApplication
{


    /// <summary>
    /// Class is purly for holding config settings for running the test console
    /// </summary>
    class TestSettings
    {
        
        public int connectionsQuantity  { get; private set; }   //number of test connections generated
        public int timerRepeatMs        { get; private set; }   //the time between each event firing to server
        public int randomPercentage { get; private set; }       //the percentage of the interval that is random 0-100% to help randomly space time interval so not firing at same time
        public bool quickTestMode   { get; private set; }       //skips all press key steps to start test

        //Settings for click test
        public bool checkClicksTest { get; private set; }                       //Will confirm that clicks sent by a connections are consistent throughout all in group
        public int checkClicksTest_instancesToMonitor { get; private set; }     //instances to monitor for check Click test
        public int checkClicksTest_valueChangeIntervalMs { get; private set; }  //will change every interval as specified
        public int checkClicksTest_valueChangeTimeVariabilityPct { get; private set; }  // variabilty percentage of total time (e.g. for 10% and time of 1000, 900ms + random ammount between 0-100ms )
        public int checkClicksTest_rangeUpper { get; private set; }             //click value will be generated as an integer value between rangeUpper and rangeLower
        public int  checkClicksTest_rangeLower { get; private set; }
        public int checkClicksTest_instancesToMonitorCounter { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionsQuantityIn"></param>
        /// <param name="timerRepeatMsIn"></param>
        /// <param name="randomPercentageIn"></param>
        /// <param name="checkClicksTestIn"></param>
        /// <param name="quickTestModeIn"></param>
        /// <param name="checkClicksTest_instancesToMonitorIn"></param>
        public TestSettings(
            int connectionsQuantityIn = 4, 
            int timerRepeatMsIn = 1000, 
            int randomPercentageIn = 10, 
            bool checkClicksTestIn = false, 
            bool quickTestModeIn = false, 
            int checkClicksTest_instancesToMonitorIn = 1, 
            int checkClicksTest_valueChangeIntervalMsIn = 1000,
            int checkClicksTest_valueChangeTimeVariabilityPctIn = 10,
            int checkClicksTest_rangeUpperIn = 100,
            int checkClicksTest_rangeLowerIn = 0
        )
        { 
            this.connectionsQuantity    = connectionsQuantityIn;
            this.timerRepeatMs          = timerRepeatMsIn;
            this.randomPercentage       = randomPercentageIn >= 0 && randomPercentageIn <= 100 ? randomPercentageIn : 0; // check limits are between 0-100%
            this.quickTestMode          = quickTestModeIn;
            this.checkClicksTest = checkClicksTestIn;
            this.checkClicksTest_instancesToMonitor = checkClicksTest_instancesToMonitorIn;
            this.checkClicksTest_instancesToMonitorCounter = checkClicksTest_instancesToMonitorIn;
            
            Console.WriteLine("interval this -> {0}, interval in -> {1}", this.checkClicksTest_instancesToMonitor, checkClicksTest_instancesToMonitorIn);
            
            this.checkClicksTest_valueChangeIntervalMs = checkClicksTest_valueChangeIntervalMsIn;
            this.checkClicksTest_valueChangeTimeVariabilityPct = checkClicksTest_valueChangeTimeVariabilityPctIn;
            this.checkClicksTest_rangeUpper = checkClicksTest_valueChangeIntervalMsIn;
            this.checkClicksTest_rangeLower = checkClicksTest_valueChangeIntervalMsIn;
        }

        internal void decrement_instancesToMonitor()
        {
            if (this.checkClicksTest_instancesToMonitorCounter > 0) {
                this.checkClicksTest_instancesToMonitorCounter--;
            }
        }

    }
}

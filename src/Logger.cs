using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Simulator1
{
    class Logger
    {
        StreamWriter log, trace;
        bool trace_is_open = false;
        private Object thisLock = new Object();
        //variables that hold references to the regs and RAM

        //CPU instantiation


//Trace Functions
        private static Logger instance;

        private Logger() 
        {
            toggleTrace();
        }

        
        public static Logger Instance{
            get {
                if (instance == null){
                    instance = new Logger();
                }
            return instance;
            }
        }


        //returns the status fo trace
        public bool getTraceStatus() { return trace_is_open; }

        //switches trace from on->off or off->on
        public bool toggleTrace()
        {
            if (trace_is_open) 
            {
                this.closeTrace();
                this.writeLog("Trace: Closed");
                return false;
            }
            else
            {
                if (!trace_is_open)
                {
                    this.openTrace();
                    this.writeLog("Trace: Opened");
                } 
                return true;

            }
        }
        //closes the trace file
        // flushes the buffer out
        //closes for reading
        public void closeTrace() 
        {
            lock (thisLock)
            {
                if (trace_is_open)
                {
                    this.trace.Flush();
                    this.trace.Close();
                }
                this.trace_is_open = false;
            }
        }

        //Opens trace (also clears the file)
        public void openTrace()
        {
            lock (thisLock)
            {
                if (!trace_is_open) //checks to see if it's not opened
                {
                    this.trace = new StreamWriter("trace.log");
                    this.trace_is_open = true;
                }
            }
        }

        
        // if enabled will write to the trace.log file
        // to keep a trace
        public void writeTrace(Computer  myComp)
        {
            lock (thisLock) {
                if (trace_is_open)
                {
                    //step_number program_counter checksum nzcf r0 r1 r2 r3
                    string firstLine = "";
                    firstLine = (myComp.getStepNumber().ToString().PadLeft(6, '0') + ' ' +
                                    Convert.ToString(myComp.currentAddress, 16).PadLeft(8, '0')).ToUpper() + ' ' +
                                    "[sys]" + ' ' +
                                    Convert.ToInt32(myComp.getFlag('N')) + Convert.ToInt32(myComp.getFlag('Z')) +
                                    Convert.ToInt32(myComp.getFlag('C')) + Convert.ToInt32(myComp.getFlag('F')) +
                                    (" 0=" + myComp.getReg(0).getRegString() +
                                    " 1=" + myComp.getReg(1).getRegString() +
                                    " 2=" + myComp.getReg(2).getRegString() +
                                    " 3=" + myComp.getReg(3).getRegString()).ToUpper();

                    this.trace.WriteLine(firstLine);

                    //r4 r5 r6 r7 r8 r9
                    this.trace.WriteLine(("\t4=" + myComp.getReg(4).getRegString() + ' ' +
                                    " 5=" + myComp.getReg(5).getRegString() + ' ' +
                                    " 6=" + myComp.getReg(6).getRegString() + ' ' +
                                    " 7=" + myComp.getReg(7).getRegString() + ' ' +
                                    " 8=" + myComp.getReg(8).getRegString() + ' ' +
                                    "9=" + myComp.getReg(9).getRegString()).ToUpper());
                    //r10 r11 r12 r13 r14
                    this.trace.WriteLine(("\t10=" + myComp.getReg(10).getRegString() + ' ' +
                                    "11=" + myComp.getReg(11).getRegString() + ' ' +
                                    "12=" + myComp.getReg(12).getRegString() + ' ' +
                                    "13=" + myComp.getReg(13).getRegString() + ' ' +
                                    "14=" + myComp.getReg(14).getRegString()).ToUpper());
                    this.trace.Flush();
                }
            }
        }

//Log functions
        public void clearLog()
        {
            log.Close();
            log = new StreamWriter("log.txt");
            log.Close();
        }

        //opens the file for writing. writes to it, and closes.
        internal void writeLog(string p)
        {
           lock(thisLock)
           {
                //if(log == null)
                log = new StreamWriter("log.txt", true);
                DateTime now = DateTime.Now;
                log.WriteLine(now.ToString() + " | " + p);
                log.Close();
            }
        }//write Log


        internal string byteArrayToString(byte[] x)
        {
            string output = "";
            for (int i = 0; i < x.Length; ++i)
            {
                output += Convert.ToString(x[i], 16).PadLeft(2, '0');
            }
            return output;
        }
    }//trace class

}//namespace


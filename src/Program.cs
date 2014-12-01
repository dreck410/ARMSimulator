using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;



//I guess this doesn't work..... worth a try
//using School.BJU.Class.CPS310.MicroProcessor.Version.100

namespace Simulator1
{
   
    class Program
    {
        static void Main(string[] args)
        {

            Logger.Instance.clearLog();
            if (Option.Instance.parseArgs(args))    //verify the proper command line input
            {
                Logger.Instance.clearLog();

                //runs test cases
                if (Option.Instance.getTest())
                {
                    try
                    {
                        TestRam.RunTests();
                        TestSimulator.RunTests();
                        TestDecodeExecute TFDE = new TestDecodeExecute();
                        TFDE.RunTests();
                        Console.WriteLine("Tests Run");
                        Logger.Instance.writeLog("TEST: Finished\n\n\n------------------------------");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Test Failed please see log.txt for details");
                        Logger.Instance.writeLog("\n\n\nTest: Failed\n\n\n");
                    }

                }

                Logger.Instance.openTrace();

                // pre-loads a file
                if (Option.Instance.getFile() != "")
                {
                    //wanted some data to play with
                    //specified through command line, load the file
                    Computer.Instance.load(Option.Instance.getFile(), Option.Instance.getMemSize());
                    if (Option.Instance.execute == true)
                    {
                        //loaded and wants to be executed.
                        Computer.Instance.run();
                        while (Computer.Instance.getThreadStatus()) { ;}
                        // System.Environment.Exit(0);

                    }
                }
                // listens for gdb connection
                if (Option.Instance.getDebug())
                {

                    Handler h = new Handler();

                    h.Listen(8080);
                }
                //if debug flag is not set. run            
            }  
        }
    }//programClass
}//namespace



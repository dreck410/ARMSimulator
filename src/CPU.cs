using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;


namespace Simulator1
{

    /*
     * CPU class that represents the CPU. This should have methods named 
     * fetch, decode, and execute. The CPU class should have instance variables 
     * that hold references to the registers and RAM objects.
     * 
     */
    class CPU
    {
        private Memory RAM;
        private Register[] reg;
        //test
        //variables that hold references to the regs and RAM
        //dddddd
        //CPU instantiation

        public CPU(ref Memory RAM, ref Register[] reg)
        {
            this.RAM = RAM;
            this.reg = reg;
        }


        //fetches data from RAM 
        public Memory fetch()
        {
            Memory cmd = new Memory(4);
            cmd.WriteWord(0, RAM.ReadWord(reg[15].ReadWord(0)));
            Logger.Instance.writeLog(String.Format("CMD: 0x{0}", Convert.ToString(cmd.ReadWord(0), 16)));

            return cmd;
           

        }

        //decodes the int into a command.  like mov r0, r1
        public Instruction decode(Memory data)
        {
            //data.WriteWord(0,0xe3a02030);//e3a02030 mov r2, #48
            InstructionParser parser = new InstructionParser();
            Instruction inst = parser.parse(data);

            return inst;
        }

        //executes the actual data by movine registers and stuff
        public bool[] execute(Instruction command, bool[] flagsNZCF)
        {
            if ((command.checkCond(flagsNZCF)))
            {
                command.run(ref reg, ref RAM);
                if (command.S)
                {
                    bool[] flags = { command.N, command.Z, command.C, command.F };
                    return flags;
                }

            }
            else
            {
                Logger.Instance.writeLog(string.Format("CMD: Condition Code Stopped Execution = {0}", command.condStr));
            }

            return null;

        }


    }
}

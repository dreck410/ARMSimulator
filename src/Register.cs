using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator1
{
    //Registers are a type of Memory.
    class Register : Memory
    {
        //creates a register. they are all 32 bits or 4 bytes. yay!!!
        public uint regID { get; set; }

        public Register()
        {
            theArray = new byte[4];
        }

        public override uint ReadWord(uint addr, bool execute = false)
        {
            uint regVal = base.ReadWord(addr);

            if (regID == 15 && execute) 
            {
                regVal += 8; 
            }

            return regVal;
        }

        //displays the register
        public byte[] getRegister()
        {
            return theArray;
        }


        internal string getRegString()
        {
            return Convert.ToString(ReadWord(0),16).PadLeft(8, '0');;
        }
    }
}

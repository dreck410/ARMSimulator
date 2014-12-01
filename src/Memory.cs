using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;

namespace Simulator1
{
    class Memory
    {
        protected byte[] theArray;
        protected byte Adr0x00100000 = 0;
        protected byte Adr0x00100001 = 0;
        //program counter
        //Int32 pc;

        //Memory Constructor that takes no arguments
        //auto designates ram size to 32768
        public Memory()
        {
            if (theArray == null)
            {
                theArray = new byte[32768];
            }
        }

        public Memory(int memSize)
        {
            if (theArray == null)
            {
                theArray = new byte[memSize];
            }
        }
        
        public string getHash()
        {
            MD5 hasher = new MD5();

            return hasher.Hash(theArray);
        }

        //gets the ram for a desired num of lines at an address
        public string getAtAddress(uint addr, int desiredLines = 8)
        {
            int numOfLines = 0;
            int numOfBytes = 0;
            string output = "";
            output = "RAM: starting at: " + addr.ToString("X2").PadLeft(8, '0') + "\n";
            output += "0x" + addr.ToString("X2").PadLeft(8, '0') + ": ";
            for (; numOfLines < desiredLines && addr < theArray.Length; addr++)
            {
                
                output += theArray[addr].ToString("X2");
                if ((numOfBytes + 1) % 2 == 0 && numOfBytes != 0)
                {
                    output += " ";
                }
                if ((numOfBytes + 1) % 16 == 0 && numOfBytes != 0)
                {
                    output += "\n";
                    output += "0x" + (addr + 1).ToString("X2").PadLeft(8, '0') + ": ";
                    numOfLines += 1;
                }
                numOfBytes++;
            }
            return output;
        }


        //do you really want the entirety of RAM?
        public byte[] getArray()
        {
            return theArray;
        }


        public bool TestFlag(UInt32 addr, byte bit)
        {
            if (bit >= 0 && bit < 32)
            {
                uint word = ReadWord(addr);
                // Shift it all the way to the rightwe only care about the LSB
                if (((word >> bit) & 0x001) == 1 ) 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //if invalid input, return false
            return false;

        }//TestFlag

        public void SetFlag(UInt32 addr, byte bit, bool flag)
        {
            uint power = 0;
            uint num = 1;
            for (; power < bit; ++power){ num *= 2;}    //get's power to one less the bit.
            uint word = ReadWord(addr);
            if (flag)
                word |= num;
            else if (TestFlag(addr, bit)) //if it's a 1 change it, otherwise it's already a 0.
            {
                word ^= num;
            }

            WriteWord(addr, word );//write it back to memory
        }//setFlag

        public virtual uint ReadWord(UInt32 addr, bool execute = false)
        {
            uint output = 0;
            //if address is not divisible by 4 escape
            if (addr % 4 == 0)
            {
                try
                {
                    output += (uint)(theArray[addr + 3] << 24);
                    output += (uint)(theArray[addr + 2] << 16);
                    output += (uint)(theArray[addr + 1] << 8);
                    output += (uint)(theArray[addr]);
                }
                catch
                {
                    Logger.Instance.writeLog("\n\nFIX READ WORD\nFIX READ WORD!!!\nFIX READ WORD!!!\n");
                }

                //output = BitConverter.ToUInt32(theArray, addr);
            }
            return output;
        }//ReadWord

        public ushort ReadHalfWord(UInt32 addr)
        {
            ushort output = 0;
            //if address is not divisible by 4 escape
            if (addr % 2 == 0)
            {
                output += (ushort)(theArray[addr + 1] << 8);
                output += (ushort)(theArray[addr]);
            }
            return output;
        }//ReadHalfWord

        public byte ReadByte(UInt32 addr)
        {
            if (addr == 0x00100001)
            {
                if (Computer.Instance.inputQueue.Count != 0)
                {
                    Adr0x00100001 = (byte)Computer.Instance.inputQueue.Dequeue();
                    Logger.Instance.writeLog(String.Format("IO: {0} From Queue", Adr0x00100001));
                }
                else
                {
                    Adr0x00100001 = 0x0;
                }

                return Adr0x00100001;
            }
            else
            {
                //used for polling
                if (addr == 0x00100000)
                {
                    return Adr0x00100000;
                }
            }
            byte output = theArray[addr];
            return output;
        }//ReadByte


        public void WriteWord(UInt32 addr, uint inpu)
        {
            if (addr % 4 == 0)
            {
                byte[] intBytes = BitConverter.GetBytes(inpu);
                Array.Copy(intBytes, 0, theArray, addr, 4);
            }
        }//WriteWord

        public void WriteHalfWord(UInt32 addr, ushort inpu)
        {
            if (addr % 2 == 0)
            {
                byte[] shortBytes = BitConverter.GetBytes(inpu);
                Array.Copy(shortBytes, 0, theArray, addr, 2);
            }
        }//WriteHalfWord

        public void WriteByte(UInt32 addr, byte inpu)
        {
            if (addr == 0x00100000)
            {
               // Console.WriteLine("Got Some Input");
                Logger.Instance.writeLog(String.Format("IO: Received {0}", (char)inpu));
                Console.Write((char)inpu);
            }
            else
            {
                theArray[addr] = inpu;
            }
        }//WriteByte

        public void CLEAR()
        {
            Array.Clear(theArray, 0, theArray.Length);
        }


        //returns a byte array of specified size
        //need ot know what size means
        internal byte[] dump(uint addr = 0, int length = 32)
        {
            byte[] output = new byte[length];
            for (int i = 0; i < length; ++i, ++addr)
            {
                output[i] = ReadByte(addr);
            }
                return output;
        }

        internal string print()
        {
            string output = "";


            return output;
        }
    }
}

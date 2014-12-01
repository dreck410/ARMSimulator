using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;


namespace Simulator1
{
    public class TestSimulator
    {

        public static void RunTests()
        {
            //append

            ELFReader e = new ELFReader();
            Memory ram = new Memory(32768);
            Computer comp = new Computer();
            Logger.Instance.closeTrace();
            Logger.Instance.writeLog("Test: Starting Simulator unit tests");

            Logger.Instance.writeLog("Test: Testing Hash of test1.exe");
            comp.load("test1.exe", 32768);

            string resultHash = comp.getRAM().getHash();
            string hash = "3500a8bef72dfed358b25b61b7602cf1";

            Debug.Assert(hash.ToUpper() == resultHash);

            comp.CLEAR();

            Logger.Instance.writeLog("Test: Testing Hash of test2.exe");
            comp.load("test2.exe", 32768);
            resultHash = comp.getRAM().getHash();

            hash = "0a81d8b63d44a192e5f9f52980f2792e";

            Debug.Assert(hash.ToUpper() == resultHash);
            

            comp.CLEAR();

            Logger.Instance.writeLog("Test: Testing Hash of test3.exe");
            comp.load("test3.exe", 32768);


            resultHash = comp.getRAM().getHash();
            hash = "977159b662ac4e450ed62063fba27029";

            Debug.Assert(hash.ToUpper() == resultHash);
            
            Logger.Instance.writeLog("Test: All Hashes correct\n");

            //Logger.Instance.toggleTrace();
            comp.reset();

        }

    }

    public class TestRam
    {

        public static void RunTests()
        {

            //append
            Logger.Instance.writeLog("Test: Starting RAM unit tests");
            Memory tram = new Memory(32768);
            Logger.Instance.closeTrace();

            Logger.Instance.writeLog("Test: Read/Write Byte");
            byte byteRes = tram.ReadByte(0);
            Debug.Assert(byteRes == 0);
            tram.WriteByte(0, 0xee);
            byteRes = tram.ReadByte(0);
            Debug.Assert(byteRes == 0xee);

            tram.CLEAR();

            Logger.Instance.writeLog("Test: Read/Write HalfWord");
            ushort shortRes = tram.ReadHalfWord(0);
            Debug.Assert(shortRes == 0);
            tram.WriteHalfWord(0, 0xeef);
            shortRes = tram.ReadHalfWord(0);
            Debug.Assert(shortRes == 0xeef);

            tram.CLEAR();

            Logger.Instance.writeLog("Test: Read/Write Word");
            uint intRes = tram.ReadWord(0);
            Debug.Assert(intRes == 0);
            tram.WriteWord(0, 0xabcdef);
            intRes = tram.ReadWord(0);
            Debug.Assert(intRes == 0xabcdef);

            tram.CLEAR();

            Logger.Instance.writeLog("Test: Set/Test Flag");

            bool flagRes = tram.TestFlag(0, 4);
            Debug.Assert(flagRes == false);

            //set a false flag true
            tram.SetFlag(0, 4, true);
            flagRes = tram.TestFlag(0, 4);
            Debug.Assert(flagRes == true);

            // true >> true
            tram.SetFlag(0, 4, true);
            flagRes = tram.TestFlag(0, 4);
            Debug.Assert(flagRes == true);

            // true >> false
            tram.SetFlag(0, 4, false);
            flagRes = tram.TestFlag(0, 4);
            Debug.Assert(flagRes == false);

            // false >> false
            tram.SetFlag(0, 4, false);
            flagRes = tram.TestFlag(0, 4);
            Debug.Assert(flagRes == false);

            Logger.Instance.writeLog("Test: All Ram Tests passed\n");
            Logger.Instance.closeTrace();
            tram.CLEAR();

        }

    }


    public class TestDecodeExecute
    {

        Memory RAM = new Memory();
        Register[] reg = new Register[16];
        CPU cpu;
        public void RunTests()
        {

            //append
            Logger.Instance.writeLog("Test: Fetch Decode Execute");
            
            Logger.Instance.closeTrace();
            //0xe3a02030 mov r2, #48
            //defines 16 registers, 0 - 15
            for (uint i = 0; i < 16; i++)
            {
                this.reg[i] = new Register();
                this.reg[i].regID = i;
            }

            cpu = new CPU(ref RAM, ref reg);

                    
            //put the instruction into memory

            Logger.Instance.writeLog("TEST: mov r2, #48 : 0xe3a02030");
            this.runCommand(0xe3a02030);
            Debug.Assert(reg[2].ReadWord(0) == 48);
            Logger.Instance.writeLog("TEST: Executed\n");


            Logger.Instance.writeLog("TEST: mov r0, r3 : 0xe1a00003");
            reg[3].WriteWord(0, 3);
            this.runCommand(0xe1a00003);
            Debug.Assert(reg[0].ReadWord(0) == 3);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: mov r0, r3 lsl #4 : 0xe1a00403");
            reg[3].WriteWord(0, 3);
            this.runCommand(0xe1a00403);
            Debug.Assert(reg[0].ReadWord(0) == 0x300);
            Logger.Instance.writeLog("TEST: Executed\n");


            Logger.Instance.writeLog("TEST: mov r0, r1 lsl r2 : 0xe1a00211");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 1);
            reg[2].WriteWord(0, 1);
            this.runCommand(0xe1a00211);
            Debug.Assert(reg[0].ReadWord(0) == 0x2);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: mov r0, r1 lsr r2 : 0xe1a00231");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 0xF);
            reg[2].WriteWord(0, 1);
            this.runCommand(0xe1a00231);
            Debug.Assert(reg[0].ReadWord(0) == 0x7);
            Logger.Instance.writeLog("TEST: Executed\n");

            //dd
            Logger.Instance.writeLog("TEST: mov r0, r1 asr r2 : 0xe1a00251");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 0x8000000F);
            reg[2].WriteWord(0, 1);
            this.runCommand(0xe1a00251);
            Debug.Assert(reg[0].ReadWord(0) == 0xC0000007);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: mov r0, r1 ror r2 : 0xe1a00271");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 0xF);
            reg[2].WriteWord(0, 2);
            this.runCommand(0xe1a00271);
            Debug.Assert(reg[0].ReadWord(0) == 0xC0000003);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: mvn R3, 1 : 0xe1e03001");
            reg[3].WriteWord(0, 0);
            reg[1].WriteWord(0, 0x6);
            this.runCommand(0xe1e03001);
            Debug.Assert(reg[3].ReadWord(0) == 0xFFFFFFF9);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: rsb r0, r1, r2 : 0xe0610002");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 5);
            reg[2].WriteWord(0, 10);
            this.runCommand(0xe0610002);
            Debug.Assert(reg[0].ReadWord(0) == 5);
            Logger.Instance.writeLog("TEST: Executed\n");

            //0xe0050291
            Logger.Instance.writeLog("TEST: mul r0, r1, r2 : 0xe0000291");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 5);
            reg[2].WriteWord(0, 10);
            this.runCommand(0xe0000291);
            Debug.Assert(reg[0].ReadWord(0) == 50);
            Logger.Instance.writeLog("TEST: Executed\n");
            /*
            Logger.Instance.writeLog("TEST: mul r0, r1, r2 : 0xe0000291");
            reg[0].WriteWord(0, 0);
            reg[1].WriteWord(0, 0xFFFFFFFF);
            reg[2].WriteWord(0, 0xFFFFFFFF);
            this.runCommand(0xe0000291);
            Debug.Assert(reg[0].ReadWord(0) == 50);
            Logger.Instance.writeLog("TEST: Executed\n");
            */

            Logger.Instance.writeLog("TEST: str r2, [r1] : 0xe5812000");
            reg[1].WriteWord(0, 0x40);
            reg[2].WriteWord(0, 0x1234);
            this.runCommand(0xe5812000);
            Debug.Assert(RAM.ReadWord(0x40) == 0x1234);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: ldr r2, [r1] : 0xe5912000");
            reg[1].WriteWord(0, 0x40);
            RAM.WriteWord(0x40, 0x1234);
            this.runCommand(0xe5912000);
            Debug.Assert(reg[2].ReadWord(0) == 0x1234);
            Logger.Instance.writeLog("TEST: Executed\n");


            Logger.Instance.writeLog("TEST: Add R9, R8, #2147483648 : 0xe2889102");
            reg[8].WriteWord(0, 10);
            this.runCommand(0xe2889102);
            Debug.Assert(reg[9].ReadWord(0) == 2147483648 + 10);
            Logger.Instance.writeLog("TEST: Executed\n");


            //test 0xe24dd008 sub r13, r13, #8
            Logger.Instance.writeLog("TEST: sub R12, R13, #8 : 0xe24dd008");
            reg[13].WriteWord(0, 10);
            reg[12].WriteWord(0, 0);
            this.runCommand(0xe24dc008);
            Debug.Assert(reg[12].ReadWord(0) == 2);
            Logger.Instance.writeLog("TEST: Executed\n");

            //test 0xeb000006 b 6;
            Logger.Instance.writeLog("TEST: B #6 : 0xea000006");
            reg[15].WriteWord(0, 0);

            reg[14].WriteWord(0, 0x477);
            this.runCommand(0xea000006);
            Debug.Assert(reg[15].ReadWord(0) == 0x18 + 4);
            Debug.Assert(reg[14].ReadWord(0) == 0x477);
            Logger.Instance.writeLog("TEST: Executed\n");

            //test 0xeb000006 bl -6;
            Logger.Instance.writeLog("TEST: B #-24 : 0xeb8FFFFFA");
            reg[15].WriteWord(0, 0);
            reg[14].WriteWord(0, 48);
            this.runCommand(0xebFFFFFA);
            Debug.Assert(reg[15].ReadWord(0) == 0xFFFFFFF0 - 4);
            Debug.Assert(reg[14].ReadWord(0) == 0x4);
            Logger.Instance.writeLog("TEST: Executed\n");

            //test 0xe12fff12
            Logger.Instance.writeLog("TEST: BX R2");
            reg[2].WriteWord(0, 0x100);
            reg[15].WriteWord(0, 0);
            this.runCommand(0xE12FFF12);
            Debug.Assert(reg[15].ReadWord(0) == 0x100 - 0x4);
            Logger.Instance.writeLog("Test: Executed\n");

            //test 0xe92d4800 strm r1, r14, r11 U = 0 P = 1 W = 1
            Logger.Instance.writeLog("TEST: strm - r1, r14, r11 ! : 0xe9214800");
            reg[11].WriteWord(0, 0x4F3);
            reg[14].WriteWord(0, 0x48);
            reg[1].WriteWord(0, 0x20);
            this.runCommand(0xe9214800);

            //lower register is always lower in memory
            Debug.Assert(RAM.ReadWord(0x18) == 0x4F3);
            Debug.Assert(RAM.ReadWord(0x1c) == 0x48);
            Debug.Assert(reg[1].ReadWord(0) == 24);
            Logger.Instance.writeLog("TEST: Executed\n");

            //test 0xe88d4800 strm r13, r14, r11 U = 1 P = 0 W = 0
            Logger.Instance.writeLog("TEST: strm r1, r14, r11 : 0xe8214800");
            reg[11].WriteWord(0, 0x4F3);
            reg[14].WriteWord(0, 0x48);
            reg[1].WriteWord(0, 0x20);
            this.runCommand(0xe8814800);
            Debug.Assert(RAM.ReadWord(0x20) == 0x4f3);
            Debug.Assert(RAM.ReadWord(0x24) == 0x48);
            Debug.Assert(reg[1].ReadWord(0) == 0x20);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: ldrm r0, r13, r1 : 0xe9904002");
            reg[0].WriteWord(0, 0x20);
            reg[13].WriteWord(0, 0);
            reg[1].WriteWord(0, 0);
            this.runCommand(0xe8902002);
            Debug.Assert(reg[1].ReadWord(0) == 0x4f3);
            Debug.Assert(reg[13].ReadWord(0) == 0x48);
            Logger.Instance.writeLog("TEST: Executed\n");

            
            Logger.Instance.writeLog("TEST: cmp R1, R2 ");
            reg[1].WriteWord(0, 0x10);
            reg[2].WriteWord(0, 0x11);
            bool[] flagsNZCF = this.runCommand(0xE1510002);
            Debug.Assert(flagsNZCF[0]);
            Debug.Assert(!flagsNZCF[1]);
            Debug.Assert(!flagsNZCF[2]);
            Debug.Assert(!flagsNZCF[3]);
            Logger.Instance.writeLog("TEST: CMP 0x10 and 0x11");
            Logger.Instance.writeLog("TEST: Executed\n");

            reg[1].WriteWord(0, 0x10);
            reg[2].WriteWord(0, 0x10);
            flagsNZCF = this.runCommand(0xE1510002);
            Debug.Assert(!flagsNZCF[0]);
            Debug.Assert(flagsNZCF[1]);
            Debug.Assert(flagsNZCF[2]);
            Debug.Assert(!flagsNZCF[3]);
            Logger.Instance.writeLog("TEST: CMP 0x10 and 0x10");
            Logger.Instance.writeLog("TEST: Executed\n");

            reg[1].WriteWord(0, 0x11);
            reg[2].WriteWord(0, 0x10);
            flagsNZCF = this.runCommand(0xE1510002);
            Debug.Assert(!flagsNZCF[0]);
            Debug.Assert(!flagsNZCF[1]);
            Debug.Assert(flagsNZCF[2]);
            Debug.Assert(!flagsNZCF[3]);
            Logger.Instance.writeLog("TEST: CMP 0x10 and 0x11");
            Logger.Instance.writeLog("TEST: Executed\n");


            Logger.Instance.writeLog("TEST: AddEQ R9, R8, #2147483648 : 0x02889102");
            reg[8].WriteWord(0, 10);
            this.runCommand(0x02889102);
            Debug.Assert(reg[9].ReadWord(0) == 2147483648 + 10);
            Logger.Instance.writeLog("TEST: Executed\n");

            Logger.Instance.writeLog("TEST: All Decode/Execute Tests Passed");

            Logger.Instance.closeTrace();
 
        }

        private bool[] runCommand(uint p)
        {
            RAM.WriteWord(0, p);

            //get the program counter to point at the test command
            reg[15].WriteWord(0, 0);

            //fetch, decode, execute commands here
            Memory rawInstruction = cpu.fetch();

            /// make sure we fetched the right hting
            Debug.Assert(rawInstruction.ReadWord(0) == p);

            Logger.Instance.writeLog("TEST: Fetched");


            //decode the uint!
            Instruction cookedInstruction = cpu.decode(rawInstruction);
           // Debug.Assert(cookedInstruction is dataManipulation);
            Logger.Instance.writeLog("TEST: Decoded");

            //exeucte the decoded Command!!
            bool[] flags = {false, true, false, false};
            return cpu.execute(cookedInstruction, flags);
        }//runTests

    }//testDecodeExecute

}//namespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Simulator1
{
    //the instruction class object
    class InstructionParser
    {

        uint type = 0;          // xx   2 determines IPUBWL, or Opcode


        Instruction instruct = new Instruction();


        public Instruction parse(Memory command)
        {

            //get the type number
            this.type = (uint)((command.ReadByte(3) & 0x0c) >> 2);
            //Get immediate value or not.

            //switches based on type
            switch (this.type)
            {
                case 0:
                    //data manipulation 00
                    if ((0x012FFF10 & command.ReadWord(0)) == 0x012FFF10)
                    {
                        Logger.Instance.writeLog("CMD: BX Instruction");
                        instruct = new Branch();
                        break;
                    }
                    instruct = new dataManipulation();
                    break;
                case 1:
                    //ldr/str 01
                    // check the PUBWL 
                    if (command.TestFlag(0, 24) || !command.TestFlag(0, 21))
                    {

                        instruct = new dataMovement();
                    }

                    break;
                case 2:
                    //10
                    if (!command.TestFlag(0, 25))
                    {
                        //load store multiple
                        instruct = new dataMoveMultiple();
                    }
                    else
                    {
                        if (command.TestFlag(0, 27) && !command.TestFlag(0, 26) && command.TestFlag(0, 25))
                        {
                            //branch command.
                            instruct = new Branch();
                        }
                    }
                    break;
                case 3:
                    //11
                    //Coprocessor
                    if (command.TestFlag(0, 26) && command.TestFlag(0, 25))
                    {
                        //software interupt.
                        Console.WriteLine("Quit");
                        Environment.Exit(0);
                    }
                    break;
                default:
                    break;
            }

            instruct.parse(command);
            instruct.cond = (uint)command.ReadByte(3) >> 4;
            instruct.type = (uint)((command.ReadByte(3) & 0x0c) >> 2);
            instruct.originalBits = (uint)command.ReadWord(0);
            instruct.rn = (uint)((command.ReadWord(0) & 0x000F0000) >> 16);
            instruct.rd = (uint)((command.ReadWord(0) & 0x0000F000) >> 12);
            return instruct; 

        }




    }

}

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
    //edit it!!!
    // A struct that mimics memory layout of ELF program file header
    // See http://www.sco.com/developers/gabi/latest/contents.html for details
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ELFPhdr
    {
        public int p_type;
        public int p_offset;
        public int p_vaddr;
        public int p_paddr;
        public int p_filesz;
        public int p_memsz;
        public int p_flags;
        public int p_align;
    } //Elf32_Phdr;


    // A struct that mimics memory layout of ELF file header
    // See http://www.sco.com/developers/gabi/latest/contents.html for details
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ELF
    {
        public byte EI_MAG0, EI_MAG1, EI_MAG2, EI_MAG3, EI_CLASS, EI_DATA, EI_VERSION;
        byte unused1, unused2, unused3, unused4, unused5, unused6, unused7, unused8, unused9;
        public ushort e_type;
        public ushort e_machine;
        public uint e_version;
        public uint e_entry;
        public uint e_phoff;
        public uint e_shoff;
        public uint e_flags;
        public ushort e_ehsize;
        public ushort e_phentsize;
        public ushort e_phnum;
        public ushort e_shentsize;
        public ushort e_shnum;
        public ushort e_shstrndx;
    }

    public class ELFReader
    {
        public ELF elfHeader;
        public ELFPhdr[] elfphs;

        //public ELF elfSection;


        public void ReadHeader(byte[] elfArray)
        {

            //	using (FileStream strm = new FileStream(elfFile, FileMode.Open))


            byte[] data = new byte[Marshal.SizeOf(elfHeader)];

            // Read ELF header into data
            Array.Copy(elfArray, data, data.Length);
            //strm.Read (data, 0, data.Length);
            // Convert to struct
            elfHeader = ByteArrayToStructure<ELF>(data);


            // seek to first program header entry
            int phEntry = (int)elfHeader.e_phoff;
            //strm.Seek (elfHeader.e_phoff, SeekOrigin.Begin);
            elfphs = new ELFPhdr[elfHeader.e_phnum];

            for (int i = 0; i < elfHeader.e_phnum; i++)
            {
                data = new byte[elfHeader.e_phentsize];
                Array.Copy(elfArray, phEntry, data, 0, elfHeader.e_phentsize);
                phEntry += elfHeader.e_phentsize;
                //strm.Read (data, 0, (int)elfHeader.e_phentsize);
                elfphs[i] = ByteArrayToStructure<ELFPhdr>(data);
            }//forloop

            // Now, do something with it ... see cppreadelf for a hint

        }




        // Converts a byte array to a struct
        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                                                typeof(T));
            handle.Free();
            return stuff;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace base64
{
    class Program
    {
        //  static Dictionary<int, char> table = new Dictionary<int, char>(); //dictionary is quite slow, using an array is better
        static char[] table2 = new char[64];
        static Dictionary<char, int> reverse = new Dictionary<char, int>();
        //populate the dictionary of index->char
        static Program()
        {
            for (int i = 65; i <= 90; i++) //A-Z
            {
                //    table.Add(i - 65, (char)i);
                table2[i - 65] = (char)i;
            }
            for (int i = 97; i <= 122; i++) //a-z
            {
                //  table.Add(i - 71, (char)i);
                table2[i - 71] = (char)i;
            }
            for (int i = 48; i <= 57; i++) //0-9
            {
                //  table.Add(i + 4, (char)i);
                table2[i + 4] = (char)i;
            }
            //  table.Add(62, '+');
            table2[62] = '+';
            //  table.Add(63, '/');
            table2[63] = '/';
            for (int i = 0; i < table2.Length; i++)
            {
                reverse.Add(table2[i], i);
            }
        }
        static void Main(string[] args)
        {
            StringToBase64("any carnal pleasure");
			FileToBase64("/home/alex/Pictures/Selection_013.png");
        }
        static void toStd()
        {

        }
		static void StringToBase64(string input) {
			Console.BufferHeight = 999;
			StringBuilder enc = new StringBuilder(input);
            int numtorep = 0;
            StringBuilder final = new StringBuilder();

            if (enc.Length % 3 == 1)
            {
            
                enc.Append((char)0);
                enc.Append((char)0);
                numtorep = 2;
            }
            else if (enc.Length % 3 == 2)
            {
                enc.Append((char)0);
                numtorep = 1;
            }
            byte[] encb = Encoding.ASCII.GetBytes(enc.ToString());
            byte[] bbfe = new byte[3];
            char[] buffer = new char[4];
            byte[] bb = new byte[4];
            for (int i = 0; i < encb.Length / 3; i++)
            {
                //Encoding.ASCII.GetBytes()
            //    byte[] b = Encoding.ASCII.GetBytes(enc.ToString(i * 3, 3));
                Array.Copy(encb, i * 3, bbfe, 0, 3);
                process(ref bbfe, ref bb);
                lookup(ref bb, ref buffer);

                final.Append(buffer);
                //Console.ReadKey();
            }
            if (numtorep > 0)
            {

                final.Remove(final.Length - numtorep, numtorep);
                if (numtorep == 1)
                {
                    final.Append("=");
                }
                else
                {
                    //StringBuilder sb = new StringBuilder();                    
                    final.Append("==");
                }
            }
            Console.WriteLine(final);
            Console.ReadKey();
		}
        static void FileToBase64(string file)
        {
            Console.BufferHeight = 999;
             byte[] all = File.ReadAllBytes(file);//@"C:\Users\Alex\Desktop\images\58197_1326510824.png");
            List<byte> enc = new List<byte>(all);
        //    Console.WriteLine(all.Length % 3);
            //StringBuilder enc = new StringBuilder("any carnal pleasure");
            int numtorep = 0;
           // byte[] test = { 19, 22, 5, 46 };
            //   decode(test);
            StringBuilder final = new StringBuilder();
            if (enc.Count% 3 == 1)
            {

                enc.Add(0);
                enc.Add(0);
                numtorep = 2;
            }
            else if (enc.Count % 3 == 2)
            {
                enc.Add(0);
                numtorep = 1;
            }
 /*           if (enc.Length % 3 == 1)
            {
            
                enc.Append((char)0);
                enc.Append((char)0);
                numtorep = 2;
            }
            else if (enc.Length % 3 == 2)
            {
                enc.Append((char)0);
                numtorep = 1;
            }*/
            byte[] encb = enc.ToArray(); //Encoding.ASCII.GetBytes(enc.ToString());
            byte[] bbfe = new byte[3];
            char[] buffer = new char[4];
            byte[] bb = new byte[4];
            for (int i = 0; i < encb.Length / 3; i++)
            {
                //Encoding.ASCII.GetBytes()
            //    byte[] b = Encoding.ASCII.GetBytes(enc.ToString(i * 3, 3));
                Array.Copy(encb, i * 3, bbfe, 0, 3);
                process(ref bbfe, ref bb);
                lookup(ref bb, ref buffer);

                final.Append(buffer);
                //Console.ReadKey();
            }
            if (numtorep > 0)
            {

                final.Remove(final.Length - numtorep, numtorep);
                if (numtorep == 1)
                {
                    final.Append("=");
                }
                else
                {
                    //StringBuilder sb = new StringBuilder();                    
                    final.Append("==");
                }
            }
            Console.WriteLine(final);
            Console.ReadKey();
        }
        /*   static printBinary(int i) {
               for(int j = 32; j > 0; j--) {
            
               }
           }*/
        static void lookup(ref byte[] b, ref char[] ret)
        {
            //   char[] ret = new char[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                ret[i] = table2[b[i]];
            }
            // return ret;
        }
        static byte[] decode(byte[] b)
        {
            if (b.Length != 4)
            {
                throw new Exception("Incorrect length.");
            }
            byte first = (byte)((b[0] << 2) | ((b[1] & 48) >> 4));
            Console.WriteLine(first);
            byte second = (byte)(((b[1] & 0xF) << 4) | ((b[2] & 0x3C) >> 2));
            Console.WriteLine(second);
            byte third = (byte)(((b[2] & 0x3) << 6) | b[3]);
            Console.WriteLine(third);
            byte[] ret = new byte[3];
            ret[0] = first;
            ret[1] = second;
            ret[2] = third;

            return ret;
        }
        static void process(ref byte[] b, ref byte[] ret)
        {
            if (b.Length != 3)
            {
                throw new Exception("Incorrect length.");
            }
            //    Console.WriteLine(b[0]);
            ret[0] = (byte)((b[0] & 252) >> 2);
            //  Console.WriteLine(first);
            ret[1] = (byte)(((b[0] & 3) << 4) | ((b[1] & 240) >> 4));
            //  Console.WriteLine(second);
            ret[2] = (byte)(((b[1] & 15) << 2) | ((b[2] & 192) >> 6));
            //   Console.WriteLine(third);
            ret[3] = (byte)(b[2] & 63);
            //  Console.WriteLine(fourth);            
            /*ret[0] = first;
            ret[1] = second;
            ret[2] = third;
            ret[3] = fourth;*/
            //byte last            
        }

    }
}

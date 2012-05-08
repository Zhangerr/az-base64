using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace base64
{
	class Program
	{
		static char[] table2 = new char[64];
		static Dictionary<char, byte> reverse = new Dictionary<char, byte>();
		
		//populate the dictionary of index->char
		static Program ()
		{
			for (int i = 65; i <= 90; i++) { //A-Z
				table2[i - 65] = (char)i;
			}
			for (int i = 97; i <= 122; i++) { //a-z
				table2[i - 71] = (char)i;
			}
			for (int i = 48; i <= 57; i++) { //0-9
				table2[i + 4] = (char)i;
			}
			table2[62] = '+';
			table2[63] = '/';
			for (int i = 0; i < table2.Length; i++) {
				reverse.Add(table2[i], (byte)i);
			}
		}

		static void Main(string[] args)
		{
			Base64ToString("YW55IGNhcm5hbCBwbGVhc3VyZQ==");				
			StringToBase64("any carnal pleasure");
			FileToBase64("/home/alex/Pictures/Selection_013.png");
		}

		static void assert(bool a)
		{
			assert(a, "");
		}

		static void assert(bool a, string reason)
		{
			if (!a) {
				throw new Exception(reason);
			}
		}

		static void Base64ToString(string b64)
		{
			assert(b64.Length % 4 == 0, "base 64 length incorrect");
			int count = b64.Split('=').Length - 1;
			
			StringBuilder sb = new StringBuilder(b64.Replace("=", "A"));
			Console.WriteLine("{0} {1} {2}", b64, sb.ToString(), count);
			char[] buffer = new char[4];
			byte[] bbuf = new byte[4];
			byte[] retbuf = new byte[3];
			List<byte> bytes = new List<byte>();
			for (int i = 0; i < sb.Length / 4; i++) {
				sb.CopyTo(i * 4, buffer, 0, 4);
				charToByte(ref buffer, ref bbuf);
				decode(ref bbuf, ref retbuf);
				bytes.AddRange(retbuf);
			}
			if (count > 0) {
				for (int i = 0; i < count; i++) {
					bytes.RemoveAt(bytes.Count - 1);
				}
			}
			Console.WriteLine(new string(Encoding.ASCII.GetChars(bytes.ToArray())));
		}

		static void charToByte(ref char[] i, ref byte[] j)
		{
			for (int foo = 0; foo < i.Length; foo++) {
				j[foo] = reverse[i[foo]];
			}
		}

		static void StringToBase64(string input)
		{
			
			StringBuilder enc = new StringBuilder(input);
			int numtorep = 0;
			StringBuilder final = new StringBuilder();
			int remainder = enc.Length % 3;
			if (remainder > 0) {
				numtorep = 3 - remainder;
				for (int i = 0; i < numtorep; i++) {
					enc.Append((char)0);
				}
			}		
			assert(enc.Length % 3 == 0, "not divisible by 3");
			
			/*   if (enc.Length % 3 == 1)
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
			byte[] encb = Encoding.ASCII.GetBytes(enc.ToString());
			byte[] bbfe = new byte[3];
			char[] buffer = new char[4];
			byte[] bb = new byte[4];
			for (int i = 0; i < encb.Length / 3; i++) {                
				Array.Copy(encb, i * 3, bbfe, 0, 3);
				process(ref bbfe, ref bb);
				lookup(ref bb, ref buffer);

				final.Append(buffer);
			}
			if (numtorep > 0) {

				final.Remove(final.Length - numtorep, numtorep);
				if (numtorep == 1) {
					final.Append("=");
				} else {                  
					final.Append("==");
				}
			}
			assert(final.Length % 4 == 0, "final string not divisible by 4");
			Console.WriteLine(final);
			Console.ReadKey();
		}

		static void FileToBase64(string file)
		{
			byte[] all = File.ReadAllBytes(file);
			List<byte> enc = new List<byte>(all);
			int numtorep = 0;
			StringBuilder final = new StringBuilder();
			int remainder = enc.Count % 3;
			if (remainder > 0) {
				numtorep = 3 - remainder;
				for (int i = 0; i < numtorep; i++) {
					enc.Add((byte)0);
				}
			}
			assert(enc.Count % 3 == 0, "not divisible by 3");
			/*  if (enc.Count% 3 == 1)
            {

                enc.Add(0);
                enc.Add(0);
                numtorep = 2;
            }
            else if (enc.Count % 3 == 2)
            {
                enc.Add(0);
                numtorep = 1;
            }*/
			byte[] encb = enc.ToArray();
			byte[] bbfe = new byte[3];
			char[] buffer = new char[4];
			byte[] bb = new byte[4];
			for (int i = 0; i < encb.Length / 3; i++) {
				Array.Copy(encb, i * 3, bbfe, 0, 3);
				process(ref bbfe, ref bb);
				lookup(ref bb, ref buffer);
				final.Append(buffer);
			}
			if (numtorep > 0) {

				final.Remove(final.Length - numtorep, numtorep);
				if (numtorep == 1) {
					final.Append("=");
				} else {                  
					final.Append("==");
				}
			}
			assert(final.Length % 4 == 0, "final string not divisible by 4");
			Console.WriteLine(final);
			Console.ReadKey();
		}

		static void lookup(ref byte[] b, ref char[] ret)
		{
			for (int i = 0; i < b.Length; i++) {
				ret[i] = table2[b[i]];
			}
		}

		static void decode(ref byte[] b, ref byte[] ret)
		{
			assert(b.Length == 4);
			ret[0] = (byte)((b[0] << 2) | ((b[1] & 48) >> 4));
			//	Console.WriteLine(first);
			ret[1] = (byte)(((b[1] & 0xF) << 4) | ((b[2] & 0x3C) >> 2));
			//	Console.WriteLine(second);
			ret[2] = (byte)(((b[2] & 0x3) << 6) | b[3]);
			//Console.WriteLine(third);
		}

		static void process(ref byte[] b, ref byte[] ret)
		{
			if (b.Length != 3) {
				throw new Exception("Incorrect length.");
			}
			
			ret[0] = (byte)((b[0] & 252) >> 2);
			ret[1] = (byte)(((b[0] & 3) << 4) | ((b[1] & 240) >> 4));
			ret[2] = (byte)(((b[1] & 15) << 2) | ((b[2] & 192) >> 6));
			ret[3] = (byte)(b[2] & 63); 
		}

	}
}

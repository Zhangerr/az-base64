using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace base64
{
	/// <summary>
	/// Base64 converter.
	/// </summary>
	/// <remarks>TODO: add comments regarding the bitmasks for decode/encoding</remarks>
	public class Base64Converter
	{
		static char[] table2 = new char[64];
		static Dictionary<char, byte> reverse = new Dictionary<char, byte>();
		
		//populate the dictionary of index->char
		static Base64Converter ()
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
			if (args.Length > 0) {
				Console.WriteLine(StringToBase64(args[0]));
			} else {
				Console.WriteLine("Please enter an argument");
			}
		}

		static void tests()
		{
			assert(StringToBase64("any carnal pleasure."), "YW55IGNhcm5hbCBwbGVhc3VyZS4=");
			assert(StringToBase64("any carnal pleasur"), "YW55IGNhcm5hbCBwbGVhc3Vy");
			assert(Base64ToString("YW55IGNhcm5hbCBwbGVhc3Vy"), "any carnal pleasur");
			//Console.WriteLine(StringToBase64("what you see is what you get"));
		}

		static void assert(bool a)
		{
			assert(a, "");
		}

		static void assert(string a, string b)
		{
			assert(a == b);
		}

		static void assert(bool a, string reason)
		{
			if (!a) {
				throw new Exception(reason);
			}
		}

		public static string Base64ToString(string b64)
		{
			return new string(Encoding.ASCII.GetChars(Base64ToBytes(b64)));
		}

		public static void Base64ToFile(string b64, string path)
		{
			File.WriteAllBytes(path, Base64ToBytes(b64));
		}

		public static byte[] Base64ToBytes(string b64)
		{			
			assert(b64.Length % 4 == 0, "base 64 length incorrect");
			int count = b64.Split('=').Length - 1;
			
			StringBuilder sb = new StringBuilder(b64.Replace("=", "A"));
			//	Console.WriteLine("{0} {1} {2}", b64, sb.ToString(), count);
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
			return bytes.ToArray();
		}
		
		public static string FileToBase64(string file)
		{
			return BytesToBase64(File.ReadAllBytes(file));
		}
		
		public static string StringToBase64(string input)
		{
			return StringToBase64(input, Encoding.ASCII);
		}

		public static string StringToBase64(string input, Encoding enco)
		{					
			return BytesToBase64(enco.GetBytes(input));
		}

		public static string BytesToBase64(byte[] ba)
		{
			byte[] all = ba;//File.ReadAllBytes(file);
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
			return final.ToString();
		}
		#region private methods
		private static void lookup(ref byte[] b, ref char[] ret)
		{
			for (int i = 0; i < b.Length; i++) {
				ret[i] = table2[b[i]];
			}
		}

		private static void decode(ref byte[] b, ref byte[] ret)
		{
			assert(b.Length == 4);
			ret[0] = (byte)((b[0] << 2) | ((b[1] & 0x30) >> 4));	
			ret[1] = (byte)(((b[1] & 0xF) << 4) | ((b[2] & 0x3C) >> 2));
			ret[2] = (byte)(((b[2] & 0x3) << 6) | b[3]);
		}

		private static void process(ref byte[] b, ref byte[] ret)
		{
			assert(b.Length == 3);			
			ret[0] = (byte)((b[0] & 0xFC) >> 2); //1111 1100 mask
			ret[1] = (byte)(((b[0] & 0x3) << 4) | ((b[1] & 0xF0) >> 4));
			ret[2] = (byte)(((b[1] & 0xF) << 2) | ((b[2] & 0xC0) >> 6));
			ret[3] = (byte)(b[2] & 0x3F); 
		}

		private static void charToByte(ref char[] i, ref byte[] j)
		{
			for (int foo = 0; foo < i.Length; foo++) {
				j[foo] = reverse[i[foo]];
			}
		}
		#endregion

	}
}

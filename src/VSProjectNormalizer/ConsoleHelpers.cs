using System;

namespace CodeTools.VisualStudio.Tools
{
	public static class ConsoleHelpers
	{
		public static void WriteLine(ConsoleColor color, string message)
		{
			ConsoleColor defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = defaultColor;
		}

		public static void WriteLine(string message)
		{
			Console.WriteLine(message);
		}

		public static void Write(ConsoleColor color, string message)
		{
			ConsoleColor defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ForegroundColor = defaultColor;
		}

		public static void Write(string message)
		{
			Console.Write(message);
		}		 
	}
}
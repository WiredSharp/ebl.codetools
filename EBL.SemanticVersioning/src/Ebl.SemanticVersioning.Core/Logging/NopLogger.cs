namespace SemanticVersioning.Logging
{
	public class NopLogger : ILogger
	{
		public static ILogger Logger = new NopLogger();

		private NopLogger() { }

		public void Debug(string message)
		{
		}

		public void Error(string message)
		{
		}

		public void Fatal(string message)
		{
		}

		public void Info(string message)
		{
		}

		public void Warning(string message)
		{
		}
	}
}

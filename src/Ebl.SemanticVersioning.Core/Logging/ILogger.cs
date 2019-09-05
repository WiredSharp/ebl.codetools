using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticVersioning.Logging
{
	public interface ILogger
	{
		void Debug(string message);

		void Info(string message);

		void Warning(string message);

		void Error(string message);

		void Fatal(string message);
	}

}

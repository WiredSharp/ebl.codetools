using SemanticVersioning.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticVersioning.Providers.Nuget.Logging
{
	internal class NugetLoggerAdapter: NuGet.Common.ILogger
	{
		private readonly ILogger _logger;

		public NugetLoggerAdapter()
			:this(NopLogger.Logger)
		{

		}

		public NugetLoggerAdapter(ILogger logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger), "logger cannot be null");
			_logger = logger;
		}

		public void LogDebug(string data)
		{
			_logger.Debug(data);
		}

		public void LogError(string data)
		{
			_logger.Error(data);
		}

		public void LogErrorSummary(string data)
		{
			_logger.Error(data);
		}

		public void LogInformation(string data)
		{
			_logger.Info(data);
		}

		public void LogInformationSummary(string data)
		{
			_logger.Info(data);
		}

		public void LogMinimal(string data)
		{
			_logger.Info(data);
		}

		public void LogVerbose(string data)
		{
			_logger.Debug(data);
		}

		public void LogWarning(string data)
		{
			_logger.Warning(data);
		}
	}
}

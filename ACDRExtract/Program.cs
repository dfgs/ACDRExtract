using CommandLine;
using LogLib;
using System;

namespace ACDRExtract
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
			{
				ILogger logger;
				if (options.Verbose) logger = new ConsoleLogger(new DefaultLogFormatter());
				else logger = NullLogger.Instance;

				/*Console.WriteLine($"Input file: {options.InputFilePath}");
				if (!string.IsNullOrWhiteSpace(options.OutputFilePath))
				{
					Console.WriteLine($"Output file: {options.OutputFilePath}");
				}*/
				string outputFilePath;

				if (options.InputFilePath == null) return;

				if (!string.IsNullOrEmpty(options.OutputFilePath)) outputFilePath = options.OutputFilePath;
				else outputFilePath = Path.Combine(Path.GetFullPath(options.InputFilePath), $"{Path.GetFileNameWithoutExtension(options.InputFilePath)} - extract.pcap");

				ExtractModule module = new ExtractModule(logger);
				module.Extract(options.InputFilePath, outputFilePath,options.Port);

			});
		}
	}
}
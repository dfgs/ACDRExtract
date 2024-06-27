using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDRExtract
{
	public class Options
	{
		[Option('v', "verbose", Required = false, HelpText = "Enable verbose output.")]
		public bool Verbose { get; set; }

		[Option('i', "input",  Required = true,  HelpText = "Input file path.")]
		public string? InputFilePath { get; set; }

		[Option('o', "output", Required = false, HelpText = "Output file path.")]
		public string? OutputFilePath { get; set; }
		
		[Option('p', "port", Required = false, Default =(ushort)925, HelpText = "ACDR port.")]
		public ushort Port { get; set; }
	}
}

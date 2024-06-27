using ACDRFrameReaderLib;
using LogLib;
using ModuleLib;
using PcapReaderLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDRExtract
{
	public class ExtractModule : Module
	{
		private ACDRModule acdrModule;
		public ExtractModule(ILogger Logger) : base(Logger)
		{
			this.acdrModule = new ACDRModule(Logger);
		}

		public void Extract(string InputFilePath,string OutputFilePath, ushort Port)
		{
			IPcapReader pcapReader;
			BinaryReader binaryReader;
			FileStream? stream=null;
			FileHeader? header=null;
			PacketRecord? packetRecord = null;
			ACDR? acdr;

			int counter=0;

			Log(LogLevels.Information, $"Extracting file {InputFilePath} to {OutputFilePath}");

			
			if (!Try(() => new FileStream(InputFilePath, FileMode.Open)).Then((result) => stream = result).OrAlert("Failed to open input file")) return;
			if (stream == null) return;

			binaryReader = new BinaryReader(stream);
			pcapReader = new PcapReader();

			do
			{
				if (!Try(() => pcapReader.ReadHeader(binaryReader)).Then((result) => header = result).OrAlert("Failed to read file header")) break;
				if (header == null) break;

				while(stream.Position<stream.Length)
				{
					
					if (!Try(() => pcapReader.ReadPacketRecord(binaryReader)).Then((result) => packetRecord = result).OrAlert("Failed to read packet record")) break;
					if (packetRecord == null) break;
					counter++;
					Log(LogLevels.Debug, $"Read packet record #{counter}");

					acdr = acdrModule.ReadACDR(packetRecord.PacketData,Port);
					if (acdr == null) continue;
					Log(LogLevels.Information, $"New ACDR {acdr.Value.FullSessionID}");
					
				}

			} while (false);

			Try(() => stream.Dispose()).OrAlert("Failed to dispose input stream");

		}

	}
}

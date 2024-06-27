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
		public ExtractModule(ILogger Logger) : base(Logger)
		{
		}

		public void Extract(string InputFilePath,string OutputFilePath, ushort Port)
		{
			ACDRModule acdrModule;

			PacketRecord? packetRecord = null;
			ACDR? acdr;

			int counter=0;

			Log(LogLevels.Information, $"Extracting file {InputFilePath} to {OutputFilePath}");

			using (PacketRecordReaderModule packetRecordReaderModule = new PacketRecordReaderModule(Logger, InputFilePath))
			{
				acdrModule = new ACDRModule(Logger);

				while (packetRecordReaderModule.CanRead)
				{
					packetRecord = packetRecordReaderModule.Read();
					if (packetRecord == null) break;
					counter++;
					Log(LogLevels.Debug, $"Read packet record #{counter}");

					acdr = acdrModule.ReadACDR(packetRecord.PacketData, Port);
					if (acdr == null) continue;
					Log(LogLevels.Information, $"New ACDR {acdr.Value.FullSessionID}");

					if (acdr.Value.Header.MediaType != MediaTypes.ACDR_SIP) continue;
					Log(LogLevels.Information, $"SIP message found");

				}
			}


			

		}

	}
}

using ACDRFrameReaderLib;
using LogLib;
using ModuleLib;
using PcapReaderLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDRExtract
{
	public class PacketRecordReaderModule : Module
	{
		private FileStream? stream = null;
		private BinaryReader? binaryReader=null;
		private IPcapReader pcapReader;

		public bool EOF
		{
			get => stream == null ? true: stream.Position >= stream.Length;
		}

		public PacketRecordReaderModule(ILogger Logger) : base(Logger)
		{
			pcapReader = new PcapReader();

		}

		public bool Open(string FileName)
		{
			FileHeader? header = null;

			if ((stream != null) || (binaryReader != null))
			{
				Log(LogLevels.Error, "File is already opened");
				return false;
			}

			if (!Try(() => new FileStream(FileName, FileMode.Open)).Then((result) => stream = result).OrAlert("Failed to open input file")) return false;
			if (stream == null) return false;

			binaryReader = new BinaryReader(stream);
			pcapReader = new PcapReader();

			if (!Try(() => pcapReader.ReadHeader(binaryReader)).Then((result) => header = result).OrAlert("Failed to read file header")) return false;
			if (header == null) return false;

			return true;
		}

		public bool Close()
		{
			if ((stream == null) || (binaryReader == null))
			{
				Log(LogLevels.Error, "File is not opened");
				return false;
			}

			stream.Dispose();
			stream = null;
			binaryReader = null;
			return true;
		}

		public PacketRecord? Read()
		{
			PacketRecord? packetRecord = null;

			if ((stream==null) || (binaryReader == null))
			{
				Log(LogLevels.Error, "File is not opened");
				return null;
			}

			if (EOF)
			{
				Log(LogLevels.Error, "End of stream reached");
				return null;
			}

			if (!Try(() => pcapReader.ReadPacketRecord(binaryReader)).Then((result) => packetRecord = result).OrAlert("Failed to read packet record")) return null;
			return packetRecord;

		}



	}
}

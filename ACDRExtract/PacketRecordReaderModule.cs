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
	public class PacketRecordReaderModule : Module,IDisposable
	{
		private FileStream? stream = null;
		private BinaryReader? binaryReader=null;
		private IPcapReader pcapReader;

		public bool CanRead
		{
			get
			{
				if ((pcapReader == null) || (stream == null) || (binaryReader == null)) return false;
				return stream.Position < stream.Length;
			}
		}

		public PacketRecordReaderModule(ILogger Logger, string FileName) : base(Logger)
		{
			pcapReader = new PcapReader();

			FileHeader? header = null;


			if (!Try(() => new FileStream(FileName, FileMode.Open)).Then((result) => stream = result).OrAlert("Failed to open input file")) return;
			if (stream == null) return;

			binaryReader = new BinaryReader(stream);
			pcapReader = new PcapReader();

			if (!Try(() => pcapReader.ReadHeader(binaryReader)).Then((result) => header = result).OrAlert("Failed to read file header")) return;
			if (header == null) return;
		}

		public override void Dispose()
		{
			if ((stream == null) || (binaryReader == null))
			{
				Log(LogLevels.Error, "File is not opened");
			}
			else
			{
				stream.Dispose();
				stream = null;
				binaryReader = null;
			}
			base.Dispose();
		}


		public PacketRecord? Read()
		{
			PacketRecord? packetRecord = null;

			if (!CanRead)
			{
				Log(LogLevels.Error, "Module not initialized successfully");
				return null;
			}

			if (!Try(() => pcapReader.ReadPacketRecord(binaryReader!)).Then((result) => packetRecord = result).OrAlert("Failed to read packet record")) return null;
			return packetRecord;

		}



	}
}

using ACDRFrameReaderLib;
using EthernetFrameReaderLib;
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
	public class ACDRModule : Module
	{
		private FrameReader frameReader;
		private PacketReader packetReader;
		private UDPSegmentReader udpSegmentReader;
		private ACDRReader acdrReader;

		public ACDRModule(ILogger Logger) : base(Logger)
		{
			this.frameReader = new FrameReader();
			this.packetReader = new PacketReader();
			this.udpSegmentReader = new UDPSegmentReader();
			this.acdrReader = new ACDRReader();
		}

		public ACDR? ReadACDR(byte[] Data, ushort Port)
		{
			Frame frame = new Frame();
			Packet packet = new Packet();
			UDPSegment segment = new UDPSegment();
			ACDR? acdr=null;

			if (!Try(() => frameReader.Read(Data)).Then((result) => frame = result).OrWarn("Failed to decode frame")) return null;
			
			if (!Try(() => packetReader.Read(frame.Payload)).Then((result) => packet= result).OrWarn("Failed to decode packet")) return null;

			if (packet.Header.Protocol != Protocols.UDP) return null;

			if (!Try(() => udpSegmentReader.Read(packet.Payload)).Then((result) => segment = result).OrWarn("Failed to decode segment")) return null;

			if (segment.Header.DestinationPort != Port) return null;

			if (!Try(() => acdrReader.Read(segment.Payload)).Then((result) => acdr= result).OrWarn("Failed to decode ACDR")) return null;

			return acdr;
		}


	}
}

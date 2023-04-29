using System.IO;

namespace WaveTest
{
    public class WaveParser
    {
        private BinaryReader _reader;
        
        public int FileSize { get; private set; }
        public int FormatSize { get; private set; }
        public short FormatType { get; private set; }
        public short Channels { get; private set; }
        public int SampleRate { get; private set; }
        public int ByteRate { get; private set; }
        public short BlockAlign { get; private set; }
        public short BitsPerSample { get; private set; }
        public int DataSize { get; private set; }
        public int Length { get; private set; }
        
        public WaveParser(BinaryReader reader)
        { 
            _reader = reader;
            
            // First off, we ensure this is a valid RIFF file
            var riff = new byte[4];
            reader.Read(riff, 0, 4);
            if (riff[0] != 'R' || riff[1] != 'I' || riff[2] != 'F' || riff[3] != 'F')
            {
                throw new InvalidDataException("Not a valid RIFF file");
            }
            
            // Next, we read the file's size
            FileSize = reader.ReadInt32();

            // Next, we ensure this is a valid WAVE file
            var wave = new byte[4];
            reader.Read(wave, 0, 4);
            if (wave[0] != 'W' || wave[1] != 'A' || wave[2] != 'V' || wave[3] != 'E')
            {
                throw new InvalidDataException("Not a valid WAVE file");
            }
            
            // Ensure fmt is next
            var fmt = new byte[4];
            reader.Read(fmt, 0, 4);
            if (fmt[0] != 'f' || fmt[1] != 'm' || fmt[2] != 't' || fmt[3] != ' ')
            {
                throw new InvalidDataException("Not a valid fmt chunk");
            }

            // More metadata about the samples themselves
            FormatSize = reader.ReadInt32();
            FormatType = reader.ReadInt16();
            Channels = reader.ReadInt16();
            SampleRate = reader.ReadInt32();
            ByteRate = reader.ReadInt32();
            BlockAlign = reader.ReadInt16();
            BitsPerSample = reader.ReadInt16();
            
            // Now we ensure "data" is next
            var data = new byte[4];
            reader.Read(data, 0, 4);
            if (data[0] != 'd' || data[1] != 'a' || data[2] != 't' || data[3] != 'a')
            {
                throw new InvalidDataException("Not a valid data chunk");
            }
            
            DataSize = reader.ReadInt32();

            Length = DataSize / ByteRate;
        }
        
        public byte[] ReadSamples(int count)
        {
            var samples = new byte[count];
            _reader.Read(samples, 0, count);
            return samples;
        }
    }
}
using System;
using System.IO;
using System.Linq;

namespace WaveTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Open the wav declared in our args
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: WaveTest.exe <wav file> <output file>");
                return;
            }
            
            byte[] file = null;
            try
            {
                file = File.ReadAllBytes(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not open file");
            }
            if (file == null || file.Length < 44)
            {
                Console.WriteLine("File is not a valid wav file.");
                return;
            }

            var parser = new WaveParser(new BinaryReader(new MemoryStream(file)));
            
            // Now we need to find the average volume of every quarter second of audio
            float[] averageVolume = new float[parser.Length*8];
            for (int segment = 0; segment < parser.Length*8; segment++) {
                // For every quarter chunk, read byte by byte and add to the average volume for this quarter
                int quarterLength = parser.ByteRate / 8;
                byte[] quarterBytes = parser.ReadSamples(quarterLength);
                float quarterVolume = quarterBytes.Sum(x => (float) x);
                averageVolume[segment] = quarterVolume / (parser.ByteRate/8);
            }

            // Write the average volume to a file for python to plot
            File.WriteAllLines(args[1], Array.ConvertAll(averageVolume, x => x.ToString()));
        }
    }
}
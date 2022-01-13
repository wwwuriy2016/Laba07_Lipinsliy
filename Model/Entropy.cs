using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEDDES.Model
{
    public static class Entropy
    {
        public static double CalculateEntropy(string fileName)
        {
            int range = byte.MaxValue + 1; 
            byte[] values = File.ReadAllBytes(fileName);

            long[] counts = new long[range];
            foreach (byte value in values)
            {
                counts[value]++;
            }

            double entropy = 0;
            foreach (long count in counts)
            {
                if (count != 0)
                {
                    double probability = (double)count / values.LongLength;
                    entropy -= probability * Math.Log(probability, 2.0);
                }
            }
            return entropy;
        }
    }
}

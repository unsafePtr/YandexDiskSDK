using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.StaticClasses
{
    /// <summary>
    /// Represents memory size in bytes
    /// </summary>
    public enum MemorySizes { KB = 1024, MB = 1042441, GB = 1067459584 }

    public static class MemorySizeParser
    {
        public static double ConvertFromBytesToGb(long sizeInBytes)
        {
            return ConvertFromBytesToGb(sizeInBytes, 2);
        }

        public static double ConvertFromBytesToGb(long sizeInBytes, int precision)
        {
            double devide = sizeInBytes / (double)MemorySizes.GB;
            return Math.Round(devide, precision);
        }
    }
}

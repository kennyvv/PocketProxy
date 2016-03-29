using System;

namespace PocketProxy.PE
{
    internal static class Extensions
    {
        public static long NextLong(this Random random)
        {
            byte[] longBytes = new byte[8];
            random.NextBytes(longBytes);
            return BitConverter.ToInt64(longBytes, 0);
        }
    }
}

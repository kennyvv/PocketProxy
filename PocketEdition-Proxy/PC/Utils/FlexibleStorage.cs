using System;
using System.Linq;

namespace PocketProxy.PC.Utils
{
    public class FlexibleStorage
    {
        private long[] Data { get; }
        private int BitsPerEntry { get; }
        private int Size { get; }
        private long MaxEntryValue { get; }

        public FlexibleStorage(int bitsPerEntry, int size)
            : this(bitsPerEntry, new long[RoundToNearest(size * bitsPerEntry, 64) / 64])
        {
        }

        public FlexibleStorage(int bitsPerEntry, long[] data)
        {
            if (bitsPerEntry < 1 || bitsPerEntry > 32)
            {
                throw new ArgumentException("BitsPerEntry cannot be outside of accepted range.");
            }

            BitsPerEntry = bitsPerEntry;
            Data = data;

            Size = Data.Length * 64 / BitsPerEntry;
            MaxEntryValue = (1L << BitsPerEntry) - 1;
        }

        public long[] GetData()
        {
            return Data.ToArray();
        }

        public int GetBitsPerEntry()
        {
            return BitsPerEntry;
        }

        public int GetSize()
        {
            return Size;
        }

        public int Get(int index)
        {
            if (index < 0 || index > Size - 1)
            {
                throw new IndexOutOfRangeException();
            }

            int bitIndex = index * BitsPerEntry;
            int startIndex = bitIndex / 64;
            int endIndex = index * BitsPerEntry / 64;
            int startBitSubIndex = bitIndex % 64;
            if (startIndex == endIndex)
            {
                return (int)(Data[startIndex] >> startBitSubIndex & MaxEntryValue);
            }

            int endBitSubIndex = 64 - startBitSubIndex;
            return
                (int)
                    ((Data[startIndex] >> startBitSubIndex | Data[endIndex] << endBitSubIndex) &
                     MaxEntryValue);
        }

        public void Set(int index, int value)
        {
            if (index < 0 || index > Size - 1)
            {
                throw new IndexOutOfRangeException();
            }

            if (value < 0 || value > MaxEntryValue)
            {
                throw new IndexOutOfRangeException();
            }

            int bitIndex = index * BitsPerEntry;
            int startIndex = bitIndex / 64;
            int endIndex = index * BitsPerEntry / 64;
            int startBitSubIndex = bitIndex % 64;
            Data[startIndex] = Data[startIndex] & ~(MaxEntryValue << startBitSubIndex) |
                               (value & MaxEntryValue) << startBitSubIndex;
            if (startIndex != endIndex)
            {
                int endBitSubIndex = 64 - startBitSubIndex;
                Data[endIndex] = Data[endIndex] >> endBitSubIndex << endBitSubIndex |
                                 (value & MaxEntryValue) >> endBitSubIndex;
            }
        }

        private static int RoundToNearest(int value, int roundTo)
        {
            if (roundTo == 0)
            {
                return 0;
            }
            if (value == 0)
            {
                return roundTo;
            }
            if (value < 0)
            {
                roundTo *= -1;
            }

            int remainder = value % roundTo;
            return remainder != 0 ? value + roundTo - remainder : value;
        }
    }
}

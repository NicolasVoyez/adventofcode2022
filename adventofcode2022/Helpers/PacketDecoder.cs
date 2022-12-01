using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Helpers
{
    public enum PacketType
    {
        Literal,
        Sum,
        Product,
        Min,
        Max,
        Gt,
        Lt,
        Eq,
    }

    public interface IPacket
    {
        PacketType PacketType { get; }
        int Version { get; }

        BigInteger GetValue();
    }
    public class LiteralPacket : IPacket
    {
        public LiteralPacket(int version, BigInteger value)
        {
            Version = version;
            Value = value;
        }

        public PacketType PacketType => PacketType.Literal;

        public int Version { get; }
        public BigInteger Value { get; }

        public BigInteger GetValue()
        {
            return Value;
        }
    }

    public class OperatorPacket : IPacket
    {
        public OperatorPacket(int version, PacketType type, IList<IPacket> subPackets)
        {
            Version = version;
            SubPackets = subPackets;
            PacketType = type;
        }

        public PacketType PacketType { get; }
        public int Version { get; }
        public IList<IPacket> SubPackets { get; }

        public BigInteger GetValue()
        {
            switch (PacketType)
            {
                case PacketType.Literal:
                    throw new ArgumentException("A literal is not an operator");
                case PacketType.Sum:
                    return SubPackets.Aggregate(new BigInteger(0), (product, packet) => product + packet.GetValue());
                case PacketType.Product:
                    return SubPackets.Aggregate(new BigInteger(1), (product, packet) => product * packet.GetValue());
                case PacketType.Min:
                    return SubPackets.Min(p => p.GetValue());
                case PacketType.Max:
                    return SubPackets.Max(p => p.GetValue());
                case PacketType.Gt:
                    return SubPackets[0].GetValue() > SubPackets[1].GetValue() ? 1 : 0;
                case PacketType.Lt:
                    return SubPackets[0].GetValue() < SubPackets[1].GetValue() ? 1 : 0;
                case PacketType.Eq:
                    return SubPackets[0].GetValue() == SubPackets[1].GetValue() ? 1 : 0;
            }
            throw new ArgumentException("Unknown operator");
        }
    }
    public static class PacketDecoder
    {
        private static Dictionary<char, string> HexToBinary = new Dictionary<char, string>
        {
            {'0', "0000"},
            {'1', "0001"},
            {'2', "0010"},
            {'3', "0011"},
            {'4', "0100"},
            {'5', "0101"},
            {'6', "0110"},
            {'7', "0111"},
            {'8', "1000"},
            {'9', "1001"},
            {'A', "1010"},
            {'B', "1011"},
            {'C', "1100"},
            {'D', "1101"},
            {'E', "1110"},
            {'F', "1111"},
        };


        public static IEnumerable<IPacket> Decode(string hexaText)
        {
            string binStr = hexaText.Aggregate("", (text, c) => text + HexToBinary[c]);

            int idx = 0;
            //while (idx < binStr.Length)
            //{
            var (packet, newIdx) = GetPacket(binStr, idx);
            yield return packet;
            idx = newIdx;
            idx++;
            //}
        }

        private static (IPacket, int) GetPacket(string binStr, int idx, bool padWithZeros = true)
        {
            var (version, type) = GetVersionAndType(binStr, idx);

            if (type == PacketType.Literal)
            {
                return GetLiteralPacketContent(binStr, version, idx, padWithZeros);
            }
            else
            {
                return GetOperatorPacketContent(binStr, version, type, idx, false);
            }
        }
        private static (OperatorPacket, int) GetOperatorPacketContent(string binStr, int version, PacketType type, int startIdx, bool paddedWithZeros)
        {
            var idx = startIdx + 6;
            var captureBits = binStr[idx++] == '0';
            if (captureBits)
            {
                var subPacketLength = BinaryCharsToInt(binStr, idx, 15);
                idx += 15;
                var endIdx = startIdx + 6 + 1 + 15 + subPacketLength;
                List<IPacket> subPackets = new List<IPacket>();
                while (idx < endIdx)
                {
                    var (p, nexIdx) = GetPacket(binStr, idx, false);
                    subPackets.Add(p);
                    idx = nexIdx;
                }
                return (new OperatorPacket(version, type, subPackets), GetEndIndex(startIdx, endIdx, 4, paddedWithZeros));
            }
            else
            {
                var subPacketCount = BinaryCharsToInt(binStr, idx, 11);
                idx += 11;

                List<IPacket> subPackets = new List<IPacket>();
                for (int i = 0; i < subPacketCount; i++)
                {
                    var (p, nexIdx) = GetPacket(binStr, idx, false);
                    subPackets.Add(p);
                    idx = nexIdx;
                }
                return (new OperatorPacket(version, type, subPackets), GetEndIndex(startIdx, idx, 4, paddedWithZeros));
            }
        }

        private static int GetEndIndex(int startIdx, int endIdx, int bitSize, bool paddedWithZeros)
        {
            if (!paddedWithZeros)
                return endIdx;

            var mod = (endIdx - startIdx) % bitSize;
            if (mod == 0)
                return endIdx;
            return endIdx + bitSize - mod;
        }

        private static (int, PacketType) GetVersionAndType(string binStr, int idx)
        {
            var version = BinaryCharsToInt(binStr[idx], binStr[idx + 1], binStr[idx + 2]);
            var type = BinaryCharsToPacketType(binStr[idx + 3], binStr[idx + 4], binStr[idx + 5]);
            return (version, type);
        }


        private static (LiteralPacket, int) GetLiteralPacketContent(string binStr, int version, int startIdx, bool paddedWithZeros)
        {
            var idx = startIdx + 6;
            bool lastPacket = false;
            List<char> num = new List<char>();
            while (!lastPacket)
            {
                lastPacket = binStr[idx++] == '0';
                for (int i = 0; i < 4; i++)
                    num.Add(binStr[idx++]);
            }

            var packet = new LiteralPacket(version, BinaryCharsToBigInt(num.ToArray()));

            return (packet, GetEndIndex(startIdx, idx, 4, paddedWithZeros));
        }

        private static PacketType BinaryCharsToPacketType(params char[] values)
        {
            var val = BinaryCharsToInt(values);
            switch (val)
            {
                case 4:
                    return PacketType.Literal;
                case 0:
                    return PacketType.Sum;
                case 1:
                    return PacketType.Product;
                case 2:
                    return PacketType.Min;
                case 3:
                    return PacketType.Max;
                case 5:
                    return PacketType.Gt;
                case 6:
                    return PacketType.Lt;
                case 7:
                    return PacketType.Eq;
            }
            throw new ArgumentException($"Unknown packet type {val}");
        }


        private static int BinaryCharsToInt(string values, int startValue, int length)
        {
            var times = 1;
            int result = 0;
            for (int idx = startValue + length - 1; idx >= startValue; idx--)
            {
                if (values[idx] == '1')
                    result += times;
                times *= 2;
            }
            return result;
        }
        private static BigInteger BinaryCharsToBigInt(params char[] values)
        {
            BigInteger times = 1;
            BigInteger result = 0;
            for (int idx = values.Length - 1; idx >= 0; idx--)
            {
                if (values[idx] == '1')
                    result += times;
                times *= 2;
            }
            return result;
        }
        private static int BinaryCharsToInt(params char[] values)
        {
            var times = 1;
            int result = 0;
            for (int idx = values.Length - 1; idx >= 0; idx--)
            {
                if (values[idx] == '1')
                    result += times;
                times *= 2;
            }
            return result;
        }
    }
}
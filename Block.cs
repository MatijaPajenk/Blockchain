using System.Text;

namespace Blockchain {
    internal class Block(int index, DateTime timestamp, byte[] data, byte[] hash,
                            byte[] previousHash, uint difficulty, uint nonce, string miner) {
        public int Index { get; set; } = index;
        public DateTime Timestamp { get; set; } = timestamp;
        public byte[] Data { get; set; } = data;
        public byte[] Hash { get; set; } = hash;
        public byte[] PreviousHash { get; set; } = previousHash;
        public uint Difficulty { get; set; } = difficulty;
        public uint Nonce { get; set; } = nonce;
        public string Miner { get; set; } = miner;

        public string GetData() {
            return Encoding.UTF8.GetString(Data);
        }

        public byte[] BlockToHashData() {
            return Encoding.UTF8.GetBytes($"{Index}{Timestamp}{Data}{PreviousHash}{Difficulty}{Nonce}");
        }

        public override string ToString() {
            var res =
                $"""
                Index: {Index}
                Data: {Encoding.UTF8.GetString(Data)}
                Timestamp: {Timestamp}
                Previous hash: {Utils.GetHexString(PreviousHash)}
                Difficulty: {Difficulty}
                Nonce: {Nonce}
                Miner: {Miner}
                Hash: {Utils.GetHexString(Hash)}
                """;
            return res;
        }

        public byte[] ToTransferByte() {
            return Encoding.UTF8.GetBytes($"{Index}|{GetData()}|{Timestamp}|{Utils.GetHexString(PreviousHash)}|{Difficulty}|{Nonce}|{Miner}|{Utils.GetHexString(Hash)}");
        }
    }
}

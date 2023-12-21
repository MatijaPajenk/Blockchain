namespace Blockchain {
    internal class Block(uint index, DateTime timestamp, byte[] data, byte[] hash,
                            byte[] previousHash, uint difficulty, uint nonce) {
        public uint Index { get; set; } = index;
        public DateTime Timestamp { get; set; } = timestamp;
        public byte[] Data { get; set; } = data;
        public byte[] Hash { get; set; } = hash;
        public byte[] PreviousHash { get; set; } = previousHash;
        public uint Difficulty { get; set; } = difficulty;
        public uint Nonce { get; set; } = nonce;

        public void Validate() {
            //TODO implement
        }
    }
}

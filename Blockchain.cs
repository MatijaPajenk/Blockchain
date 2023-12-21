namespace Blockchain {
    internal class Blockchain(string name) {
        public string Name { get; set; } = name;
        public uint Difficulty { get; set; }
        public List<Block> Blocks { get; set; } = [];

        public void CalculateCumulativeDifficulty() {
            foreach(var block in Blocks) {
                Difficulty += (uint)Math.Pow(2, block.Difficulty);
            }
        }
    }
}

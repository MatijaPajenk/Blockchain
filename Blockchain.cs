namespace Blockchain {
    internal class Blockchain(string name, List<Block>? blocks) {
        public string Name { get; set; } = name;
        public uint Difficulty { get; set; }
        public List<Block> Blocks { get; set; } = blocks ?? [];

        public void CalculateCumulativeDifficulty() {
            foreach(var block in Blocks) {
                Difficulty += (uint) Math.Pow(2, block.Difficulty);
            }
        }
    }
}

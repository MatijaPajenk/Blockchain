namespace Blockchain {
    internal class Blockchain(string name = "") {
        public string Name { get; set; } = name;
        public uint Difficulty { get; set; }
        public List<Block> Blocks { get; set; } = [];

        public void AddBlock(Block block) {
            if(Blocks.Count > 0 && block.Index < Blocks.Last().Index)
                block.Index = Blocks.Last().Index + 1;
            Blocks.Add(block);
        }

        public void CalculateCumulativeDifficulty() {
            Difficulty = 0;
            foreach(var block in Blocks) {
                Difficulty += (uint)Math.Pow(2, block.Difficulty);
            }
        }

        public void ValidateChain() {
            for(int i = 1; i < Blocks.Count; i++) {
                if(Blocks[i].PreviousHash != Blocks[i - 1].Hash) {
                    Blocks[i].PreviousHash = Blocks[i - 1].Hash;
                }
            }
        }
    }
}

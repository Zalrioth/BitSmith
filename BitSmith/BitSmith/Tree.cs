using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitSmith
{
    class Tree
    {
        public Tree(int x, int y)
        {
            for (int zy = 0; zy <= 2; zy++)
            {
                Game1.getTerrain()[x, y - zy].setBlockType(Block.WOOD_BLOCK);
            }
            for (int zx = 0; zx < 3; zx++)
            {
                Game1.getTerrain()[x + zx - 1, y - 3].setBlockType(Block.BRUSH_BLOCK);
            }
            for (int zx = 0; zx < 5; zx++)
            {
                Game1.getTerrain()[x + zx - 2, y - 4].setBlockType(Block.BRUSH_BLOCK);
            }
            for (int zx = 0; zx < 3; zx++)
            {
                Game1.getTerrain()[x + zx - 1, y - 5].setBlockType(Block.BRUSH_BLOCK);
            }
        }
        public Tree(int x, int y, int height, int iterations)
        {
        }
    }
}
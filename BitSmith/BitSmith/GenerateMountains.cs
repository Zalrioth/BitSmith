using System.Collections.Generic;
using System;

namespace BitSmith
{
    public class GenerateMountains
    {
        public GenerateMountains(int x, int y, int height, int iterations)
        {
            var ys = new List<double>(new double[] { 0.0, 0.0 });
            double displacement = 1.0;
            Random random = new Random();

            for (int i = 0; i < 8; i++)
                ys = Split(ys, displacement *= 0.5, random);
            ComposeCoordinatePairs(ys, x, y);
        }
        public List<double> Split(List<double> ys, double displacement, Random random)
        {
            if (ys.Count < 2)
                throw new ArgumentException(">= 2 coordinates required");
            var r = new List<double>();
            for (int i = 0; i < ys.Count - 1; i++)
            {
                double dy = (ys[i + 1] - ys[i]) / 2.0;
                double d = random.NextDouble() * displacement;
                r.Add(ys[i]);
                r.Add(ys[i] + dy + d);
            }
            r.Add(ys[ys.Count - 1]);
            return r;
        }
        public void ComposeCoordinatePairs(List<double> ys, int x, int y)
        {
            double dx = 1.0 / (ys.Count - 1);
            for (int i = 0; i < ys.Count; i++)
            {
                //Console.WriteLine("{0:0.000} {1:0.000}", i * dx, ys[i]);
                //Console.WriteLine(""+x + ((int)(dx * 100)));
                for (int zy = 0; zy < ((int)(ys[i] * 100)); zy++)
                {
                    if (zy + 3 > ((int)(ys[i] * 100)))
                        Game1.getTerrain()[x + ((int)((i * dx) * 100)), y - zy].setBlockType(Block.DIRT_BLOCK);
                    else
                        Game1.getTerrain()[x + ((int)((i * dx) * 100)), y - zy].setBlockType(Block.STONE_BLOCK);
                }
                //Game1.getTerrain()[x + ((int)((i*dx) * 100)), y - ((int)(ys[i] * 100))].setBlockType(Block.GRASS_BLOCK);
            }
        }
    }
}
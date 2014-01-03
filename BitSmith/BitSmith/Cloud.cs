using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BitSmith
{
    class Cloud
    {
        private static Random random;

        private Vector2 cloudPos;
        private int cX;
        private int cY;
        private Color cC;
        private int[] shape;

        public Cloud()
        {
            generateCloud();
        }
        public Cloud(Vector2 _cloudPos)
        {
            cloudPos = _cloudPos;
            generateCloud();
        }
        public Cloud(int _cX, int _cY, Vector2 _cloudPos, Color _cC)
        {
            cX = _cX;
            cY = _cY;
            cC = _cC;
            cloudPos = _cloudPos;
            generateCloud();
        }        
        public void update()
        {
            cloudPos.X -= 0.02f;
        }
        public void draw(Color color)
        {
            for (int x = 0; x < cX; x++)
            {
                Game1.DrawRectangle(new Rectangle((int)cloudPos.X + (16 * x), (int)cloudPos.Y - (shape[x] * 16) / 2, 16, shape[x] * 16), color);
            }
        }
        public void setCoudPos(Vector2 _cloudPos)
        {
            cloudPos = _cloudPos;
        }
        public Vector2 getCoudPos()
        {
            return cloudPos;
        }
        private void generateCloud()
        {
            if (random == null)
                random = new Random();
            cX = random.Next(8, 15);
            shape = new int[cX];
            for (int x = 0; x < cX; x++)
            {
                cY = random.Next(3, 7);
                if (x == 0)
                {
                    shape[x] = cY/2;
                }
                else if (x == cX-1)
                {
                    shape[x] = cY / 2;
                }
                else
                {
                    shape[x] = cY;
                }
            }            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

namespace BitSmith
{
    public class Block
    {
        public static byte NO_BLOCK = 0;
        public static byte STONE_BLOCK = 1;
        public static byte DIRT_BLOCK = 2;
        public static byte IRON_BLOCK = 3;
        public static byte GOLD_BLOCK = 4;
        public static byte CLOUD_BLOCK = 10;
        public static byte GRASS_BLOCK = 11;
        public static byte WOOD_BLOCK = 12;
        public static byte BRUSH_BLOCK = 13;
        //public static byte GRASS_BLOCK = 3;

        private byte blockType;
        //private byte shadow;
        private byte variation;

        private Body body = null;
        private bool delete = false;

        public Block()
        {
        }
        public void setBlockType(byte _blockType)
        {
            blockType = _blockType;
        }
        public byte getBlockType()
        {
            return blockType;
        }
        public void setVariation(byte _variation)
        {
            variation = _variation;
        }
        public byte getVariation()
        {
            return variation;
        }
        public Body getBlockBlody()
        {
            return body;
        }
        public void createBlock(Vector2 _position)
        {
            //body = BodyFactory.CreateBody(Game1.world, _position + new Vector2(8/Game1.scale, 8/Game1.scale));
            body = BodyFactory.CreateBody(Game1.world, _position + new Vector2(8.05f / Game1.scale, 8.05f / Game1.scale));
            //body = BodyFactory.CreateBody(Game1.world, _position + new Vector2(16f / Game1.scale, 16f / Game1.scale));
            PolygonShape shape = new PolygonShape(1f);
            //shape.SetAsBox(8/Game1.scale, 8/Game1.scale);
            shape.SetAsBox(8.05f / Game1.scale, 8.05f / Game1.scale);
            //shape.SetAsBox(16f / Game1.scale, 16f / Game1.scale);
            body.CreateFixture(shape);
        }
        public void deleteBlock()
        {
            Game1.world.RemoveBody(body);
            body = null;
            delete = false;
        }
        public void setDelete(bool _delete)
        {
            delete = _delete;
        }
        public bool getDelete()
        {
            return delete;
        }
    }    
}

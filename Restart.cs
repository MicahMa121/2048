using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2048
{
        internal class Restart
    {
        private Texture2D _texture;
        private Rectangle _position;
        public Restart(Texture2D texture, Rectangle position)
        {
            _texture = texture;
            _position = position; 
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position,Color.White);
        }
        public Rectangle Rectangle { get { return _position; } }
    }
}

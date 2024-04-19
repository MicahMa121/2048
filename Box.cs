using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2048
{
    internal class Box
{
        private Texture2D _texture;
        private Rectangle _position;
        private int _value;
        private Color _color,_txtColor;
        private SpriteFont _font;
        bool _combined;
        private Rectangle _rectangle;
        private Vector2 _center;
        private int _grow;
        
        public Box(Texture2D texture, Rectangle position, int value, Color color,SpriteFont font, string state)
        {
            _texture = texture;
            _position = position;
            _value = value;
            _color = color;
            Value = value;
            _font = font;
            _combined = false;
            if (state == "grow")
            {
                _grow = 1;
            }
            else
            {
                _grow = 45;
            }
            _center = new Vector2(_position.X + _position.Width/2, _position.Y + _position.Height/2);
            _rectangle = new Rectangle((int)_center.X - _grow,(int)_center.Y -_grow, _grow * 2, _grow * 2);
            if (Value>= 8192)
            {
                _txtColor = Color.Blue;
            }
            else 
                _txtColor = Color.Black;
        }
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public Rectangle Rectangle 
        { 
            get { return _position; } 
            set { _position = value; }
        }
        public bool Combined
        {
            get { return _combined; }
            set { _combined = value; }  
        }
        public void Update()
        {
            if (Value >= 8192)
            {
                _txtColor = Color.Blue;
            }
            else
                _txtColor = Color.Black;
            if (_grow < 45)
            {
                _grow += 2;
                _rectangle = new Rectangle((int)_center.X - _grow, (int)_center.Y - _grow, _grow * 2, _grow * 2);
            }
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            string txt = _value.ToString();
            Vector2 txtPosition = new Vector2(_center.X-_font.MeasureString(txt).X/2, _center.Y- _font.MeasureString(txt).Y/2);
            
            spriteBatch.Draw(_texture, _rectangle, _color);
            
            spriteBatch.DrawString(_font, txt, txtPosition ,_txtColor);
        }
    }
}

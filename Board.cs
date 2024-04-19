using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Buffers.Binary;

namespace _2048
{
    internal class Board
{
        private Texture2D _texture;
        private Random _gen = new Random();
        private Rectangle _border;
        private Box[,] _board = new Box[4,4];
        private int _level;
        private SpriteFont _font;
        private int _score,_highscore;
        private Texture2D _restart;
        private Restart _restartButton;
        private int _bomb;
        private bool _bombActive;
        private bool _upActive;
        private int _spawn;
        private int _upgrades;
        private int _levelScore;
        public Texture2D Restart
        {
            get { return _restart; }
            set { _restart = value; }
        }

        public SoundEffect _pop;
        public SoundEffect _bub;
        public SoundEffect _block1;
        public SoundEffect _block2;

        private Rectangle[,] _rectangles = new Rectangle[4,4];
        public Board(Texture2D texture, Rectangle border, SpriteFont font)
        {
            _texture = texture;
            _border = border;
            _font = font;
        }
        public int Score { get { return _score; }set { _score = value; } }
        public int HighScore { get { return _highscore; } set { _highscore = value; } }
        public bool BombActive { get { return _bombActive; } set { _bombActive = value; } }
        public bool UpActive { get { return _upActive; } set { _upActive = value; } }
        public int Bomb { get { return _bomb; } set { _bomb = value; } }
        public int Upgrades { get { return _upgrades; } set { _upgrades = value;} }
        public int Spawn { get { return _spawn; } set { _spawn = value; } }
        public void NewBoard()
        {
            _levelScore = 3;
            _spawn = 0;
            _level = 7;
            _score = 0;
            _bomb = 0;
            _upgrades = 0;
            for (int i =  0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    NewEmpty(i, j);
                }
            }
        }
        private void NewEmpty(int i, int j)
        {
            Rectangle rectEmpty = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
            _rectangles[i,j] = rectEmpty;
            Box box = new Box(_texture, rectEmpty, 0, Color.Gray,_font, "");
            _board[i, j] = box;
        }
        private void NewBoxAtRandom()
        {
            bool end = true;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_board[i,j].Value == 0)
                    {
                        end = false;
                    }
                }
            }
            if (end)
            {
                if (Bomb == 0)
                {
                    _block2.Play();
                    Rectangle position = new Rectangle(_border.X + _border.Width / 2 - _border.Width / 4, _border.Y + _border.Height / 2 - _border.Height / 4, _border.Width / 2, _border.Height / 2);
                    _restartButton = new Restart(_restart, position);
                    return;
                }
                else
                    return;

            }
            int value = (int)Math.Pow(2, (double)1 + _gen.Next(_spawn, _spawn+3));
            bool done = false;
            while (!done)
            {
                int i = _gen.Next(0, 4);
                int j = _gen.Next(0, 4);
                if (_board[i,j].Value == 0)
                {
                    Rectangle rectFilled = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                    Color color = RefreshColor(value);
                    Box box = new Box(_texture, rectFilled, value, color,_font,"grow");
                    _board[i, j] = box;
                    _pop.Play();
                    done = true; break;
                }
            }
        }
        private Color RefreshColor(int value)
        {
            if (value != 0)
            {
                if (value ==2)
                {
                    return Color.White;
                }
                else if (value == 4)
                {
                    return new Color(255, 255, 100);
                }
                else if (value == 8)
                {
                    return Color.Yellow;
                }
                else if (value == 16)
                {
                    return Color.Orange;
                }
                else if (value == 32)
                {
                    return Color.OrangeRed;
                }
                else if (value == 64)
                {
                    return Color.Red;
                }
                else if (value == 128)
                {
                    return Color.MediumPurple;
                }
                else if (value == 256)
                {
                    return Color.Purple;
                }
                else if (value == 512)
                {
                    return Color.BlueViolet;
                }
                else if (value == 1024)
                {
                    return Color.CornflowerBlue;
                }
                else if (value == 2048)
                {
                    return Color.LightBlue;
                }
                else if (value == 4098)
                {
                    return Color.Gold;
                }
                else 
                {
                    return Color.Black;
                }
            }
            else
                return Color.Gray;
        }
        public void Up()
        {
            
            for (int i = 0; i < 4; i++)
            {
                for (int j =  0; j < 4; j++)
                {
                    _board[i, j].Combined = false;
                }
            }
            for (int i =0; i<4 ; i++) 
            {
                bool done;
                do
                {
                    done = true;
                    for (int j =0; j<4; j++)
                    {
                        if (j !=3 &&_board[i, j].Value == 0)
                        {
                                _board[i, j].Value = _board[i, j + 1].Value;
                                _board[i, j].Color = RefreshColor(_board[i, j].Value);
                                if (_board[i, j].Value != 0)
                                    _board[i, j].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(i, j + 1);
                            
                        }
                        if (j != 0 &&_board[i, j].Value != 0 && _board[i, j-1].Value == 0)
                        {
                            done = false;
                        }
                    }
                }
                while (!done);
                
            }
            for (int i = 0; i < 4; i++) 
            {
                bool combine;
                do
                {
                    combine = false;
                    for (int j = 0; j < 3; j++)
                    {
                        if (_board[i, j].Value == 0)
                        {
                            _board[i, j].Value = _board[i, j + 1].Value;
                            _board[i, j].Color = RefreshColor(_board[i, j].Value);
                            if (_board[i,j].Value != 0)
                                _board[i, j].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(i, j + 1);
                        }
                        else if (_board[i, j].Value == _board[i, j + 1].Value && !_board[i,j].Combined&& !_board[i, j+1].Combined)
                        {
                            
                            
                            _board[i, j].Value += _board[i, j + 1].Value;
                            _board[i, j].Color = RefreshColor(_board[i, j].Value);
                            NewEmpty(i, j + 1);
                            _bub.Play();
                            _score += _board[i, j].Value;
                            if (Math.Log2(_board[i, j].Value) >= _level)
                            {
                                _level++;
                                _bomb += _level-5;
                            }
                            if (_score > Math.Pow(2,_levelScore) * 1000)
                            {
                                _levelScore++;
                                _upgrades++;
                            }
                            _board[i, j].Combined = true;
                            combine = true;
                        }
                    }
                }
                while (combine);
                
            }
            NewBoxAtRandom();
        }
        public void Left()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _board[i, j].Combined = false;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                bool done;
                do
                {
                    done = true;
                    for (int j = 0; j < 4; j++)
                    {
                        if (j != 3 && _board[j,i].Value == 0)
                        {
                            _board[j, i].Value = _board[j+1, i].Value;
                            _board[j , i].Color = RefreshColor(_board[j, i].Value);
                            if (_board[j,i].Value != 0)
                                _board[j,i].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(j+1, i);
                        }
                        if (j != 0 && _board[j, i].Value != 0 && _board[j-1, i].Value == 0)
                        {
                            done = false;
                        }
                    }
                }
                while (!done);

            }
            for (int i = 0; i < 4; i++)
            {
                bool combine;
                do
                {
                    combine = false;
                    for (int j = 0; j < 3; j++)
                    {
                        if (_board[j, i].Value == 0)
                        {
                            _board[j, i].Value = _board[j+1, i].Value;
                            _board[j, i].Color = RefreshColor(_board[j, i].Value);
                            if (_board[j, i].Value != 0)
                                _board[j, i].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(j + 1, i);
                        }
                        else if (_board[j, i].Value == _board[j+1, i].Value && !_board[j, i].Combined && !_board[j+1, i].Combined)
                        {
                            _board[j, i].Value += _board[j, i].Value;
                            _board[j, i].Color = RefreshColor(_board[j, i].Value);
                            NewEmpty(j+1, i);
                            _bub.Play();
                            _score += _board[i, j].Value;
                            if (Math.Log2(_board[i, j].Value) >= _level)
                            {
                                _level++;
                                _bomb += _level-5;
                            }
                            if (_score > Math.Pow(2, _levelScore) * 1000)
                            {
                                _levelScore++;
                                _upgrades++;
                            }
                            _board[j, i].Combined = true;
                            combine = true;
                        }
                    }
                }
                while (combine);

            }
            NewBoxAtRandom();
        }
        public void Down()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _board[i, j].Combined = false;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                bool done;
                do
                {
                    done = true;
                    for (int j = 3; j >-1; j--)
                    {
                        if (j != 0 && _board[i, j].Value == 0)
                        {
                            _board[i, j].Value = _board[i, j - 1].Value;
                            _board[i, j].Color = RefreshColor(_board[i, j].Value);
                            if (_board[i, j].Value != 0)
                                _board[i, j].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(i, j - 1);
                        }
                        if (j != 3 && _board[i, j].Value != 0 && _board[i, j + 1].Value == 0)
                        {
                            done = false;
                        }
                    }
                }
                while (!done);

            }
            for (int i = 0; i < 4; i++)
            {
                bool combine;
                do
                {
                    combine = false;
                    for (int j = 3; j > 0; j--)
                    {
                        if (_board[i, j].Value == 0)
                        {
                            _board[i, j].Value = _board[i, j - 1].Value;
                            _board[i, j].Color = RefreshColor(_board[i, j].Value);
                            if (_board[i, j].Value != 0)
                                _board[i, j].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(i, j - 1);
                        }
                        else if (_board[i, j].Value == _board[i, j - 1].Value && !_board[i, j].Combined && !_board[i, j-1].Combined)
                        {
                            _board[i, j].Value += _board[i, j - 1].Value;
                            _board[i, j].Color = RefreshColor(_board[i, j].Value);
                            NewEmpty(i, j - 1);
                            _bub.Play();
                            _score += _board[i, j].Value;
                            if (Math.Log2(_board[i, j].Value) >= _level)
                            {
                                _level++;
                                _bomb += _level - 5;
                            }
                            if (_score > Math.Pow(2, _levelScore) * 1000)
                            {
                                _levelScore++;
                                _upgrades++;
                            }
                            _board[i, j].Combined = true;
                            combine = true;
                        }
                    }
                }
                while (combine);

            }
            NewBoxAtRandom();
        }
        public void Right()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _board[i, j].Combined = false;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                bool done;
                do
                {
                    done = true;
                    for (int j = 3; j > -1; j--)
                    {
                        if (j != 0 && _board[j,i].Value == 0)
                        {
                            _board[j, i].Value = _board[j-1, i].Value;
                            _board[j, i].Color = RefreshColor(_board[j, i].Value);
                            if (_board[j, i].Value != 0)
                                _board[j, i].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(j-1, i);
                        }
                        if (j != 3 && _board[j, i].Value != 0 && _board[j+1, i].Value == 0)
                        {
                            done = false;
                        }
                    }
                }
                while (!done);

            }
            for (int i = 0; i < 4; i++)
            {
                bool combine;
                do
                {
                    combine = false;
                    for (int j = 3; j > 0; j--)
                    {
                        if (_board[j, i].Value == 0)
                        {
                            _board[j, i].Value = _board[j-1,i].Value;
                            _board[j, i].Color = RefreshColor(_board[j, i].Value);
                            if (_board[j, i].Value != 0)
                                _board[j, i].Rectangle = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                            NewEmpty(j-1,i);
                        }
                        else if (_board[j, i].Value == _board[j-1, i].Value && !_board[j, i].Combined && !_board[j-1, i].Combined)
                        {
                            _board[j, i].Value += _board[j-1, i].Value;
                            _board[j, i].Color = RefreshColor(_board[j, i].Value);
                            NewEmpty(j-1, i);
                            _bub.Play();
                            _score += _board[i, j].Value;
                            if (Math.Log2(_board[i, j].Value) >= _level)
                            {
                                _level++;
                                _bomb += _level-5;
                            }
                            if (_score > Math.Pow(2, _levelScore) * 1000)
                            {
                                _levelScore++;
                                _upgrades++;
                            }
                            _board[j, i].Combined = true;
                            combine = true;
                        }
                    }
                }
                while (combine);

            }
            NewBoxAtRandom();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _board[i,j].Draw(spriteBatch);
                }
            }
            if (_restartButton != null)
            {
            _restartButton.Draw(spriteBatch);  
            }

        }
        public void Update(MouseState mouseState, MouseState prevMouseState)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _board[i,j].Update();
                }
            }
            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_restartButton != null && _restartButton.Rectangle.Contains(mouseState.Position))
                {
                    _restartButton = null;
                    NewBoard();
                    _block1.Play();
                }
                for (int i = 0;i < 4; i++)
                {
                    for (int j =0; j < 4; j++)
                    {
                        if (BombActive && _rectangles[i, j].Contains(mouseState.Position))
                        {
                            NewEmpty(i,j);
                            BombActive = false;
                            break;
                        }
                    }
                }
            }
            if (_upgrades == 0)
            {
                _upActive = false;
            }
            else if (_upgrades >= 1)
            {
                _upActive = true;
            }
        }
        public void AddTo(int i, int j, int value)
        {
            if (_board[i, j].Value == 0)
            {
                Rectangle rectFilled = new Rectangle(_border.X + 5 + i * 100, _border.Y + 5 + j * 100, 90, 90);
                Color color = RefreshColor(value);
                Box box = new Box(_texture, rectFilled, value, color, _font, "grow");
                _board[i, j] = box;
            }
        }
    }
}

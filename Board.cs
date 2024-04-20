using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;

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
        private int _bomb;
        private bool _bombActive;
        private bool _upActive;
        private int _spawn;
        private int _upgrades;
        private int _levelScore;
        public SoundEffect _pop;
        public SoundEffect _bub;
        public SoundEffect _block1;
        public SoundEffect _block2;
        public SoundEffect _bombsound;
        private Rectangle[,] _rectangles = new Rectangle[4,4];
        public Board(Texture2D texture, Rectangle border, SpriteFont font)
        {
            _texture = texture;
            _border = border;
            _font = font;
            _savetxt = "Save Game";
            _loadtxt = "Load Game";
        }
        public int Score { get { return _score; }set { _score = value; } }
        public int HighScore { get { return _highscore; } set { _highscore = value; } }
        public bool BombActive { get { return _bombActive; } set { _bombActive = value; } }
        public bool UpActive { get { return _upActive; } set { _upActive = value; } }
        public int Bomb { get { return _bomb; } set { _bomb = value; } }
        public int Upgrades { get { return _upgrades; } set { _upgrades = value;} }
        public int Spawn { get { return _spawn; } set { _spawn = value; } }
        private string _loadtxt, _savetxt;
        public string Loadtxt { get { return _loadtxt; } set {  _loadtxt = value; } }
        public string Savetxt { get { return _savetxt; } set { _savetxt = value; } }
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
        public void LoadBoard()
        {
            _loadtxt = "Loading...";
            if (File.Exists(@"history.txt"))
            {
                NewBoard();
                string[] arrLine = File.ReadAllLines(@"history.txt");
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        AddTo(i, j, Convert.ToInt16(arrLine[count]));
                        count++;
                    }
                }
                _levelScore = Convert.ToInt16(arrLine[count]);
                count++;
                _spawn = Convert.ToInt16(arrLine[count]);
                count++;
                _level = Convert.ToInt16(arrLine[count]);
                count++;
                _score = Convert.ToInt32(arrLine[count]);
                count++;
                _bomb = Convert.ToInt16(arrLine[count]);
                count++;
                _upgrades = Convert.ToInt16(arrLine[count]);
                _loadtxt = "Loaded";
            }
            else
            {
                _loadtxt = "Not Found";
            }
        }
        public void SaveBoard()
        {
            _savetxt = "Saving...";
            if (File.Exists(@"history.txt"))
            {
                string[] arrLine = File.ReadAllLines(@"history.txt");
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        arrLine[count]=Convert.ToString(_board[i, j].Value);
                        count++;
                    }
                }
                arrLine[count] = Convert.ToString(_levelScore);
                count++;
                arrLine[count] = Convert.ToString(_spawn);
                count++;
                arrLine[count] = Convert.ToString(_level);
                count++;
                arrLine[count] = Convert.ToString(_score);
                count++;
                arrLine[count] = Convert.ToString(_bomb);
                count++;
                arrLine[count] = Convert.ToString(_upgrades);
                File.WriteAllLines(@"history.txt", arrLine);
            }
            else
            {
                StreamWriter writer = File.CreateText(@"history.txt");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        writer.WriteLine(_board[i, j].Value);
                    }
                }
                writer.WriteLine(_levelScore);
                writer.WriteLine(_spawn);
                writer.WriteLine(_level);
                writer.WriteLine(_score);
                writer.WriteLine(_bomb);
                writer.WriteLine(_upgrades);

                writer.WriteLine();
                writer.Close();
            }
            _savetxt = "Saved";
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
                else if (value == 4096)
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
            _savetxt = "Save Game";
            _loadtxt = "Load Game";
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
                                _bomb++;
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
            _savetxt = "Save Game";
            _loadtxt = "Load Game";
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
                                _bomb++;
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
            _savetxt = "Save Game";
            _loadtxt = "Load Game";
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
                                _bomb++;
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
            _savetxt = "Save Game";
            _loadtxt = "Load Game";
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
                                _bomb ++;
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
                for (int i = 0;i < 4; i++)
                {
                    for (int j =0; j < 4; j++)
                    {
                        if (BombActive && _rectangles[i, j].Contains(mouseState.Position)&&_board[i, j].Value != 0)
                        {
                            NewEmpty(i,j);
                            BombActive = false;
                            _bombsound.Play();
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
            _bub.Play();
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Security.Principal;

namespace _2048
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D rectTexture, squareTexture, circleTexture,UpTexture;
        SpriteFont spriteFont,spriteFont2;
        Texture2D bombTexture;
        Texture2D restartTexture;

        int highScore,userbomb;

        bool bombActive = false;
        bool upgradeActive = false;
        int objectFloat=0,period = 4;
        int upgrades;

        Rectangle rectBorder,rectMargin,rectBomb,rectUpgrade,rectSave,rectLoad,rectRestart;

        string saveGameText,loadGameText;
        int width,height;

        int score;
        
        KeyboardState keyboardState,prevKeyboardState;

        MouseState mouseState, prevMouseState;

        Board game;

        SoundEffect popSound, bubSound, block1Sound, block2Sound,bombSound;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
            width = _graphics.PreferredBackBufferWidth;
            height = _graphics.PreferredBackBufferHeight;

            
            rectBorder = new Rectangle((width / 2 - 210), height / 2 - 210, 400, 400);
            rectMargin = new Rectangle((width / 2 - 230), height / 2 - 230, 440, 440);
            rectBomb = new Rectangle(width / 15, height / 6, 64, 64);
            rectUpgrade = new Rectangle(width / 15, height * 2 / 5, 64, 64);
            rectSave = new Rectangle(width * 4 / 5, height * 5 / 6, 150, 50);
            rectLoad = new Rectangle(width * 4 / 5, height * 2/3, 150, 50);
            rectRestart = new Rectangle(width *17/20, height / 6, 64, 64);


            score = 0;

            if (File.Exists(@"data.txt"))
            {
                string[] arrLine = File.ReadAllLines(@"data.txt");
                for (int i = 0; i < arrLine.Length; i++)
                {
                    if ( i == 0)
                    {
                        highScore = Convert.ToInt32(arrLine[i]);
                    }
                }
            }
            else
            {
                StreamWriter writer = File.CreateText(@"data.txt");
                writer.WriteLine("10");
                writer.Close();
            }

            base.Initialize();
            game = new Board(rectTexture, rectBorder,spriteFont);
            game._pop = popSound;
            game._bub = bubSound;
            game._block1 = block1Sound; game._block2 = block2Sound;
            game._bombsound = bombSound;
            game.NewBoard();
            game.AddTo(0, 0, 2);
            game.AddTo(1, 0, 0);
            game.AddTo(2, 0, 4);
            game.AddTo(3, 0, 8);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            rectTexture = Content.Load<Texture2D>("rectangle");
            squareTexture = Content.Load<Texture2D>("square");
            circleTexture = Content.Load<Texture2D>("circle");
            spriteFont = Content.Load<SpriteFont>("SpriteFont");
            popSound = Content.Load<SoundEffect>("pop");
            bubSound = Content.Load<SoundEffect>("bubpop");
            block1Sound = Content.Load<SoundEffect>("block1");
            block2Sound = Content.Load<SoundEffect>("block2");
            bombTexture = Content.Load<Texture2D>("bombTexture");
            restartTexture = Content.Load<Texture2D>("restart");
            UpTexture = Content.Load<Texture2D>("UpArrow");
            spriteFont2 = Content.Load<SpriteFont>("SpriteFont2");
            bombSound = Content.Load<SoundEffect>("explode");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            objectFloat++;
            if (objectFloat % period == 0)
            {
                if (objectFloat > period * 5)
                {
                    rectUpgrade.Y += 1;
                }
                else if (objectFloat <= period * 5)
                {
                    rectUpgrade.Y -= 1;
                }

            }
            if (objectFloat > period * 10)
            {
                objectFloat = 0;
            }

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            string[] arrLine = File.ReadAllLines(@"data.txt");
            for (int i = 0; i < arrLine.Length; i++)
            {
                if (score > highScore && i == 0)
                {
                    arrLine[i] = Convert.ToString(score);
                    File.WriteAllLines(@"data.txt", arrLine);
                    highScore = Convert.ToInt32(arrLine[i]);
                }
            }

            if (rectSave.Contains(mouseState.Position))
            {
                rectSave = new Rectangle(width * 4 / 5+5, height * 5 / 6+2, 140, 46);
            }
            else
            {
                rectSave = new Rectangle(width * 4 / 5, height * 5 / 6, 150, 50);
            }

            if (rectLoad.Contains(mouseState.Position))
            {
                rectLoad = new Rectangle(width * 4 / 5 + 5, height * 2 / 3 + 2, 140, 46);
            }
            else
            {
                rectLoad = new Rectangle(width * 4 / 5, height * 2 / 3, 150, 50);
            }

            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                if (rectBomb.Contains(mouseState.Position)&&userbomb>0&&!bombActive)
                {
                    userbomb -= 1;
                    game.Bomb = userbomb;
                    bombActive = true;
                    game.BombActive = bombActive;
                }
                else if (rectUpgrade.Contains(mouseState.Position))
                {
                    upgrades--;
                    game.Upgrades = upgrades;
                    game.Spawn++;
                }
                else if (rectRestart.Contains(mouseState.Position))
                {
                    game.NewBoard();
                    block1Sound.Play();
                }
                if (rectSave.Contains(mouseState.Position))
                {
                    game.SaveBoard();
                }
                if (rectLoad.Contains(mouseState.Position))
                {
                    game.LoadBoard();
                }
            }
            else if (mouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed)
            {
                if (bombActive)
                {
                    userbomb += 1;
                    game.Bomb = userbomb;
                    bombActive = false;
                    game.BombActive = bombActive;
                }

            }

            if (keyboardState.IsKeyDown(Keys.W)&&prevKeyboardState.IsKeyUp(Keys.W))
            {
                game.Up();
            }
            else if (keyboardState.IsKeyDown(Keys.S) && prevKeyboardState.IsKeyUp(Keys.S))
            {
                game.Down();
            }
            else if (keyboardState.IsKeyDown(Keys.A) && prevKeyboardState.IsKeyUp(Keys.A))
            {
                game.Left();
            }
            else if (keyboardState.IsKeyDown(Keys.D) && prevKeyboardState.IsKeyUp(Keys.D))
            {
                game.Right();
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up))
            {
                game.Up();
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down))
            {
                game.Down();
            }
            else if (keyboardState.IsKeyDown(Keys.Left) && prevKeyboardState.IsKeyUp(Keys.Left))
            {
                game.Left();
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && prevKeyboardState.IsKeyUp(Keys.Right))
            {
                game.Right();
            }
            game.Update(mouseState,prevMouseState);
            score = game.Score;
            userbomb = game.Bomb;
            bombActive = game.BombActive;
            upgradeActive = game.UpActive;
            upgrades = game.Upgrades;
            saveGameText = game.Savetxt;
            loadGameText = game.Loadtxt;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.Draw(rectTexture, rectMargin, Color.SandyBrown);
            _spriteBatch.Draw(rectTexture, rectBorder, Color.LightGray);
            game.Draw(_spriteBatch);
            _spriteBatch.DrawString(spriteFont, "Score: " + score, new Vector2(rectMargin.X-50,rectMargin.Y - 50), Color.Black) ;
            _spriteBatch.DrawString(spriteFont, "Highscore: " + highScore, new Vector2(rectMargin.X + 250, rectMargin.Y - 50), Color.Black);
            _spriteBatch.Draw(bombTexture, rectBomb, Color.White);
            _spriteBatch.DrawString(spriteFont, userbomb+"",new Vector2(rectBomb.Right, rectBomb.Bottom), Color.Black);
            _spriteBatch.Draw(squareTexture, rectSave, Color.Green);
            Vector2 txtVector = new Vector2(rectSave.X+rectSave.Width/2-spriteFont2.MeasureString(saveGameText).X/2, rectSave.Y+rectSave.Height/2-spriteFont2.MeasureString(saveGameText).Y/2);
            _spriteBatch.DrawString(spriteFont2, saveGameText, txtVector, Color.White);
            _spriteBatch.Draw(squareTexture, rectLoad, Color.GreenYellow);
            Vector2 txtVector1 = new Vector2(rectLoad.X + rectLoad.Width / 2 - spriteFont2.MeasureString(loadGameText).X / 2, rectLoad.Y + rectLoad.Height / 2 - spriteFont2.MeasureString(loadGameText).Y / 2);
            _spriteBatch.DrawString(spriteFont2, loadGameText, txtVector1, Color.White) ;
            _spriteBatch.Draw(restartTexture, rectRestart, Color.White);

            if (upgradeActive)
            {
                _spriteBatch.Draw(UpTexture, rectUpgrade, Color.LightGreen);
            }

            if (bombActive)
            {
                Rectangle mouseRect = new Rectangle(mouseState.X-16,mouseState.Y-16,32,32);
                _spriteBatch.Draw(bombTexture, mouseRect, Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
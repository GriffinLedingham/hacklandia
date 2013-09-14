using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Corgie;

namespace PurpleCorgi
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static Texture2D whitePixel = null;
        public static Texture2D WhitePixel { get { return whitePixel; } }

        private static Random rand = new Random();
        public static Random GameRandom { get { return rand; } }

        private MiniGameContext[] miniGames;
        private RenderTarget2D testMiniGameCanvas;
        private MiniGame testMiniGame;

        public static Texture2D corgi_Sprite; 


        private class MiniGameContext
        {
            public MiniGame game;
            public RenderTarget2D canvas;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = GameConstants.GameResolutionWidth;
            graphics.PreferredBackBufferHeight = GameConstants.GameResolutionHeight;

            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Kinect ein = new Kinect(100,100);
            ein.Init();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });

            testMiniGameCanvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
            testMiniGame = new TestMiniGame(GraphicsDevice);

            corgi_Sprite = Content.Load<Texture2D>("corgi");

            miniGames = new MiniGameContext[4];
            for (int i = 0; i < 4; i++)
            {
                if (i == 1)
                {
                    miniGames[i] = new MiniGameContext();
                    miniGames[i].game = new HeadBallGame(GraphicsDevice);
                    miniGames[i].canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                }
                else if (i == 0)
                {
                    miniGames[i] = new MiniGameContext();
                    miniGames[i].game = new PlatformerGame(GraphicsDevice);
                    miniGames[i].canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                }
                else
                {
                    miniGames[i] = new MiniGameContext();
                    miniGames[i].game = new TestMiniGame(GraphicsDevice);
                    miniGames[i].canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            for (int i = 0; i < 4; i++)
            {
                miniGames[i].game.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
      
            // render frames for each mini game
            for (int i = 0; i < 4; i++)
            {
                miniGames[i].game.Render(miniGames[i].canvas);
            }

            // render mini games to screen
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Matrix.Identity);
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(miniGames[i].canvas, new Vector2(GameConstants.MiniGameCanvasWidth * (i % 2), GameConstants.MiniGameCanvasHeight * (i / 2)), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

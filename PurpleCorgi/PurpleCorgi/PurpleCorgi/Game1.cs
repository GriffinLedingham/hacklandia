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

        private List<MiniGameContext> miniGames;

        public static Texture2D corgi_Sprite; 
        public static Texture2D spaceSheet;

        public static SpriteFont SegoeUIMono24 = null;

        public static Effect BlackAndWhite = null;

        private MetaGameState gameState = MetaGameState.Init;

        // running logic
        private float addMiniGameTimer;
        private const float addSecondMiniGameDuration = 3000f;
        private const float addThirdMiniGameDuration = 6000f;
        private const float addFourthMiniGameDuration = 9000f;
        //end running logic

        private enum MetaGameState
        {
            /// <summary>
            /// Metagame state on startup. This is used to indicate that game logic needs to be initialized. 
            /// </summary>
            Init,
            
            /// <summary>
            /// Game is waiting for player to enter Kinect space and be ready.
            /// </summary>
            Lobby,

            /// <summary>
            /// Game is running and throwing new minigames. Woo.
            /// </summary>
            Running,

            /// <summary>
            /// Player has lost the game. Some sort of result screen that indiciates the Player's progress.
            /// </summary>
            PlayerLose,
        }

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
            Kinect ein = new Kinect(0, 0);
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

            spaceSheet = Content.Load<Texture2D>("spaceSheet");
            corgi_Sprite = Content.Load<Texture2D>("corgi");
            SegoeUIMono24 = Content.Load<SpriteFont>("segoe24");

            BlackAndWhite = Content.Load<Effect>("BlackAndWhite");

            miniGames = new List<MiniGameContext>();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private MiniGame PureRandomMiniGame()
        {
            switch (GameRandom.Next() % 4)
            {
                case 0:
                    return new SpaceGame(GraphicsDevice);
                case 1:
                    return new PaddleMiniGame(GraphicsDevice);
                case 2:
                    return new PlatformerGame(GraphicsDevice);
                case 3:
                default:
                    return new TestMiniGame(GraphicsDevice);
            }
        }

        private void UpdateInit(GameTime gameTime)
        {
            gameState = MetaGameState.Lobby;
        }

        private void UpdateLobby(GameTime gameTime)
        {
            //prepare the game for running
            {
                miniGames = new List<MiniGameContext>();
                addMiniGameTimer = 0;
                gameState = MetaGameState.Running;
            }
        }

        private void UpdateRunning(GameTime gameTime)
        {
            addMiniGameTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (miniGames.Count == 0 && addMiniGameTimer > 0)
            {
                MiniGameContext context = new MiniGameContext();
                context.game = new PaddleMiniGame(GraphicsDevice);
                context.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                miniGames.Add(context);
            }
            else if (miniGames.Count == 1 && addMiniGameTimer > addSecondMiniGameDuration)
            {
                MiniGameContext context = new MiniGameContext();
                context.game = new SpaceGame(GraphicsDevice);
                context.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                miniGames.Add(context);
            }
            else if (miniGames.Count == 2 && addMiniGameTimer > addThirdMiniGameDuration)
            {
                MiniGameContext context = new MiniGameContext();
                context.game = new TestMiniGame(GraphicsDevice);
                context.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                miniGames.Add(context);
            }
            else if (miniGames.Count == 3 && addMiniGameTimer > addFourthMiniGameDuration)
            {
                MiniGameContext context = new MiniGameContext();
                context.game = new PlatformerGame(GraphicsDevice);
                context.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);
                miniGames.Add(context);
            }

            for (int i = 0; i < miniGames.Count; i++)
            {
                miniGames[i].game.Update(gameTime);
            }

            foreach (MiniGameContext me in miniGames)
            {
                if (me.game.GetState() == MiniGameState.Win)
                {
                    me.game = PureRandomMiniGame();
                }
                else if (me.game.GetState() == MiniGameState.Lose)
                {
                    gameState = MetaGameState.PlayerLose;
                }
            }
        }

        private void UpdateLose(GameTime gameTime)
        {
            //
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

            switch (gameState)
            {
                case MetaGameState.Init:
                    UpdateInit(gameTime);
                    break;
                case MetaGameState.Lobby:
                    UpdateLobby(gameTime);
                    break;
                case MetaGameState.Running:
                    UpdateRunning(gameTime);
                    break;
                case MetaGameState.PlayerLose:
                    UpdateLose(gameTime);
                    break;
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
            for (int i = 0; i < miniGames.Count; i++)
            {
                miniGames[i].game.Render(miniGames[i].canvas);
            }

            if (gameState == MetaGameState.Init)
            {
                //
            }
            else if (gameState == MetaGameState.Lobby)
            {
                //
            }
            else if (gameState == MetaGameState.Running)
            {
                // render mini games to screen
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Salmon);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Matrix.Identity);
                for (int i = 0; i < miniGames.Count; i++)
                {
                    spriteBatch.Draw(miniGames[i].canvas, new Vector2(GameConstants.MiniGameCanvasWidth * (i % 2), GameConstants.MiniGameCanvasHeight * (i / 2)), Color.White);
                }
                spriteBatch.End();
            }
            else if (gameState == MetaGameState.PlayerLose)
            {
                // render mini games to screen
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Salmon);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, BlackAndWhite, Matrix.Identity);
                for (int i = 0; i < miniGames.Count; i++)
                {
                    spriteBatch.Draw(miniGames[i].canvas, new Vector2(GameConstants.MiniGameCanvasWidth * (i % 2), GameConstants.MiniGameCanvasHeight * (i / 2)), Color.White);
                }
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Matrix.Identity);
                spriteBatch.Draw(Game1.whitePixel, new Rectangle((GameConstants.GameResolutionWidth / 2 - 200), (GameConstants.GameResolutionHeight / 2 - 100), 400, 200), Color.Blue);
                spriteBatch.DrawString(SegoeUIMono24, "LOSE", (new Vector2(GameConstants.GameResolutionWidth, GameConstants.GameResolutionHeight) - SegoeUIMono24.MeasureString("LOSE")) / 2, Color.Cyan);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}

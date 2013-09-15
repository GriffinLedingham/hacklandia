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
        Kinect ein = new Kinect(0, 0);
        private static Texture2D whitePixel = null;
        public static Texture2D WhitePixel { get { return whitePixel; } }

        private static Random rand = new Random();
        public static Random GameRandom { get { return rand; } }

        private List<MiniGameContext> miniGames;

        public static Texture2D corgi_Sprite;
        public static Texture2D peak_Sprite;
        public static Texture2D hand_closed_sprite;
        public static Texture2D hand_open_sprite; 
        public static Texture2D spaceSheet;
        public static Texture2D tutorialFrames;
        public static Texture2D handCursor;

        public static SpriteFont SegoeUIMono24 = null;
        public static SpriteFont SegoeUIMono72 = null;

        public static Effect BlackAndWhite = null;

        private MetaGameState gameState = MetaGameState.Init;

        private bool lobbyHandClosed = false;

        public static int score;

        // running logic
        private float addMiniGameTimer;
        private const float addSecondMiniGameDuration = 20000f;
        private const float addThirdMiniGameDuration = 40000f;
        private const float addFourthMiniGameDuration = 9000000000000000000f;
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
            tutorialFrames = Content.Load<Texture2D>("tutorialFrames");
            SegoeUIMono24 = Content.Load<SpriteFont>("segoe24");
            SegoeUIMono72 = Content.Load<SpriteFont>("segoe72");
            handCursor = Content.Load<Texture2D>("hands");

            BlackAndWhite = Content.Load<Effect>("BlackAndWhite");

            miniGames = new List<MiniGameContext>();

            peak_Sprite = Content.Load<Texture2D>("peaktit");
            hand_closed_sprite = Content.Load<Texture2D>("hand_closed");
            hand_open_sprite = Content.Load<Texture2D>("hand_open");

            g1 = new MiniGameContext();
            g2 = new MiniGameContext();
            g3 = new MiniGameContext();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


                private MiniGame PureRandomMiniGameHelper(){
            
            switch ((new Random()).Next() % 9) 
            {
                case 0:
                    return new TittyTwist(GraphicsDevice);
                case 1:
                    return new PlatformerGame(GraphicsDevice);
                case 2:
                    return new TittyTwistRight(GraphicsDevice);
                case 3:
                    return new PaddleMiniGame(GraphicsDevice);
                case 4:
                    return new Kardashian(GraphicsDevice);
                case 5:
                    return new BrickMiniGame(GraphicsDevice);
                case 6:
                    return new HeadBallGame(GraphicsDevice);
                case 7:
                    return new SpaceGame(GraphicsDevice);
                default:
                    return new ColorGame(GraphicsDevice);
               

            }
        }
        private int TypeOfGame(MiniGame g) {
            if(g is TittyTwist) return 1;
            if(g is PlatformerGame) return 1;
            if(g is TittyTwistRight) return 2;
            if(g is PaddleMiniGame) return 2;
            if(g is Kardashian) return 3;
            if(g is BrickMiniGame) return 3;
            if(g is HeadBallGame) return 4;
            if(g is SpaceGame) return 5;
            if(g is ColorGame) return 5;
            if(g is FootGame) return 6;

            return 0;
        }

          private MiniGame PureRandomMiniGame()
        {

            MiniGame game;
            while(true){
                game = PureRandomMiniGameHelper();
                int type = TypeOfGame(game);
                if (type != 5) continue; 
                bool conflict = false;
                foreach(MiniGameContext m in miniGames){
                    if(type == TypeOfGame(m.game)){
                        conflict = true;
                        break;
                    }
                }
                if(!conflict)
                    break;
            }
            return game;
        }

        private void UpdateInit(GameTime gameTime)
        {

            Lobby.Pointer.X = (int)(ein.RightHand.Pos.X * GameConstants.MiniGameCanvasWidth);
            Lobby.Pointer.Y = (int)(ein.RightHand.Pos.Y * GameConstants.MiniGameCanvasHeight);

            bool thing = ein.RightHand.Gripped;
            bool thing2 = ein.RightHand.Released;
            
            if (thing)
            {
                lobbyHandClosed = true;
            }

            if (thing2)
            {
                lobbyHandClosed = false;
            }

            if (Rectangle.Intersect(Lobby.Box1, Lobby.Pointer).Height > 0 && (!Lobby.prevFrameGrabbing && lobbyHandClosed))
            {
                Lobby.Difficulty = 1;
            }
            else if (Rectangle.Intersect(Lobby.Box2, Lobby.Pointer).Height > 0 && (!Lobby.prevFrameGrabbing && lobbyHandClosed))
            {
                Lobby.Difficulty = 2;
            }
            else if (Rectangle.Intersect(Lobby.Box3, Lobby.Pointer).Height > 0 && (!Lobby.prevFrameGrabbing && lobbyHandClosed))
            {
                Lobby.Difficulty = 3;
            }

            if (Lobby.Difficulty != 0)
            {
                gameState = MetaGameState.Lobby;
            }

            Console.WriteLine(lobbyHandClosed);

            Lobby.prevFrameGrabbing = lobbyHandClosed;

        }

        private void UpdateLobby(GameTime gameTime)
        {

            Lobby.User.X = (int)(((ein.UserX / .5f) * GameConstants.MiniGameCanvasWidth) + GameConstants.MiniGameCanvasWidth);
            Lobby.User.Y = (int)(((ein.UserZ - 0.9f) * GameConstants.MiniGameCanvasHeight));


            if (Rectangle.Intersect(Lobby.User, Lobby.Landing).Height > 0)
            {
                Lobby.landingHoverTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (Lobby.landingHoverTimer > Lobby.landingHoverDuration)
                {
                    Lobby.READY = true;
                }
            }
            else
            {
                Lobby.landingHoverTimer = 0;
            }


            if(Lobby.READY){
                miniGames = new List<MiniGameContext>();
                addMiniGameTimer = 0;
                gameState = MetaGameState.Running;
                ein.LastColor = null;
                score = 0;
            }
        }

        MiniGameContext g1, g2, g3;

        private void UpdateRunning(GameTime gameTime)
        {
            addMiniGameTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (miniGames.Count == 0 && addMiniGameTimer > 0)
            {
                
                g1.game = PureRandomMiniGame();
                g1.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);

                miniGames.Add(g1);
            }
            else if (miniGames.Count == 1 && addMiniGameTimer > addSecondMiniGameDuration && Lobby.Difficulty > 1)
            {
                g2.game = PureRandomMiniGame();
                g2.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);

                miniGames.Add(g2);
            }
            else if (miniGames.Count == 2 && addMiniGameTimer > addThirdMiniGameDuration && Lobby.Difficulty > 2)
            {
                g3.game = PureRandomMiniGame();
                g3.canvas = new RenderTarget2D(GraphicsDevice, GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);

                miniGames.Add(g3);
            }

            for (int i = 0; i < miniGames.Count; i++)
            {
                miniGames[i].game.Update(gameTime);
            }

            foreach (MiniGameContext me in miniGames)
            {
                if (me.game.GetState() == MiniGameState.Win)
                {
                    score += 100;

                    if (TypeOfGame(me.game) == 5) ein.LastColor = null;

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
            Lobby.LosePointer.X = (int)(((ein.UserX / .5f) * GameConstants.MiniGameCanvasWidth) + GameConstants.MiniGameCanvasWidth);
            Lobby.LosePointer.Y = (int)(((ein.UserZ - 0.9f) * GameConstants.MiniGameCanvasHeight));

            if (Rectangle.Intersect(Lobby.LosePointer, Lobby.LoseLanding).Height > 0)
            {
                gameState = MetaGameState.Init;

                foreach (MiniGameContext me in miniGames)
                {
                    me.game = null;
                }
              
                miniGames.Clear();
                Lobby.READY = false;
                Lobby.Difficulty = 0;
                Lobby.landingHoverTimer = 0;
                score = 0;
            }
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
                // render mini games to screen
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

                spriteBatch.Draw(Game1.WhitePixel, Lobby.Box1, Color.Red);
                spriteBatch.DrawString(Game1.SegoeUIMono24, "Easy\nOne Game", new Vector2(Lobby.Box1.X, Lobby.Box1.Y) + new Vector2(8), Color.White);
                spriteBatch.Draw(Game1.WhitePixel, Lobby.Box2, Color.Blue);
                spriteBatch.DrawString(Game1.SegoeUIMono24, "Medium\nTwo Games", new Vector2(Lobby.Box2.X, Lobby.Box2.Y) + new Vector2(8), Color.White);
                spriteBatch.Draw(Game1.WhitePixel, Lobby.Box3, Color.Green);
                spriteBatch.DrawString(Game1.SegoeUIMono24, "Hard\nThree Games", new Vector2(Lobby.Box3.X, Lobby.Box3.Y) + new Vector2(8), Color.White);

                spriteBatch.Draw(Game1.handCursor, Lobby.Pointer, new Rectangle((lobbyHandClosed ? 150 : 0), 0, 150, 150), Color.White);
                
                spriteBatch.End();
            }
            else if (gameState == MetaGameState.Lobby)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

                spriteBatch.DrawString(SegoeUIMono24, "Align yourself in front of the Kinect", new Vector2(75), Color.White);

                spriteBatch.Draw(Game1.WhitePixel, Lobby.Landing, Color.Red);
                spriteBatch.Draw(Game1.WhitePixel, Lobby.User, new Color(1, 1, 1, 0.5f));

                if (Lobby.landingHoverTimer > 0)
                {
                    if (Lobby.landingHoverTimer / Lobby.landingHoverDuration < 0.333f)
                    {
                        spriteBatch.DrawString(Game1.SegoeUIMono72, "3", (new Vector2(GameConstants.GameResolutionWidth, GameConstants.GameResolutionHeight) + Game1.SegoeUIMono72.MeasureString("3")) / 2 - new Vector2(0, 100), Color.White);
                    }
                    else if (Lobby.landingHoverTimer / Lobby.landingHoverDuration < 0.6666f)
                    {
                        spriteBatch.DrawString(Game1.SegoeUIMono72, "2", (new Vector2(GameConstants.GameResolutionWidth, GameConstants.GameResolutionHeight) + Game1.SegoeUIMono72.MeasureString("2")) / 2 - new Vector2(0, 100), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(Game1.SegoeUIMono72, "1", (new Vector2(GameConstants.GameResolutionWidth, GameConstants.GameResolutionHeight) + Game1.SegoeUIMono72.MeasureString("1")) / 2 - new Vector2(0, 100), Color.White);
                    }

                }

                spriteBatch.End();
            }
            else if (gameState == MetaGameState.Running)
            {
                // render mini games to screen
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Matrix.Identity);
                for (int i = 0; i < miniGames.Count; i++)
                {
                    spriteBatch.Draw(miniGames[i].canvas, new Vector2(GameConstants.MiniGameCanvasWidth * (i % 2), GameConstants.MiniGameCanvasHeight * (i / 2)), Color.White);
                }

                spriteBatch.DrawString(SegoeUIMono24, "Score: " + score, new Vector2(640, 360) + new Vector2(16), Color.White);
                spriteBatch.End();
            }
            else if (gameState == MetaGameState.PlayerLose)
            {
                // render mini games to screen
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, BlackAndWhite, Matrix.Identity);
                for (int i = 0; i < miniGames.Count; i++)
                {
                    spriteBatch.Draw(miniGames[i].canvas, new Vector2(GameConstants.MiniGameCanvasWidth * (i % 2), GameConstants.MiniGameCanvasHeight * (i / 2)), Color.White);
                }

                spriteBatch.DrawString(SegoeUIMono24, "Score: " + score, new Vector2(640, 360) + new Vector2(16), Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Matrix.Identity);
                spriteBatch.Draw(Game1.whitePixel, new Rectangle((GameConstants.GameResolutionWidth / 2 - 200), (GameConstants.GameResolutionHeight / 2 - 100), 400, 200), Color.Blue);

                spriteBatch.DrawString(SegoeUIMono24, "You lost!\nScore: " + score, (new Vector2(GameConstants.GameResolutionWidth, GameConstants.GameResolutionHeight) - SegoeUIMono24.MeasureString("You lost!\nScore: " + score)) / 2, Color.Cyan);

                spriteBatch.Draw(Game1.whitePixel, Lobby.LosePointer, Color.Yellow);
                spriteBatch.Draw(Game1.whitePixel, Lobby.LoseLanding, Color.White);
                spriteBatch.DrawString(SegoeUIMono24, "Stand in me to restart!", (new Vector2(GameConstants.GameResolutionWidth, GameConstants.GameResolutionHeight) - SegoeUIMono24.MeasureString("Stand in me to restart!")) / 2 + new Vector2(200, -200), Color.Black);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}

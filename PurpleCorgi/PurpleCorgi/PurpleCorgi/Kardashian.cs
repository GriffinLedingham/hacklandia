using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Corgie;

namespace PurpleCorgi
{
    class Kardashian : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        Body body, body2;

        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        public static bool ShowedTutorial = false;
        private float tutorialTimer;
        private const float tutorialDuration = 1000f;
        World world;
        Texture2D PlainTexture, GoalTexture;

        Vector2 Size, size, Size2, size2;

        bool win, lose = false;

        float height, width, density, posx, posy;
        float height2, width2, density2, posx2, posy2;

        float gravity;

        private Kinect ein;

        private MiniGameState gameState;

        private Random random;

        float winTimer = 0.0f;

        int score = 0;

        private bool colliding = false;

        public Kardashian(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            random = new Random();

            ein = new Kinect(100, 100);
            ein.Init();

            height = 50;
            width = 50;
            density = 1f;
            posx = 200;
            posy = 50;

            height2 = 50;
            width2 = 50;
            density2 = 1f;
            posx2 = 200;
            posy2 = 200;

            gravity = 0f;

            // TODO: Add your initialization logic here
            world = new World(new Vector2(0, gravity));

            PlainTexture = new Texture2D(this.graphicsDevice, 1, 1);
            PlainTexture.SetData(new[] { Color.White });

            GoalTexture = new Texture2D(this.graphicsDevice, 1, 1);
            GoalTexture.SetData(new[] { Color.Red });

            body = BodyFactory.CreateRectangle(world, width * pixelToUnit, height * pixelToUnit, density);
            body.BodyType = BodyType.Dynamic;
            size = new Vector2(width, height);
            Size = size;
            body.Position = new Vector2(posx * pixelToUnit, posy * pixelToUnit);
            body.SleepingAllowed = false;

            body2 = BodyFactory.CreateRectangle(world, width2 * pixelToUnit, height2 * pixelToUnit, density2);
            body2.BodyType = BodyType.Dynamic;
            size2 = new Vector2(width2, height2);
            Size2 = size2;
            body2.Position = new Vector2(random.Next(1, GameConstants.MiniGameCanvasWidth) * pixelToUnit, random.Next(1, GameConstants.MiniGameCanvasHeight) * pixelToUnit);
            body2.SleepingAllowed = false;
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }
            if (!ShowedTutorial)
            {
                tutorialTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (tutorialTimer > tutorialDuration)
                {
                    ShowedTutorial = true;
                }

                return;
            }
            winTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (colliding == false)
                body.OnCollision += body_OnCollision;
            else
            {
                body2.Position = new Vector2(random.Next(1, GameConstants.MiniGameCanvasWidth) * pixelToUnit, random.Next(1, GameConstants.MiniGameCanvasHeight) * pixelToUnit);
                //Console.WriteLine("Score +1");
                score++;
                colliding = false;
            }

            if (score >= 2 && !lose)
            {
                win = true;
            }

            if (winTimer > 10000 && !win)
            {
                lose = true;

            }

            body.Position = new Vector2((((ein.UserX / .5f) * GameConstants.MiniGameCanvasWidth / 2) + GameConstants.MiniGameCanvasWidth/2) * pixelToUnit, 
                (((ein.UserZ -.9f) * GameConstants.MiniGameCanvasHeight / 2)) * pixelToUnit);

            // Simulate physics.
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 scale = new Vector2(Size.X / (float)PlainTexture.Width, Size.Y / (float)PlainTexture.Height);
            Vector2 scale2 = new Vector2(Size2.X / (float)PlainTexture.Width, Size2.Y / (float)PlainTexture.Height);
            
            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            if (!ShowedTutorial)
            {
                sb.Draw(Game1.tutorialFrames, new Vector2(40, 10), new Rectangle(((int)(tutorialTimer / 300f) % 2) * 300, 600, 300, 300), Color.White);
            }
            sb.Draw(PlainTexture, body.Position * unitToPixel, null, Color.White, body.Rotation, new Vector2(PlainTexture.Width / 2.0f, PlainTexture.Height / 2.0f), scale, SpriteEffects.None, 0);
            sb.Draw(GoalTexture, body2.Position * unitToPixel, null, Color.White, body2.Rotation, new Vector2(PlainTexture.Width / 2.0f, PlainTexture.Height / 2.0f), scale2, SpriteEffects.None, 0);
            
            sb.End();

            if (win)
            {
                graphicsDevice.Clear(Color.Red);
                winTimer = 0.0f;
            }

            if (lose)
            {
                graphicsDevice.Clear(Color.Black);
            }
        }

        private bool body_OnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (f1.Body == body && f2.Body == body2 && colliding == false)
            {
                colliding = true;
            }
            return true;
        }

        public MiniGameState GetState()
        {
            if (win)
                return MiniGameState.Win;
            else if (lose)
                return MiniGameState.Lose;
            else
                return MiniGameState.Running;
        }

       
    }
}

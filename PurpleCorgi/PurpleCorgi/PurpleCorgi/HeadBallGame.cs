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
    class HeadBallGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private float paddle_width = 256;
        private float paddle_height = 16;
        private float ball_radius = 8;

        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        private MiniGameState gameState;

        private World physicsWorld;
        private Body paddle;
        private Vector2 paddle_size;
        private Body ball;

        private Texture2D circleTexture;

        private Kinect ein;

        private bool win, lost = false;

        float winTimer = 0.0f;

        public HeadBallGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            ein = new Kinect(100, 100);
            ein.Init();

            circleTexture = CreateCircle(8, graphicsDevice);

            gameState = MiniGameState.Initialized;

            physicsWorld = new World(new Vector2(0, 0.8f));

            paddle = BodyFactory.CreateRectangle(physicsWorld, paddle_width * pixelToUnit, paddle_height * pixelToUnit, 1000f);
            paddle.BodyType = BodyType.Static;
            paddle.Position = new Vector2(320 * pixelToUnit, 240 * pixelToUnit);
            paddle_size = new Vector2(paddle_width * pixelToUnit, paddle_height * pixelToUnit);
            paddle.Rotation = 0;

            ball = BodyFactory.CreateCircle(physicsWorld, ball_radius * pixelToUnit, 1000f);
            ball.BodyType = BodyType.Dynamic;
            ball.Position = new Vector2(320 * pixelToUnit, 100 * pixelToUnit);
            ball.SleepingAllowed = false;
        }


        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            winTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (winTimer > 10000)
            {
                win = true;
            }

            if (ball.Position.Y*unitToPixel > GameConstants.MiniGameCanvasHeight)
            {
                lost = true;
            }

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Tab))
            {
                ball.Position = new Vector2(320 * pixelToUnit, 100 * pixelToUnit);
                ball.AngularVelocity = 0;
                ball.LinearVelocity = Vector2.Zero;
            }

            /*
            if (ks.IsKeyDown(Keys.A))
            {
                paddle.Rotation += 0.01f;
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                paddle.Rotation -= 0.01f;
            }
             */


            //paddle.Rotation -= (ein.RightHandAngle)/50.0f;

            //paddle.Rotation = -ein.RightHandAngle;
            if (Math.Abs(paddle.Rotation - (-1 * ein.HeadNormAngle)) > 0.025f)
            {
                if (paddle.Rotation < (-1 * ein.HeadNormAngle))
                {
                    paddle.Rotation += 0.051f;
                }
                else if (paddle.Rotation > (-1 * ein.HeadNormAngle))
                {
                    paddle.Rotation -= 0.051f;
                }
            }

            // Simulate physics.
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.White);
            Vector2 paddle_scale = new Vector2(paddle_size.X * unitToPixel, paddle_size.Y * unitToPixel);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sb.Draw(Game1.WhitePixel, paddle.Position * unitToPixel, null, Color.Black, paddle.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), paddle_scale, SpriteEffects.None, 0);
            sb.Draw(circleTexture, ball.Position * unitToPixel, null, Color.Black, ball.Rotation, new Vector2(16) / 2, new Vector2(1), SpriteEffects.None, 0.0f);
            sb.End();

            if (win)
                graphicsDevice.Clear(Color.Red);

            if (lost)
                graphicsDevice.Clear(Color.Black);
        }

        public MiniGameState GetState()
        {
            if (win)
                return MiniGameState.Win;
            else if (lost)
                return MiniGameState.Lose;
            else
                return MiniGameState.Running;
        }

        public Texture2D CreateCircle(int radius, GraphicsDevice gd)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(gd, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }
    }
}

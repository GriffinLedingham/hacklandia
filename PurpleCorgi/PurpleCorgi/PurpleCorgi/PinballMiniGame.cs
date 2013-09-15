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
    class PinballMiniGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private int ball_radius = 16;

        const float unitToPixel = 500;
        const float pixelToUnit = 1 / unitToPixel;

        private MiniGameState gameState;

        private World physicsWorld;
        private Body wallLeft;
        private Body wallRight;
        private Body wallTop;
        private Body paddleLeft;
        private Body paddleRight;
        private Body ball;
        private Vector2 paddleSize;
        private Texture2D circleTexture;

        private Kinect ein;

        public PinballMiniGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            ein = new Kinect(0, 0);
            ein.Init();

            circleTexture = CreateCircle(ball_radius, graphicsDevice);

            gameState = MiniGameState.Initialized;

            physicsWorld = new World(new Vector2(0, .982f));
            paddleSize = new Vector2(300, 16);
            paddleLeft = BodyFactory.CreateRectangle(physicsWorld, paddleSize.X * 2 * pixelToUnit, paddleSize.Y * pixelToUnit, 1000f);
            paddleLeft.BodyType = BodyType.Dynamic;
            paddleLeft.Position = new Vector2(8 * pixelToUnit, (GameConstants.MiniGameCanvasHeight - 100) * pixelToUnit);
            paddleLeft.LocalCenter = new Vector2(0, 0);
            paddleLeft.SleepingAllowed = false;
            paddleLeft.Restitution = 1.0f;
            paddleLeft.IgnoreGravity = true;

 
            ball = BodyFactory.CreateCircle(physicsWorld, (ball_radius) * pixelToUnit, 1000f);
            ball.BodyType = BodyType.Dynamic;
            ball.Position = new Vector2(100 * pixelToUnit, 100 * pixelToUnit);
            ball.Restitution = 1.0f;
            ball.SleepingAllowed = false;
        }


        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Tab))
            {
                ball.Position = new Vector2(320 * pixelToUnit, 100 * pixelToUnit);
                ball.AngularVelocity = 0;
                ball.LinearVelocity = Vector2.Zero;
            }


            if (ks.IsKeyDown(Keys.A))
            {
                paddleRight.Rotation += 0.05f;
                paddleLeft.Rotation -= 0.05f;
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                paddleRight.Rotation -= 0.05f;
                paddleLeft.Rotation += 0.05f;
            }
            

            
            //paddle.Rotation -= (ein.RightHandAngle)/50.0f;

            //paddle.Rotation = -ein.RightHandAngle;
            /*
            if (Math.Abs(paddle.Rotation - (-1 * ein.RHRSAngle)) > 0.025f)
            {
                if (paddle.Rotation < (-1 * ein.RHRSAngle))
                {
                    paddle.Rotation += 0.051f;
                }
                else if (paddle.Rotation > (-1 * ein.RHRSAngle))
                {
                    paddle.Rotation -= 0.051f;
                }
            }
             * */

            // Simulate physics.
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.White);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // Render left and paddle
            sb.Draw(Game1.WhitePixel,
                paddleLeft.Position * unitToPixel,
                null, Color.Black,
                paddleLeft.Rotation,
                new Vector2(0f, .5f),
                paddleSize,
                SpriteEffects.None,
                0);
            sb.Draw(Game1.WhitePixel,
                paddleRight.Position * unitToPixel,
                null, Color.Black,
                paddleRight.Rotation,
                new Vector2(1f, .5f),
                paddleSize,
                SpriteEffects.None,
                0);
            sb.Draw(circleTexture, ball.Position * unitToPixel, null, Color.Green, ball.Rotation, new Vector2(ball_radius), new Vector2(1f), SpriteEffects.None, 0.0f);
            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
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

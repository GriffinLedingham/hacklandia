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
    class BrickMiniGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private int targetRadius = 30;
        private int ballRadius = 16;
        private int paddleRadius = 75;
        const float unitToPixel = 30;
        const float pixelToUnit = 1 / unitToPixel;
        bool win = false;
        bool lose = false;
        private MiniGameState gameState;

        private World physicsWorld;
        private Body wallLeft;
        private Body wallRight;
        private Body wallTop;
        private Body paddle;
        private Body paddleRight;
        private Body ball;
        private Texture2D circleTexture;
        private Texture2D paddleTexture;
        private Texture2D targetTexture;
        private List<Body> targets;

        private Kinect ein;
        public BrickMiniGame(GraphicsDevice graphicsDevice)
        {
            targets = new List<Body>();

            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            ein = new Kinect(0, 0);
            ein.Init();

            circleTexture = CreateCircle(ballRadius, graphicsDevice);
            paddleTexture = CreateCircle(paddleRadius, graphicsDevice);
            targetTexture = CreateCircle(targetRadius, graphicsDevice);

            gameState = MiniGameState.Initialized;

            physicsWorld = new World(new Vector2(0, 0f));
            Random rand = new Random(DateTime.Now.Ticks.GetHashCode());
            for (int i = 0; i < 2; i++)
            {
                Body body = BodyFactory.CreateCircle(physicsWorld, targetRadius * pixelToUnit, 1000f);
                body.BodyType = BodyType.Static;
                body.Position = new Vector2((float)(32 + rand.NextDouble() * (GameConstants.MiniGameCanvasWidth - 64)) * pixelToUnit,
                    (float)(32 + rand.NextDouble() * (GameConstants.MiniGameCanvasHeight - 150)) * pixelToUnit);
                targets.Add(body);
            }
            paddle = BodyFactory.CreateCircle(physicsWorld, paddleRadius * pixelToUnit, 1000f);
            paddle.BodyType = BodyType.Static;
            paddle.Position = new Vector2(GameConstants.MiniGameCanvasWidth / 2 * pixelToUnit, (GameConstants.MiniGameCanvasHeight+20) * pixelToUnit);
            paddle.SleepingAllowed = false;
            paddle.Restitution = 1.0f;
            paddle.IgnoreGravity = true;
            paddle.Mass = 10000000;
            paddle.Friction = 0;

            wallLeft = BodyFactory.CreateRectangle(physicsWorld, 16 * pixelToUnit, GameConstants.MiniGameCanvasHeight * pixelToUnit, 1000f);
            wallLeft.BodyType = BodyType.Static;
            wallLeft.Position = new Vector2(0, GameConstants.MiniGameCanvasHeight / 2f * pixelToUnit);
            wallLeft.SleepingAllowed = false;
            wallLeft.Restitution = 1.0f;
            wallLeft.Friction = 0;

            wallRight = BodyFactory.CreateRectangle(physicsWorld, 16 * pixelToUnit, GameConstants.MiniGameCanvasHeight * pixelToUnit, 1000f);
            wallRight.BodyType = BodyType.Static;
            wallRight.Position = new Vector2(GameConstants.MiniGameCanvasWidth * pixelToUnit, GameConstants.MiniGameCanvasHeight / 2f * pixelToUnit);
            wallRight.SleepingAllowed = false;
            wallRight.Restitution = 1.0f;
            wallRight.Friction = 0;

            wallTop = BodyFactory.CreateRectangle(physicsWorld, GameConstants.MiniGameCanvasWidth * pixelToUnit, 16 * pixelToUnit, 1000f);
            wallTop.BodyType = BodyType.Static;
            wallTop.Position = new Vector2(GameConstants.MiniGameCanvasWidth / 2 * pixelToUnit, 0);
            wallTop.SleepingAllowed = false;
            wallTop.Restitution = 1.0f;
            wallTop.Friction = 0;


            ball = BodyFactory.CreateCircle(physicsWorld, (ballRadius) * pixelToUnit, 1000f);
            ball.BodyType = BodyType.Dynamic;
            ball.Position = new Vector2(GameConstants.MiniGameCanvasWidth / 2 * pixelToUnit, (GameConstants.MiniGameCanvasHeight - 100) * pixelToUnit);
            ball.Restitution = 1.0f;
            ball.Mass = 200;
            ball.SleepingAllowed = false;
            ball.AngularDamping = 1.0f;
            ball.FixedRotation = true;
            ball.Friction = 0;
            float speed = 8;
            float theta = (float)(Math.PI / 4 + new Random().NextDouble() * Math.PI / 2);
            float x = (float)Math.Cos(theta) * speed;
            float y = (float)-Math.Sin(theta) * speed;
            ball.LinearVelocity = new Vector2(x, y);
        }

        private bool ball_OnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if(targets.Contains(f2.Body)){
                targets.Remove(f2.Body);
                if (targets.Count == 0)
                {
                    win = true;
                }
                physicsWorld.RemoveBody(f2.Body);
                return true;
            }
            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            KeyboardState ks = Keyboard.GetState();

            
            ball.OnCollision +=ball_OnCollision;
            if (ks.IsKeyDown(Keys.Tab))
            {
                ball.Position = new Vector2(320 * pixelToUnit, 100 * pixelToUnit);
                ball.AngularVelocity = 0;
                ball.LinearVelocity = Vector2.Zero;
            }

            float x = (float)(ein.UserX / .5) * (GameConstants.MiniGameCanvasWidth/2.0f) + (GameConstants.MiniGameCanvasWidth) / 2;
            paddle.Position = new Vector2(x * pixelToUnit, paddle.Position.Y);

            if (!win && ball.Position.Y*unitToPixel >= GameConstants.MiniGameCanvasHeight)
            {
                lose = true;
            }
            
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
                wallTop.Position * unitToPixel,
                null, Color.Red,
                wallTop.Rotation,
                new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f),
                new Vector2(GameConstants.MiniGameCanvasWidth, 16),
                SpriteEffects.None,
                0);

            sb.Draw(Game1.WhitePixel,
                wallLeft.Position * unitToPixel,
                null, Color.Red,
                wallLeft.Rotation,
                new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f),
                new Vector2(16, GameConstants.MiniGameCanvasHeight),
                SpriteEffects.None,
                0);

            sb.Draw(Game1.WhitePixel,
                wallRight.Position * unitToPixel,
                null, Color.Red,
                wallRight.Rotation,
                new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f),
                new Vector2(16, GameConstants.MiniGameCanvasHeight),
                SpriteEffects.None,
                0);
            sb.Draw(paddleTexture, paddle.Position * unitToPixel, null, Color.Black, paddle.Rotation, new Vector2(paddleRadius), new Vector2(1f), SpriteEffects.None, 0.0f);
            sb.Draw(circleTexture, ball.Position * unitToPixel, null, Color.Green, ball.Rotation, new Vector2(ballRadius), new Vector2(1f), SpriteEffects.None, 0.0f);
            foreach(Body target in targets){
                sb.Draw(targetTexture, target.Position * unitToPixel, null, Color.Red, target.Rotation, new Vector2(targetRadius), new Vector2(1f), SpriteEffects.None, 0.0f);
            }
            sb.End();
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

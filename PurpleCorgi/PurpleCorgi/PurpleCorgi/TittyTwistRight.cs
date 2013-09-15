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
    class TittyTwistRight : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        Body body, body2, body3;

        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        World world;
        Texture2D PlainTexture;

        Texture2D PlainTextureLeft, PlainTextureRight;

        Vector2 Size, size, Size2, size2, Size3, size3;

        bool win, lose = false;

        private bool prizeGet = false;

        float height, width, density, posx, posy;
        float height2, width2, density2, posx2, posy2;
        float height3, width3, density3, posx3, posy3;

        bool leftGripped, rightGripped = false;


        float gravity;

        private Kinect ein;

        private MiniGameState gameState;

        private Random random;

        float winTimer = 0.0f;

        private bool colliding = false;

        private int score = 0;

        public static bool ShowedTutorial = false;
        private float tutorialTimer;
        private const float tutorialDuration = 1000f;


        public TittyTwistRight(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            random = new Random();

            ein = new Kinect(100, 100);
            ein.Init();

            height = 50;
            width = 50;
            density = 1f;
            posx = 400;
            posy = 50;

            height2 = 50;
            width2 = 50;
            density2 = 1f;
            posx2 = 200;
            posy2 = 50;

            height3 = 75;
            width3 = 75;
            density3 = 1f;
            posx3 = 200;
            posy3 = 50;

            gravity = 0f;

            

            // TODO: Add your initialization logic here
            world = new World(new Vector2(0, gravity));

            PlainTexture = new Texture2D(this.graphicsDevice, 1, 1);//Game1.peak_Sprite;
            PlainTexture.SetData(new[] { Color.Yellow });

            PlainTextureLeft = new Texture2D(this.graphicsDevice, 1, 1);
            PlainTextureLeft.SetData(new[] { Color.White });

            PlainTextureRight = new Texture2D(this.graphicsDevice, 1, 1);
            PlainTextureRight.SetData(new[] { Color.White });

            body = BodyFactory.CreateRectangle(world, width * pixelToUnit, height * pixelToUnit, density);
            body.BodyType = BodyType.Static;
            size = new Vector2(width, height);
            Size = size;
            body.Position = new Vector2(posx * pixelToUnit, posy * pixelToUnit);
            body.SleepingAllowed = false;

            body2 = BodyFactory.CreateRectangle(world, width2 * pixelToUnit, height2 * pixelToUnit, density2);
            body2.BodyType = BodyType.Static;
            size2 = new Vector2(width2, height2);
            Size2 = size2;
            body2.Position = new Vector2(random.Next(1, GameConstants.MiniGameCanvasWidth) * pixelToUnit, random.Next(1, GameConstants.MiniGameCanvasHeight) * pixelToUnit);
            body2.SleepingAllowed = false;

            body3 = BodyFactory.CreateRectangle(world, width3 * pixelToUnit, height3 * pixelToUnit, density2);
            body3.BodyType = BodyType.Static;
            size3 = new Vector2(width3, height3);
            Size3 = size3;
            body3.Position = new Vector2(random.Next(1, GameConstants.MiniGameCanvasWidth) * pixelToUnit, random.Next(1, GameConstants.MiniGameCanvasHeight) * pixelToUnit);
            body3.SleepingAllowed = false;

            
        }



        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            winTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (winTimer > 10000 && !win)
            {
                lose = true;
            }

            if (score >= 3)
            {
                win = true;
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


            if (prizeGet)
            {
                body3.Position = new Vector2(random.Next(1, GameConstants.MiniGameCanvasWidth) * pixelToUnit, random.Next(1, GameConstants.MiniGameCanvasHeight) * pixelToUnit);
                prizeGet = false;
                score++;
            }

            //Console.WriteLine(prizeGet);

            /*if (body.Position.X > body3.Position.X - ((width3 / 2) * pixelToUnit) && body.Position.X < (body3.Position.X + ((width3 / 2) * pixelToUnit)) && body.Position.Y > body3.Position.Y - ((height3 / 2) * pixelToUnit) && body.Position.Y < body3.Position.Y + ((height3 / 2) * pixelToUnit))
            {
                if (leftGripped)
                    prizeGet = true;
            }*/

            if (body2.Position.X > body3.Position.X - ((width3 / 2) * pixelToUnit) && body2.Position.X < (body3.Position.X + ((width3 / 2) * pixelToUnit)) && body2.Position.Y > body3.Position.Y - ((height3 / 2) * pixelToUnit) && body2.Position.Y < body3.Position.Y + ((height3 / 2) * pixelToUnit))
            {
                if (rightGripped)
                    prizeGet = true;
            }

            /*if (ein.LeftHand.Gripped)
            {
                PlainTextureLeft.SetData(new[] { Color.Red });
                leftGripped = true;
            }
            if (ein.LeftHand.Released)
            {
                PlainTextureLeft.SetData(new[] { Color.White });
                leftGripped = false;
            }*/

            if (ein.RightHand.Gripped)
            {
                PlainTextureRight.SetData(new[] { Color.Red });
                rightGripped = true;
            }
            if (ein.RightHand.Released)
            {
                PlainTextureRight.SetData(new[] { Color.White });
                rightGripped = false;
            }

            //if(ein.LeftHand.Active)
                //body.Position = new Vector2(((ein.LeftHand.Pos.X+.5f) * GameConstants.MiniGameCanvasWidth/2 ) * pixelToUnit, ein.LeftHand.Pos.Y * GameConstants.MiniGameCanvasHeight * pixelToUnit);
            if(ein.RightHand.Active)
                body2.Position = new Vector2((ein.RightHand.Pos.X/2 * GameConstants.MiniGameCanvasWidth/2 + GameConstants.MiniGameCanvasWidth/2) * pixelToUnit, ein.RightHand.Pos.Y * GameConstants.MiniGameCanvasHeight * pixelToUnit);



            //Console.WriteLine(ein.LeftHand.Pos.Y);
            // Simulate physics.
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 scale = new Vector2(Size.X / (float)PlainTexture.Width, Size.Y / (float)PlainTexture.Height);
            Vector2 scale2 = new Vector2(Size2.X / (float)PlainTexture.Width, Size2.Y / (float)PlainTexture.Height);
            Vector2 scale3 = new Vector2(Size3.X / (float)PlainTexture.Width, Size3.Y / (float)PlainTexture.Height);

            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);


            if (!ShowedTutorial)
            {
                sb.Draw(Game1.tutorialFrames, new Vector2(40, 10), new Rectangle((((int)(tutorialTimer / 300f) % 2) * 300) + 600, 300, 300, 300), Color.White);
            }
            // TODO: Add your drawing code here

            //sb.Draw(PlainTextureLeft, body.Position * unitToPixel, null, Color.White, body.Rotation, new Vector2(PlainTexture.Width / 2.0f, PlainTexture.Height / 2.0f), scale, SpriteEffects.None, 0);
            //sb.Draw(PlainTextureRight, body2.Position * unitToPixel, null, Color.White, body2.Rotation, new Vector2(PlainTexture.Width / 2.0f, PlainTexture.Height / 2.0f), scale2, SpriteEffects.None, 0);
            sb.Draw(PlainTexture, body3.Position * unitToPixel, null, Color.White, body3.Rotation, new Vector2(PlainTexture.Width / 2.0f, PlainTexture.Height / 2.0f), scale3, SpriteEffects.None, 0);

            sb.Draw(Game1.handCursor, new Rectangle((int)(body2.Position.X * unitToPixel) - 25, (int)(body2.Position.Y * unitToPixel) - 25, 50, 50), new Rectangle((rightGripped ? 150 : 0), 0, 150, 150), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.5f);

            sb.End();
            if(win)
                graphicsDevice.Clear(Color.Red);

            if(lose)
                graphicsDevice.Clear(Color.Black);


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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;

namespace PurpleCorgi
{
    class TestMiniGriff : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private float redBoxSpeed = 0.1f;

        private Vector2 redBoxPosition;
        private Color backgroundColor;

        private MiniGameState gameState;

        private World world;
        private float gravity;
        private float height, width, density, posx, posy, floor_width, floor_height;

        private float rod_height, rod_width, rod_density, rod_posx, rod_posy;
        private float rodtray_height, rodtray_width, rodtray_density, rodtray_posx, rodtray_posy;
        private float fixpt_height, fixpt_width, fixpt_density, fixpt_posx, fixpt_posy;

        private Vector2 size, Size, rod_size, rod_Size, rodtray_size, rodtray_Size;



        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;


        private Body body, rod_body, rodtray_body, fixpt_body;
        private Joint playerRod;

        public TestMiniGriff(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            height = 50;
            width = 50;
            density = 1f;
            posx = 350;
            posy = 0;

            rod_height = 100;
            rod_width = 20;
            rod_density = 1f;
            rod_posx = 350;
            rod_posy = 150;

            rodtray_height = 20;
            rodtray_width = 200;
            rodtray_density = .5f;
            rodtray_posx = 350;
            rodtray_posy = 150;

            fixpt_height = 1;
            fixpt_width = 1;
            fixpt_density = 1f;

            sb = new SpriteBatch(graphicsDevice);

            

            redBoxPosition = new Vector2(Game1.GameRandom.Next() % (GameConstants.MiniGameCanvasWidth - 16), Game1.GameRandom.Next() % (GameConstants.MiniGameCanvasHeight - 16));
            backgroundColor = new Color((float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), 1);
            gravity = 9.8f;
            world = new World(new Vector2(0, gravity));

            body = BodyFactory.CreateRectangle(world, width * pixelToUnit, height * pixelToUnit, density);
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;

            size = new Vector2(width, height);
            Size = size;
            body.Position = new Vector2(posx * pixelToUnit, posy * pixelToUnit);

            /*rod_body = BodyFactory.CreateRectangle(world, rod_width * pixelToUnit, rod_height * pixelToUnit, rod_density);
            rod_body.BodyType = BodyType.Dynamic;
            rod_size = new Vector2(rod_width, rod_height);
            rod_Size = rod_size;
            rod_body.Position = new Vector2(rod_posx * pixelToUnit, rod_posy* pixelToUnit);*/

            rodtray_body = BodyFactory.CreateRectangle(world, rodtray_width * pixelToUnit, rodtray_height * pixelToUnit, rodtray_density);
            rodtray_body.BodyType = BodyType.Static;
            rodtray_size = new Vector2(rodtray_width, rodtray_height);
            rodtray_Size = rodtray_size;
            rodtray_body.Position = new Vector2(rodtray_posx * pixelToUnit, rodtray_posy * pixelToUnit);

            fixpt_body = BodyFactory.CreateRectangle(world, fixpt_width * pixelToUnit, fixpt_height * pixelToUnit, fixpt_density);
            fixpt_body.BodyType = BodyType.Static;
            fixpt_body.Position = new Vector2(rodtray_posx * pixelToUnit, rod_posy+rod_height/2 * pixelToUnit);

            //playerRod = JointFactory.CreateWeldJoint(world, rod_body, rodtray_body, Vector2.Zero, new Vector2(0, rod_height/2 * pixelToUnit));
            //FarseerPhysics.Factories.JointFactory.CreateRevoluteJoint(world, rod_body, rodtray_body, new Vector2(0, rod_height / 2 * pixelToUnit));


            gameState = MiniGameState.Initialized;
        }

        public void Update(GameTime GameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            Vector2 force1 = new Vector2(0,0);
            float forcePower = 0f;

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A) )
                    forcePower = -1f/100f;//force1 += new Vector2(-forcePower, 0);
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                    forcePower = 1f/100f;//force1 += new Vector2(forcePower, 0);

            rodtray_body.Rotation += (forcePower);
            //rod_body.ApplyForce(new Vector2(0, -4.7f));
            //rod_body.ApplyTorque(10.0f);
            //rodtray_body.ApplyForce(force1,new Vector2(rodtray_width/2,0));

            world.Step((float)GameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            Vector2 scale = new Vector2(Size.X / (float)Game1.WhitePixel.Width, Size.Y / (float)Game1.WhitePixel.Height);
            Vector2 rod_scale = new Vector2(rod_Size.X / (float)Game1.WhitePixel.Width, rod_Size.Y / (float)Game1.WhitePixel.Height);
            Vector2 rodtray_scale = new Vector2(rodtray_Size.X / (float)Game1.WhitePixel.Width, rodtray_Size.Y / (float)Game1.WhitePixel.Height);


            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(backgroundColor);
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.Draw(Game1.WhitePixel, redBoxPosition, null, Color.Red, 0.0f, Vector2.Zero, new Vector2(16), SpriteEffects.None, 0.0f);
            sb.Draw(Game1.WhitePixel, body.Position * unitToPixel, null, Color.White, body.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), scale, SpriteEffects.None, 0);
            sb.Draw(Game1.WhitePixel, rodtray_body.Position * unitToPixel, null, Color.White, rodtray_body.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), rodtray_scale, SpriteEffects.None, 0);            
            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

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
    class PlatformerGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private float redBoxSpeed = 0.1f;

        private Vector2 redBoxPosition;
        private Color backgroundColor;

        private MiniGameState gameState;

        private World world;
        private float gravity;
        private float height, width, density, posx, posy;

        private float rod_height, rod_width, rod_density, rod_posx, rod_posy;
        private float floor_height, floor_width, floor_density, floor_posx, floor_posy;
        private float fixpt_height, fixpt_width, fixpt_density, fixpt_posx, fixpt_posy;

        private Vector2 size, Size, rod_size, rod_Size, floor_size, floor_Size;



        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;


        Body body, rod_body, floor_body, fixpt_body;

        public PlatformerGame(GraphicsDevice graphicsDevice)
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

            floor_height = 20;
            floor_width = GameConstants.MiniGameCanvasWidth;
            floor_density = .5f;
            floor_posx = GameConstants.MiniGameCanvasWidth/2;
            floor_posy = (GameConstants.MiniGameCanvasHeight - 50);

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

            floor_body = BodyFactory.CreateRectangle(world, floor_width * pixelToUnit, floor_height * pixelToUnit, floor_density);
            floor_body.BodyType = BodyType.Static;
            floor_size = new Vector2(floor_width, floor_height);
            floor_Size = floor_size;
            floor_body.Position = new Vector2(floor_posx * pixelToUnit, floor_posy * pixelToUnit);

            //playerRod = JointFactory.CreateWeldJoint(world, rod_body, floor_body, Vector2.Zero, new Vector2(0, rod_height/2 * pixelToUnit));
            //FarseerPhysics.Factories.JointFactory.CreateRevoluteJoint(world, rod_body, floor_body, new Vector2(0, rod_height / 2 * pixelToUnit));


            gameState = MiniGameState.Initialized;
        }

        public void Update(GameTime GameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            Vector2 force1 = new Vector2(0, 0);
            float forcePower = 0f;

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up))
                force1 += new Vector2(0, -forcePower);
            //rod_body.ApplyForce(new Vector2(0, -4.7f));
            //rod_body.ApplyTorque(10.0f);
            //floor_body.ApplyForce(force1,new Vector2(floor_width/2,0));

            body.ApplyForce(force1);

            world.Step((float)GameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            Vector2 scale = new Vector2(Size.X / (float)Game1.WhitePixel.Width, Size.Y / (float)Game1.WhitePixel.Height);
            Vector2 rod_scale = new Vector2(rod_Size.X / (float)Game1.WhitePixel.Width, rod_Size.Y / (float)Game1.WhitePixel.Height);
            Vector2 floor_scale = new Vector2(floor_Size.X / (float)Game1.WhitePixel.Width, floor_Size.Y / (float)Game1.WhitePixel.Height);


            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(backgroundColor);
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.Draw(Game1.WhitePixel, redBoxPosition, null, Color.Red, 0.0f, Vector2.Zero, new Vector2(16), SpriteEffects.None, 0.0f);
            sb.Draw(Game1.WhitePixel, body.Position * unitToPixel, null, Color.White, body.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), scale, SpriteEffects.None, 0);
            sb.Draw(Game1.WhitePixel, floor_body.Position * unitToPixel, null, Color.White, floor_body.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), floor_scale, SpriteEffects.None, 0);
            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

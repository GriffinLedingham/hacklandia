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

        Body body;

        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        World world;
        Texture2D PlainTexture;

        Vector2 Size, size;


        float height, width, density, posx, posy;

        float gravity;

        private Kinect ein;

        private MiniGameState gameState;

        public Kardashian(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            ein = new Kinect(100, 100);
            ein.Init();

            height = 50;
            width = 50;
            density = 1f;
            posx = 200;
            posy = 50;

            gravity = 0f;

            // TODO: Add your initialization logic here
            world = new World(new Vector2(0, gravity));

            PlainTexture = new Texture2D(this.graphicsDevice, 1, 1);
            PlainTexture.SetData(new[] { Color.White });

            body = BodyFactory.CreateRectangle(world, width * pixelToUnit, height * pixelToUnit, density);
            body.BodyType = BodyType.Dynamic;
            size = new Vector2(width, height);
            Size = size;
            body.Position = new Vector2(posx * pixelToUnit, posy * pixelToUnit);
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            /*if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W))
                body.Position += new Vector2(0,.1f);
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                body.Position += new Vector2(.1f, 0);
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
                body.Position -= new Vector2(0, .1f);
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                body.Position -= new Vector2(.1f, 0);*/




            //paddle.Rotation -= (ein.RightHandAngle)/50.0f;

            //paddle.Rotation = -ein.RightHandAngle;
            body.Position = new Vector2((((ein.UserX / .5f) * GameConstants.MiniGameCanvasWidth / 2) + GameConstants.MiniGameCanvasWidth/2) * pixelToUnit, 
                (((ein.UserZ -.9f) * GameConstants.MiniGameCanvasHeight / 2)) * pixelToUnit);

            Console.WriteLine(ein.UserZ);

            // Simulate physics.
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 scale = new Vector2(Size.X / (float)PlainTexture.Width, Size.Y / (float)PlainTexture.Height);
            
            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // TODO: Add your drawing code here

            sb.Draw(PlainTexture, body.Position * unitToPixel, null, Color.White, body.Rotation, new Vector2(PlainTexture.Width / 2.0f, PlainTexture.Height / 2.0f), scale, SpriteEffects.None, 0);
            
            sb.End();


        }

        public MiniGameState GetState()
        {
            return gameState;
        }

       
    }
}

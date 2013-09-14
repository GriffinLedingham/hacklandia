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

        Vector2 Size, size, SizeFloor, sizeFloor;


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
            posx = 5;
            posy = 5;

            gravity = 9.8f;

            // TODO: Add your initialization logic here
            world = new World(new Vector2(0, gravity));

            PlainTexture = new Texture2D(this.graphicsDevice, 1, 1);
            PlainTexture.SetData(new[] { Color.White });

            body = BodyFactory.CreateRectangle(world, width * pixelToUnit, height * pixelToUnit, density);
            body.BodyType = BodyType.Dynamic;
            size = new Vector2(width, height);
            Size = size;
            body.Position = new Vector2(200, 50);
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            KeyboardState ks = Keyboard.GetState();





            //paddle.Rotation -= (ein.RightHandAngle)/50.0f;

            //paddle.Rotation = -ein.RightHandAngle;
            /*if (Math.Abs(paddle.Rotation - (-1 * ein.RPRKAngle)) > 0.025f)
            {


                if (paddle.Rotation + .9 < (-1 * ein.RPRKAngle))
                {
                    paddle.Rotation += 0.051f;
                }
                else if (paddle.Rotation + .9 > (-1 * ein.RPRKAngle))
                {
                    paddle.Rotation -= 0.051f;
                }
            }*/

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

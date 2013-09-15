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
    class ColorCallMiniGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private int targetRadius = 30;
        private int ballRadius = 16;
        private int paddleRadius = 75;
        const float unitToPixel = 30;
        const float pixelToUnit = 1 / unitToPixel;

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
        public ColorCallMiniGame(GraphicsDevice graphicsDevice)
        {
           
            sb = new SpriteBatch(graphicsDevice);

            ein = new Kinect(0, 0);
            ein.Init();

            gameState = MiniGameState.Initialized;
        }



        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            KeyboardState ks = Keyboard.GetState();

        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.White);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // Render left and paddle

            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

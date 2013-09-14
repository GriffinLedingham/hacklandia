using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Corgie;

namespace PurpleCorgi
{
    class PointerGame : MiniGame
    {
        private MiniGameState gameState = MiniGameState.Initialized;

        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private Kinect ein;

        private Color bgColor = Color.Teal;
        
        private Vector2 pointerPosition;

        public PointerGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            sb = new SpriteBatch(graphicsDevice);

            pointerPosition = new Vector2(160);

            ein = new Kinect(0, 0);
            ein.Init();
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            if (ein.Hand != null)
            {
                if (ein.Hand.X != 0)
                    Console.WriteLine("here");

                pointerPosition.X = ein.Hand.X * 200;
                pointerPosition.Y = ein.Hand.Y * 200 * -1;
            }
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(bgColor);

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.Draw(Game1.WhitePixel, pointerPosition, null, Color.Orange, 0.0f, Vector2.Zero, new Vector2(16), SpriteEffects.None, 0.0f);
            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

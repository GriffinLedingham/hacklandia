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

        public static int Difficulty = 0;

        public PointerGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            sb = new SpriteBatch(graphicsDevice);

            pointerPosition = new Vector2(GameConstants.MiniGameCanvasWidth, GameConstants.MiniGameCanvasHeight);

            ein = Game1.ein;
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }


                pointerPosition.X = ein.RightHand.Pos.X * 640;
                pointerPosition.Y = ein.RightHand.Pos.Y * 360;

                if (pointerPosition.X > 300 && pointerPosition.X < 468 && pointerPosition.Y > 400 && pointerPosition.Y < 562)
                {
                    Difficulty = 1;
                }

                if (pointerPosition.X > 600 && pointerPosition.X < 768 && pointerPosition.Y > 400 && pointerPosition.Y < 562)
                {
                    Difficulty = 2;
                }

                if (pointerPosition.X > 900 && pointerPosition.X < 1068 && pointerPosition.Y > 400 && pointerPosition.Y < 562)
                {
                    Difficulty = 3;
                }
        }
        public void Nuke()
        {
            ein.Nuke();
        }
        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(bgColor);

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            sb.Draw(Game1.WhitePixel, new Vector2(300,400), null, Color.White, 0.0f, Vector2.Zero, new Vector2(200), SpriteEffects.None, 0.0f);
            sb.Draw(Game1.WhitePixel, new Vector2(600, 400), null, Color.White, 0.0f, Vector2.Zero, new Vector2(200), SpriteEffects.None, 0.0f);
            sb.Draw(Game1.WhitePixel, new Vector2(900, 400), null, Color.White, 0.0f, Vector2.Zero, new Vector2(200), SpriteEffects.None, 0.0f);

            sb.Draw(Game1.WhitePixel, pointerPosition, null, Color.Black, 0.0f, Vector2.Zero, new Vector2(32), SpriteEffects.None, 0.0f);

            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

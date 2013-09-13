using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleCorgi
{
    class TestMiniGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private float redBoxSpeed = 0.1f;

        private Vector2 redBoxPosition;
        private Color backgroundColor;

        private MiniGameState gameState;

        public TestMiniGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            sb = new SpriteBatch(graphicsDevice);

            redBoxPosition = new Vector2(Game1.GameRandom.Next() % (GameConstants.MiniGameCanvasWidth - 16), Game1.GameRandom.Next() % (GameConstants.MiniGameCanvasHeight - 16));
            backgroundColor = new Color((float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), 1);

            gameState = MiniGameState.Initialized;
        }

        public void Update(GameTime GameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Right))
            {
                redBoxPosition.X += redBoxSpeed * GameTime.ElapsedGameTime.Milliseconds;
            }
            else if (ks.IsKeyDown(Keys.Left))
            {
                redBoxPosition.X -= redBoxSpeed * GameTime.ElapsedGameTime.Milliseconds;
            }

            if (ks.IsKeyDown(Keys.Down))
            {
                redBoxPosition.Y += redBoxSpeed * GameTime.ElapsedGameTime.Milliseconds;
            }
            else if (ks.IsKeyDown(Keys.Up))
            {
                redBoxPosition.Y -= redBoxSpeed * GameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(backgroundColor);
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.Draw(Game1.WhitePixel, redBoxPosition, null, Color.Red, 0.0f, Vector2.Zero, new Vector2(16), SpriteEffects.None, 0.0f);
            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

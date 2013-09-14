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
    class SpaceGame : MiniGame
    {
        private MiniGameState gameState = MiniGameState.Initialized;

        private GraphicsDevice graphicsDevice;

        public SpaceGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(Color.Teal);
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

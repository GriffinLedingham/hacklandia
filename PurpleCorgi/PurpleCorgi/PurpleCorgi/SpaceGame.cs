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
        private SpriteBatch sb;

        private Kinect ein;

        private Color bgColor = Color.Black;
        private Star[] stars;

        private struct Star
        {
            public Vector2 position;
            public float timePassed;
            public float durationOnScreen;

            public Star(float timePassed)
            {
                position = new Vector2(Game1.GameRandom.Next() % GameConstants.GameResolutionWidth, 0);
                this.timePassed = timePassed;
                durationOnScreen = 3000 + (float)(Game1.GameRandom.NextDouble() * 700 - 350);
            }

            public void Update(GameTime gameTime)
            {
                timePassed += gameTime.ElapsedGameTime.Milliseconds;

                if (timePassed > durationOnScreen)
                {
                    position.X = Game1.GameRandom.Next() % GameConstants.GameResolutionWidth;
                    timePassed = 0;
                    durationOnScreen = 3000 + (float)(Game1.GameRandom.NextDouble() * 700 - 350);
                }
            }
        }

        public SpaceGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            sb = new SpriteBatch(graphicsDevice);

            stars = new Star[100];
            for (int i = 0; i < 100; i++)
            {
                stars[i] = new Star(Game1.GameRandom.Next() % 3000);
            }

            ein = new Kinect(0, 0);
            ein.Init();
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            for (int i = 0; i < 100; i++)
            {
                stars[i].Update(gameTime);
            }

#if CHANGE_BG_COLOR
            {
                string lastColor = ein.LastColor;
                if (!string.IsNullOrEmpty(lastColor))
                {
                    switch (lastColor)
                    {
                        case "RED": bgColor = Color.Red; break;
                        case "GREEN": bgColor = Color.Green; break;
                        case "YELLOW": bgColor = Color.Yellow; break;
                        case "BLUE": bgColor = Color.Blue; break;
                        default:
                            break;
                    }
                }
            }
#endif
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(bgColor);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int i = 0; i < 100; i++)
            {
                sb.Draw(Game1.WhitePixel, new Vector2(stars[i].position.X, (GameConstants.GameResolutionHeight + 300) * (stars[i].timePassed / stars[i].durationOnScreen)), Color.White);
            }
            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

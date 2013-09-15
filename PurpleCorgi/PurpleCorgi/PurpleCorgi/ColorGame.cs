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
    class ColorGame : MiniGame
    {
        private MiniGameState gameState = MiniGameState.Initialized;
        private bool win = false;
        private bool lose = false;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;
        private string currentColor;
        private Color renderColor;
        private Kinect ein;
        private int correct = 0;
        float time = 0;
        private Color bgColor = Color.Black;
        private List<string> colorsStrings = new List<string> {"RED", "BLUE", "GREEN", "YELLOW"};
        private List<Color> colors = new List<Color> {Color.Red, Color.Blue, Color.Green, Color.Yellow};
        private int colorIndex = 0;
        private Vector2 renderPosition;

        public static bool ShowedTutorial = false;
        private float tutorialTimer;
        private const float tutorialDuration = 1000f;
        public ColorGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            sb = new SpriteBatch(graphicsDevice);

            ein = new Kinect(0, 0);
            ein.Init();
            resetDisplay();
        }


        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            } 
            if (!ShowedTutorial)
            {
                tutorialTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (tutorialTimer > tutorialDuration)
                {
                    ShowedTutorial = true;
                }

                return;
            }
            string col = ein.LastColor;
            if (!String.IsNullOrEmpty(col))
            {
                //Check if the text version of the rendered color is said.
                if (col.Equals(colorsStrings[colorIndex]))
                {
                    correct++;
                    resetDisplay();
                    if (correct == 15)
                    {
                        win = true;
                        ein.LastColor = string.Empty;
                    }
                }
                else
                {
                    lose = true;
                }
                
            }
            time -= gameTime.ElapsedGameTime.Milliseconds;
            if (time <= 0)
            {
                lose = true;
            }
        }


        private void resetDisplay()
        {
            Random rand = new Random(DateTime.Now.Ticks.GetHashCode());
            int bgIndex = rand.Next(colors.Count);

            while (colorIndex == bgIndex)
            {
                colorIndex = rand.Next(colorsStrings.Count);
            }
            currentColor = colorsStrings[rand.Next(colorsStrings.Count)];
            renderColor = colors[colorIndex];
            bgColor = colors[bgIndex];
            renderPosition = new Vector2(rand.Next(64, 400), rand.Next(64, 260));
            time = 4000;
        }
        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(bgColor);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            sb.DrawString(Game1.SegoeUIMono24, ((int)Math.Ceiling((float)time/1000)).ToString(), new Vector2(16), Color.Black);
            sb.DrawString(Game1.SegoeUIMono72, currentColor, renderPosition, renderColor);
            if (!ShowedTutorial)
            {
                sb.Draw(Game1.tutorialFrames, new Vector2(40, 10), new Rectangle(((int)(tutorialTimer / 300f) % 2) * 300, 0, 300, 300), Color.White);
            }
            sb.End();
        }

        public MiniGameState GetState()
        {
            if (win)
                return MiniGameState.Win;
            else if (lose)
                return MiniGameState.Lose;
            else
                return MiniGameState.Running;
        }
    }
}

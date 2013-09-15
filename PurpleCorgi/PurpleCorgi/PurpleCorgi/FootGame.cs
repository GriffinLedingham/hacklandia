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
using FarseerPhysics.Dynamics.Contacts;
using Corgie;

namespace PurpleCorgi
{
    class FootGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private float redBoxSpeed = 0.1f;

        private Vector2 redBoxPosition;
        private Color backgroundColor;

        private MiniGameState gameState;

        public static bool ShowedTutorial = false;
        private float tutorialTimer;
        private const float tutorialDuration = 1000f;
        
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        private Kinect ein;

        private Corgi2 lastForearm = null;

        private bool footInProgress = false;
        private string foot = "";

        float initialTime, holdUpTime = 0.0f;

        bool win, lose = false;

        Random random;

        public FootGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ein = new Kinect(100, 100);
            ein.Init();

            if ((new Random()).Next() % 2 == 0)
            {
                foot = "left";
            }
            else
            {
                foot = "right";
            }


            sb = new SpriteBatch(graphicsDevice);


            backgroundColor = new Color((float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), 1);
            
            gameState = MiniGameState.Initialized;
        }

        public void Update(GameTime gameTime)
        {

            //3 seconds to lift foot
            //hold left foot up

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

            

            if (!ein.LeftLegRaised && !ein.RightLegRaised || (foot == "right" && !ein.RightLegRaised && ein.LeftLegRaised) || (foot == "left" && ein.RightLegRaised && !ein.LeftLegRaised))
            {
                holdUpTime = 0.0f;
                initialTime += gameTime.ElapsedGameTime.Milliseconds;
                if (footInProgress)
                    lose = true;
            }
            else
            {
                initialTime = 0.0f;
                holdUpTime += gameTime.ElapsedGameTime.Milliseconds;
                footInProgress = true;
            }

            if (initialTime > 5000)
            {
                lose = true;
            }

            if (holdUpTime > 5000)
            {
                win = true;
            }

                
        }

        public void Render(RenderTarget2D canvas)
        {
            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(backgroundColor);
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.DrawString(Game1.SegoeUIMono24, "Lift " + foot + "for "+ Math.Ceiling(5-holdUpTime) +" seconds", new Vector2(100, 100), Color.Black);
            if (!ShowedTutorial)
            {
                sb.Draw(Game1.tutorialFrames, new Vector2(40, 10), new Rectangle((((int)(tutorialTimer / 300f) % 2) * 300) + 600, 900, 300, 300), Color.White);
            }
            sb.End();

            if (win)
            {
                graphicsDevice.Clear(Color.Red);
            }

            if (lose)
            {
                graphicsDevice.Clear(Color.Black);
            }
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

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

        
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        private Kinect ein;

        private Corgi2 lastForearm = null;



        public FootGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ein = new Kinect(100, 100);
            ein.Init();


            sb = new SpriteBatch(graphicsDevice);


            backgroundColor = new Color((float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), 1);
            
            gameState = MiniGameState.Initialized;
        }

        public void Update(GameTime GameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            
            if (lastForearm != null)
            {
                if ((ein.LHLEVector.Y - lastForearm.Y) > .05f)
                {

                }
            }
            lastForearm = ein.LHLEVector;

            
        }

        public void Render(RenderTarget2D canvas)
        {
           

            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(backgroundColor);
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.End();
        }


        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

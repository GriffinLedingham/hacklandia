using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleCorgi
{
    /// <summary>
    /// Determines the state of the minigame.
    /// </summary>
    public enum MiniGameState
    {
        /// <summary>
        /// MiniGame is created, but not initiating its gameloop
        /// </summary>
        Initialized,

        /// <summary>
        /// MiniGame is running, should be calling update(), and render() every tick
        /// </summary>
        Running,

        /// <summary>
        /// MiniGame has ended and the player probably messed up
        /// </summary>
        Lose,

        /// <summary>
        /// MiniGame has ended and the player probably did something awesome
        /// </summary>
        Win,
    }


    interface MiniGame
    {
        /// <summary>
        /// Update the MiniGame for one frame
        /// </summary>
        /// <param name="gameTime">Current XNA time for the update call.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Render the MiniGame to the desired SpriteBatch
        /// </summary>
        /// <param name="sb">Canvas to write to.</param>
        void Render(RenderTarget2D canvas);

        /// <summary>
        /// Obtains the current state of the MiniGame
        /// </summary>
        /// <returns>A MiniGameState representing the current state of the MiniGame</returns>
        MiniGameState GetState();
    }
}

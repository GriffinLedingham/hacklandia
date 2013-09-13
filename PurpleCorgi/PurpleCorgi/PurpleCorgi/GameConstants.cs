using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurpleCorgi
{
    /// <summary>
    /// A bunch of global variables that tune game logic. Keep these secret and safe, yo.
    /// </summary>
    public class GameConstants
    {
        // Game resolution
        public const int GameResolutionWidth = 1280;
        public const int GameResolutionHeight = 720;

        // MiniGame Resolution 
        public static int MiniGameCanvasWidth { get { return GameResolutionWidth / 2; } }
        public static int MiniGameCanvasHeight { get { return GameResolutionHeight / 2; } }

        // Physics
        public const float InGameGravity = 9.82f;
    }
}

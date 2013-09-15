using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurpleCorgi
{
    class Lobby
    {
        public static Rectangle Pointer = new Rectangle(50, 50, 50, 50);

        public static Rectangle Box1 = new Rectangle(300, 468, 200, 200);
        public static Rectangle Box2 = new Rectangle(600, 468, 200, 200);
        public static Rectangle Box3 = new Rectangle(900, 468, 200, 200);

        public static int HoverNumber = -1;
        public static bool prevFrameGrabbing = false;

        public static int Difficulty = 0;

        public static Rectangle User = new Rectangle(10, 10, 100, 100);
        public static Rectangle Landing = new Rectangle(GameConstants.GameResolutionWidth / 2 - 150, GameConstants.GameResolutionHeight / 2 - 150, 300, 300);

        public static float landingHoverTimer = 0;
        public static float landingHoverDuration = 3000f;

        public static bool READY = false;

    }
}

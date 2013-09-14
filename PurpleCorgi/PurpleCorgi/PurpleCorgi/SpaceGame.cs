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

        public Vector2 playerPosition = new Vector2(320, 260);

        private List<Alien> aliens;
        private List<PlayerBullet> bullets;

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

        private class Alien
        {
            /// <summary>
            /// Represents alien color as well as Y offset on sprite sheet
            /// </summary>
            public enum AlienColor
            {
                Red = 0,
                Green = 1,
                Blue = 2,
                Yellow = 3,
            }

            public Vector2 position;
            public float targetAngle;

            private float timePassed;

            private SpaceGame parent;

            public bool dead = false;

            private AlienColor color;
            public Color DrawColor
            {
                get
                {
                    switch (color)
                    {
                        case AlienColor.Red:
                            return Color.Red;
                        case AlienColor.Green:
                            return Color.Green;
                        case AlienColor.Blue:
                            return Color.Blue;
                        case AlienColor.Yellow:
                            return Color.Yellow;
                    }

                    return Color.White;
                }
            }


            public const float alienVelocity = 0.3f;

            public Alien(SpaceGame parent)
            {
                position = new Vector2(Game1.GameRandom.Next() % (GameConstants.MiniGameCanvasWidth * 0.8f), -10);

                this.parent = parent;
                timePassed = 0;

                color = (AlienColor)(Game1.GameRandom.Next() % 4);
            }

            public void Update(GameTime currentTime)
            {
                targetAngle = (float)Math.Atan2(parent.playerPosition.Y - position.Y, parent.playerPosition.X - position.X);

                timePassed += currentTime.ElapsedGameTime.Milliseconds;

                position += new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)) * alienVelocity;
            }

            public void Draw(SpriteBatch sb)
            {
                sb.Draw(Game1.spaceSheet, position, new Rectangle((((int)timePassed / 500) % 2) * 16, 16 + 16 * (int)color, 16, 16), Color.White, targetAngle - (float)(Math.PI / 2), new Vector2(8), 3.0f, SpriteEffects.None, 0.5f);
            }
        }

        private class PlayerBullet
        {
            public Vector2 position;
            public const float bulletSpeed = 6.0f;
            public Alien target;
            public float timePassed;
            public bool dead = false;

            public PlayerBullet(Vector2 position, Alien target)
            {
                this.position = position;
                this.target = target;
                timePassed = Game1.GameRandom.Next() % 1000;
            }

            public void Update(GameTime currentTime)
            {
                float targetAngle = (float)Math.Atan2(target.position.Y - position.Y, target.position.X - position.X);

                timePassed += currentTime.ElapsedGameTime.Milliseconds;

                position += new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)) * bulletSpeed;

                if (Vector2.Distance(position, target.position) < 0.4f)
                {
                    target.dead = true;
                    dead = true;
                }

                if (target.dead)
                {
                    dead = true;
                }
            }

            public void Draw(SpriteBatch sb)
            {
                sb.Draw(Game1.WhitePixel, position, null, target.DrawColor, timePassed / 500f, new Vector2(0.5f), new Vector2(10), SpriteEffects.None, 0.5f);
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

            aliens = new List<Alien>();
            bullets = new List<PlayerBullet>();

            for (int i = 0; i < 4; i++)
            {
                aliens.Add(new Alien(this));
            }
        }

        private static bool BulletDead(PlayerBullet b)
        {
            return b.dead;
        }

        private static bool AlienDead(Alien a)
        {
            return a.dead;
        }

        public void Update(GameTime gameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Space))
            {
                if (aliens.Count > 0)
                {
                    bullets.Add(new PlayerBullet(playerPosition, aliens[0]));
                }
            }

            for (int i = 0; i < 100; i++)
            {
                stars[i].Update(gameTime);
            }

            foreach (PlayerBullet b in bullets)
            {
                b.Update(gameTime);
            }

            foreach (Alien al in aliens)
            {
                al.Update(gameTime);
            }

            bullets.RemoveAll(BulletDead);
            aliens.RemoveAll(AlienDead);

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

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            for (int i = 0; i < 100; i++)
            {
                sb.Draw(Game1.WhitePixel, new Vector2(stars[i].position.X, (GameConstants.GameResolutionHeight + 300) * (stars[i].timePassed / stars[i].durationOnScreen)), Color.White);
            }

            sb.Draw(Game1.spaceSheet, new Vector2(320, 260), new Rectangle(0, 0, 16, 16), Color.White, 0.0f, new Vector2(8), 2.0f, SpriteEffects.None, 0.0f);

            foreach (Alien ae in aliens)
            {
                ae.Draw(sb);
            }

            foreach (PlayerBullet b in bullets)
            {
                b.Draw(sb);
            }

            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

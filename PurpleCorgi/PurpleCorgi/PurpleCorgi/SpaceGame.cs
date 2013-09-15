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

        public Vector2 playerPosition = new Vector2(320, 300);
        public float playerAngle = (float)Math.PI * 1.5f;

        private List<Alien> aliens;
        private List<PlayerBullet> bullets;
        private List<Particle> particles;

        private float pushAlienTimer;
        private float timeBetweenNewAliens = 3000f;

        public int killCount;


        public void Nuke()
        {
            ein.Nuke();
        }

        public static bool ShowedTutorial = false;
        private float tutorialTimer;
        private const float tutorialDuration = 3000f;
        private class Particle
        {
            public Vector2 position;
            public float direction;
            public float velocity;
            public float timePassed;
            public float duration;
            public bool dead;

            public Particle(Vector2 position, float direction, float velocity)
            {
                this.position = position;
                this.direction = direction;
                this.velocity = velocity;
                this.timePassed = 0;
                this.duration = 500 + (Game1.GameRandom.Next() % 500f);
                this.dead = false;
            }

            public void Update(GameTime currentTime)
            {
                timePassed += currentTime.ElapsedGameTime.Milliseconds;
                if (timePassed > duration)
                {
                    dead = true;
                }

                this.position += new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction)) * velocity;
            }

            public void Draw(SpriteBatch sb)
            {
                sb.Draw(Game1.WhitePixel, position, null, Game1.GameRandom.Next() % 2 == 0 ? Color.Orange : Color.Yellow, timePassed / 500f, new Vector2(0.5f), new Vector2(10), SpriteEffects.None, 0.5f);
            }
        }

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

            public AlienColor color;
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

                if (Vector2.Distance(parent.playerPosition, position) < 4.0f)
                {
                    parent.gameState = MiniGameState.Lose;
                }
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
            public SpaceGame parent;

            public PlayerBullet(Vector2 position, Alien target, SpaceGame parent)
            {
                this.position = position;
                this.target = target;
                timePassed = Game1.GameRandom.Next() % 1000;
                this.parent = parent;
            }

            public void Update(GameTime currentTime)
            {
                float targetAngle = (float)Math.Atan2(target.position.Y - position.Y, target.position.X - position.X);

                timePassed += currentTime.ElapsedGameTime.Milliseconds;

                position += new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)) * bulletSpeed;

                if (Vector2.Distance(position, target.position) < 0.4f)
                {
                    parent.killCount++;
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

            pushAlienTimer = 0;

            killCount = 0;

            stars = new Star[100];
            for (int i = 0; i < 100; i++)
            {
                stars[i] = new Star(Game1.GameRandom.Next() % 3000);
            }

            ein = new Kinect(0, 0);
            ein.Init();

            aliens = new List<Alien>();
            bullets = new List<PlayerBullet>();
            particles = new List<Particle>();

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

        private static bool ParticleDead(Particle p)
        {
            return p.dead;
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
            pushAlienTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (pushAlienTimer > timeBetweenNewAliens)
            {
                pushAlienTimer = 0;
                aliens.Add(new Alien(this));
            }

            if (killCount >= 5)
            {
                gameState = MiniGameState.Win;
            }

            {
                string lastColor = ein.LastColor;

                if (!String.IsNullOrEmpty(lastColor))
                {
                    bool assigned = true;
                    Alien.AlienColor winColor = Alien.AlienColor.Red;

                    switch (lastColor)
                    {
                        case "RED": winColor = Alien.AlienColor.Red; break;
                        case "GREEN": winColor = Alien.AlienColor.Green; break;
                        case "YELLOW": winColor = Alien.AlienColor.Yellow; break;
                        case "BLUE": winColor = Alien.AlienColor.Blue; break;
                        default:
                            assigned = false;
                            break;
                    }

                    if (assigned)
                    {
                        foreach (Alien al in aliens)
                        {
                            if (al.color == winColor)
                            {
                                bullets.Add(new PlayerBullet(playerPosition, al, this));

                                playerAngle = (float)(Math.Atan2(al.position.Y - playerPosition.Y, al.position.X - playerPosition.X));

                                break;
                            }
                        }
                    }
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

                if (al.dead)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        particles.Add(new Particle(al.position, (float)(Game1.GameRandom.NextDouble() * Math.PI * 2), 3));
                    }
                }
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
            }

            bullets.RemoveAll(BulletDead);
            aliens.RemoveAll(AlienDead);
            particles.RemoveAll(ParticleDead);

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

            sb.Draw(Game1.spaceSheet, playerPosition, new Rectangle(0, 0, 16, 16), Color.White, playerAngle, new Vector2(8), 2.0f, SpriteEffects.None, 0.0f);
            if (!ShowedTutorial)
            {
                sb.Draw(Game1.tutorialFrames, new Vector2(40, 10), new Rectangle((((int)(tutorialTimer / 300f) % 2) * 300) + 600, 1200, 300, 300), Color.White);
            }
            foreach (Alien ae in aliens)
            {
                ae.Draw(sb);
            }

            foreach (PlayerBullet b in bullets)
            {
                b.Draw(sb);
            }

            foreach (Particle p in particles)
            {
                p.Draw(sb);
            }

            sb.End();
        }

        public MiniGameState GetState()
        {
            return gameState;
        }
    }
}

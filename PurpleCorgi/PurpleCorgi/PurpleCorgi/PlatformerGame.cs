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
    class PlatformerGame : MiniGame
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch sb;

        private float redBoxSpeed = 0.1f;

        private Vector2 redBoxPosition;
        private Color backgroundColor;

        private MiniGameState gameState;

        private World world;
        private float gravity;
        private float height, width, density, posx, posy;

        private float enemy_height, enemy_width, enemy_density, enemy_posx, enemy_posy;
        private float floor_height, floor_width, floor_density, floor_posx, floor_posy;
        private float fixpt_height, fixpt_width, fixpt_density, fixpt_posx, fixpt_posy;

        private Vector2 size, Size, enemy_size, enemy_Size, floor_size, floor_Size;

        private bool on_Ground = false;

        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        private Kinect ein;

        private bool win = false;
        private bool lost = false;

        private Corgi2 lastForearm = null;



        Body body, enemy_body, floor_body, fixpt_body;

        public PlatformerGame(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            ein = new Kinect(100, 100);
            ein.Init();

            height = 50;
            width = 50;
            density = .5f;
            posx = 350;
            posy = 0;

            enemy_height = 20;
            enemy_width = 20;
            enemy_density = 1f;
            enemy_posx = GameConstants.MiniGameCanvasWidth - 50;
            enemy_posy = 150;

            floor_height = 20;
            floor_width = GameConstants.MiniGameCanvasWidth;
            floor_density = .5f;
            floor_posx = GameConstants.MiniGameCanvasWidth/2;
            floor_posy = (GameConstants.MiniGameCanvasHeight - 50);

            fixpt_height = 1;
            fixpt_width = 1;
            fixpt_density = 1f;

            sb = new SpriteBatch(graphicsDevice);


            backgroundColor = new Color((float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), (float)Game1.GameRandom.NextDouble(), 1);
            gravity = 9.8f;
            world = new World(new Vector2(0, gravity));

            body = BodyFactory.CreateRectangle(world, width * pixelToUnit, height * pixelToUnit, density);
            body.BodyType = BodyType.Dynamic;
            body.SleepingAllowed = false;
            size = new Vector2(width, height);
            Size = size;
            body.Position = new Vector2(posx * pixelToUnit, posy * pixelToUnit);

            enemy_body = BodyFactory.CreateRectangle(world, enemy_width * pixelToUnit, enemy_height * pixelToUnit, enemy_density);
            enemy_body.BodyType = BodyType.Dynamic;
            enemy_size = new Vector2(enemy_width, enemy_height);
            enemy_Size = enemy_size;
            enemy_body.Position = new Vector2(enemy_posx * pixelToUnit, enemy_posy * pixelToUnit);

            floor_body = BodyFactory.CreateRectangle(world, floor_width * pixelToUnit, floor_height * pixelToUnit, floor_density);
            floor_body.BodyType = BodyType.Static;
            floor_size = new Vector2(floor_width, floor_height);
            floor_Size = floor_size;
            floor_body.Position = new Vector2(floor_posx * pixelToUnit, floor_posy * pixelToUnit);

            //playerRod = JointFactory.CreateWeldJoint(world, rod_body, floor_body, Vector2.Zero, new Vector2(0, rod_height/2 * pixelToUnit));
            //FarseerPhysics.Factories.JointFactory.CreateRevoluteJoint(world, rod_body, floor_body, new Vector2(0, rod_height / 2 * pixelToUnit));


            gameState = MiniGameState.Initialized;
        }

        public void Update(GameTime GameTime)
        {
            if (gameState == MiniGameState.Initialized)
            {
                gameState = MiniGameState.Running;
            }

            Random random = new Random();
            int randomNumber = random.Next(0, 100);
            if (randomNumber > 70 && enemy_body.Position.X < 0)
            {
                enemy_body.ResetDynamics();
                enemy_body.ApplyForce(new Vector2(-.6f, 0));

                enemy_body.Position = new Vector2(GameConstants.MiniGameCanvasWidth * pixelToUnit, (floor_posy - 100) * pixelToUnit);
            }

            if (body.Position.X < 0)
            {
                lost = true;
            }
            else if (body.Position.X > GameConstants.MiniGameCanvasWidth)
            {
                win = true;
            }   


            body.OnCollision +=body_OnCollision;
            float enemyForce = .3f;
            Vector2 force1 = new Vector2(0, 0);
            float forcePower = 32.5f;

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up))
                if (on_Ground) { force1 += new Vector2(15f, -forcePower); on_Ground = false; }
            //rod_body.ApplyForce(new Vector2(0, -4.7f));
            //rod_body.ApplyTorque(10.0f);
            //floor_body.ApplyForce(force1,new Vector2(floor_width/2,0));

            if (lastForearm != null)
            {
                if ((ein.LHLEVector.Y - lastForearm.Y) > .05f)
                {
                    if (on_Ground) { force1 += new Vector2(3.5f, -forcePower); on_Ground = false; }
                }
            }
            lastForearm = ein.LHLEVector;
           
            body.ApplyForce(force1);
            enemy_body.ApplyForce(new Vector2(-enemyForce, 0));
            world.Step((float)GameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Render(RenderTarget2D canvas)
        {
            Vector2 scale = new Vector2(Size.X / (float)Game1.corgi_Sprite.Width, Size.Y / (float)Game1.corgi_Sprite.Height);
            Vector2 enemy_scale = new Vector2(enemy_Size.X / (float)Game1.WhitePixel.Width, enemy_Size.Y / (float)Game1.WhitePixel.Height);
            Vector2 floor_scale = new Vector2(floor_Size.X / (float)Game1.WhitePixel.Width, floor_Size.Y / (float)Game1.WhitePixel.Height);


            graphicsDevice.SetRenderTarget(canvas);
            graphicsDevice.Clear(backgroundColor);
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            sb.Draw(Game1.corgi_Sprite, body.Position * unitToPixel, null, Color.White, body.Rotation, new Vector2(Game1.corgi_Sprite.Width / 2.0f, Game1.corgi_Sprite.Height / 2.0f), scale, SpriteEffects.None, 0);
            sb.Draw(Game1.WhitePixel, floor_body.Position * unitToPixel, null, Color.White, floor_body.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), floor_scale, SpriteEffects.None, 0);
            sb.Draw(Game1.WhitePixel, enemy_body.Position * unitToPixel, null, Color.Blue, enemy_body.Rotation, new Vector2(Game1.WhitePixel.Width / 2.0f, Game1.WhitePixel.Height / 2.0f), enemy_scale, SpriteEffects.None, 0);            
            sb.End();
        }

        private bool body_OnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (f1.Body == body && f2.Body == floor_body)
            {
                on_Ground = true;}
            return true;
        }

        public MiniGameState GetState()
        {
            if (win)
                return MiniGameState.Win;
            else if (lost)
                return MiniGameState.Lose;
            else
                return MiniGameState.Running;
        }
    }
}

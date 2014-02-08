namespace Physicist
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using FarseerPhysics;
    using FarseerPhysics.Common;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Physicist.Actors;
    using Physicist.Controls;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private static World world;
        private static List<Actor> actors;
        private static List<string> maps;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D testText;
        private Rectangle backgroundsize;
        private Vertices platformVert;
        
        public MainGame()
            : base()
        {
            this.graphics = new GraphicsDeviceManager(this);
        }

        public static World World
        {
            get
            {
                return MainGame.world;
            }
        }

        public static void RegisterActor(Actor actor)
        {
            MainGame.actors.Add(actor);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ContentController.Instance.Initialize(this.Content, "Content");
            MainGame.actors = new List<Actor>();
            MainGame.maps = new List<string>() { "Content\\Levels\\TestLevel.xml" };
            //// TODO: Add your initialization logic here
            base.Initialize();
        }

        private SpriteFont myFont;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            myFont = this.Content.Load<SpriteFont>("Fonts\\Pericles6");
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.SetupWorld(MainGame.maps[0]);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameTime != null)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }

                MainGame.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                foreach (var actor in MainGame.actors)
                {
                    Player player = actor as Player;
                    if (player != null)
                    {
                        player.Update(gameTime, Keyboard.GetState());
                    }
                    else
                    {
                        actor.Update(gameTime);
                    }
                }

                // TODO: Add your update logic here
            }

            base.Update(gameTime);
        }

        public static Fixture Joint;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this.spriteBatch.Begin();

            this.spriteBatch.Draw(this.testText, this.platformVert[0].ToDisplayUnits(), Color.White); 

            this.spriteBatch.DrawString(myFont, this.test.Position.ToString(), new Vector2(300, 40), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            if (Joint != null)
            {
             //   this.spriteBatch.DrawString(myFont, Joint.Position.ToString(), new Vector2(300, 60), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            }
            this.test.Draw(this.spriteBatch);
            MainGame.actors.ForEach(actor => actor.Draw(this.spriteBatch));

            base.Draw(gameTime);

            this.spriteBatch.End();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Loop Body is tracked and disposed by world")]
        private void SetupWorld(string mapPath)
        {
            MainGame.world = new World(new Vector2(0f, 9.81f));
            ConvertUnits.SetDisplayUnitToSimUnitRatio(1f);

            if (MapLoader.LoadMap(mapPath))
            {
                foreach (var error in MapLoader.Errors)
                {
                    System.Console.WriteLine(error);
                }
            }

            if (MapLoader.HasFailed)
            {
                System.Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Loading of Map: {0} has failed!", mapPath));
                throw new AggregateException("Error: Map Load Failure!");
            }

            Vertices borderVerts = new Vertices();
            borderVerts.Add(Vector2.Zero.ToSimUnits());
            borderVerts.Add(new Vector2(0, this.GraphicsDevice.Viewport.Height).ToSimUnits());
            borderVerts.Add(new Vector2(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height).ToSimUnits());
            borderVerts.Add(new Vector2(this.GraphicsDevice.Viewport.Width, 0).ToSimUnits());

            var border = BodyFactory.CreateLoopShape(MainGame.World, borderVerts);
            border.Friction = 1f;

            this.backgroundsize = new Rectangle(100, 400, 100, 20);
            this.testText = new Texture2D(this.GraphicsDevice, this.backgroundsize.Width, this.backgroundsize.Height);
            Color[] colors = new Color[this.backgroundsize.Width * this.backgroundsize.Height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(255, 255, 0);
            }

            this.platformVert = new Vertices();
            foreach (var corner in this.backgroundsize.GetCorners())
            {
                this.platformVert.Add(new Vector2(corner.X, corner.Y).ToSimUnits());
            }

            BodyFactory.CreateLoopShape(MainGame.World, this.platformVert);

            this.testText.SetData(colors);
        }

        private void CreateActors()
        {
            this.texture = this.Content.Load<Texture2D>("Textures\\NOTSTOLEN");

            GameSprite testSprite = new GameSprite(this.texture, new Size(19, 40));
            testSprite.AddAnimation(StandardAnimation.Idle, new SpriteAnimation(0, 1, 1));
            testSprite.AddAnimation(StandardAnimation.Down, new SpriteAnimation(0, 8, 1));
            testSprite.AddAnimation(StandardAnimation.Up, new SpriteAnimation(0, 8, 1) { FlipVertical = true });
            testSprite.AddAnimation(StandardAnimation.Right, new SpriteAnimation(1, 8, 1));
            testSprite.AddAnimation(StandardAnimation.Left, new SpriteAnimation(1, 8, 1) { FlipHorizontal = true });
            testSprite.CurrentAnimationString = StandardAnimation.Idle.ToString();

            this.test = new Player();
            this.test.AddSprite("test", testSprite);

            this.test.Body = BodyFactory.CreateRectangle(MainGame.world, ConvertUnits.ToSimUnits(19f), ConvertUnits.ToSimUnits(40f), 100f);
            this.test.Body.BodyType = BodyType.Dynamic;
            this.test.Body.CollidesWith = Category.All;
            this.test.Body.FixedRotation = true;
            this.test.Position = new Vector2(100f, 100f).ToSimUnits();
            this.test.MaxSpeed = new Vector2(60f, 120f);
        }
    }
}

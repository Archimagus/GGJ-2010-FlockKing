using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using FlocKing.Helpers;
using FlocKing.Particles;
using FlocKing.Objects;
using FlocKing.Managers;
using FlocKing.StateMachine;


namespace FlocKing
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public float SoundEffectsVolume { get; set; }
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Random Random = new Random();

        FpsCounter fps = new FpsCounter();
        SpriteFont menuFont;
        InputManager inputManager = new InputManager();

        public List<Player> Players;
        public CollisionManager collisionManager = new CollisionManager();
        private bool _isFullScreen;

        public static Game1 Instance { get; set; }

        public Game1()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);

#if !DEBUG
            var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            graphics.PreferredBackBufferWidth = displayMode.Width;
            graphics.PreferredBackBufferHeight = displayMode.Height;
            graphics.PreferredBackBufferFormat = displayMode.Format;
            graphics.ToggleFullScreen();
            _isFullScreen = true;
#endif
            Content.RootDirectory = "Content";

            SoundEffectsVolume = 1.0f;
            SoundEffect.MasterVolume = 0.2f;
            Players = new List<Player>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ServiceProvider.AddService(Content);
            ServiceProvider.AddService(graphics);
            ServiceProvider.AddService(inputManager);

            Camera.Instance.Init(graphics.GraphicsDevice);
            //CStateMachine.Instance.PushState(new GameplayState());
            CStateMachine.Instance.PushState(new MenuState());
            inputManager.KeyDown += inputManager_KeyDown;

            fps.Initialize();
            base.Initialize();
        }

        void inputManager_KeyDown(InputManager sender, KeyboardEventArgs e)
        {
            KeyboardState ks = Keyboard.GetState();
            if (e.KeyID == Keys.Enter && (ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)))
            {
                _isFullScreen = !_isFullScreen;
                if (_isFullScreen)
                {
                    var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                    graphics.PreferredBackBufferWidth = displayMode.Width;
                    graphics.PreferredBackBufferHeight = displayMode.Height;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = 1024;
                    graphics.PreferredBackBufferHeight = 768;
                }
                graphics.ToggleFullScreen();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ServiceProvider.AddService(spriteBatch);
            // TODO: use this.Content to load your game content here

            menuFont = Content.Load<SpriteFont>("Fonts\\MenuFont");
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
           // fps.Update(gameTime);
            base.Update(gameTime);
            inputManager.Update();
            KeyboardState ks = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !GameplayState.paused)
                CStateMachine.Instance.PushState(new PauseExitGameState());
            if (!CStateMachine.Instance.Input() || ks.IsKeyDown(Keys.Escape))
                this.Exit();



            // TODO: Add your update logic here
            CStateMachine.Instance.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //Viewport viewport = GraphicsDevice.Viewport;

            //float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            //float time = (float)gameTime.TotalGameTime.TotalSeconds;

            //Matrix rotation = Matrix.CreateRotationY(time * 0.1f);

            //Matrix view = Matrix.CreateLookAt(new Vector3(1000, 500, 0),
            //                                  new Vector3(0, 150, 0),
            //                                  Vector3.Up);

            //Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            //                                                        aspectRatio,
            //                                                        10,

            //                                                        10000);




            // TODO: Add your drawing code here
            CStateMachine.Instance.Render();

            //spriteBatch.Begin();
            //spriteBatch.DrawString(menuFont, "FPS: " + fps.Fps, new Vector2(0, 0), Color.White);
            //spriteBatch.End();
            base.Draw(gameTime);
        }


        public static float RandomFloatBetween(float min, float max)
        {
            return min + (float)Game1.Instance.Random.NextDouble() * (max - min);
        }
    }
}
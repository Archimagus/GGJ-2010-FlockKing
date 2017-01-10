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
using System.Windows.Forms;
using FlocKing.Particles;
using FlocKing.Helpers;
using FlocKing;

namespace ParticleEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ParticleEditorGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PropertiesDialog propertyWindow;
        ParticleSystem particleSystem;
        Camera camera;

        public ParticleEditorGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            camera = new Camera();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Game1 g1= new Game1();
            // TODO: Add your initialization logic here
            camera.Init(graphics.GraphicsDevice);
            camera.CameraLookAt = Vector3.Zero;
            camera.CameraPosition = new Vector3(0, 0, -30);
            MouseState ms = Mouse.GetState();
            lastMouseX = ms.X;
            lastMouseY = ms.Y;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ServiceProvider.AddService(Content);
            ServiceProvider.AddService(graphics);
            // TODO: use this.Content to load your game content here
            particleSystem = new ParticleSystem(1, Content.Load<Texture2D>(@"particles\smoke2"), "FireDeath.xml");
            propertyWindow = new PropertiesDialog();
            propertyWindow.DataSource = particleSystem;
            propertyWindow.PropertyChanged += propertyWindow_PropertyChanged;
            Control f = Form.FromHandle(this.Window.Handle);
            propertyWindow.Show(f);
        }

        void propertyWindow_PropertyChanged()
        {
            particleSystem.Initialize();
            particleSystem.AddParticles(Vector3.Zero);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            propertyWindow.Hide();
            propertyWindow.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            RotateCamera();
            camera.Update(true);
            particleSystem.Update(gameTime);
            base.Update(gameTime);
        }
        float lastMouseX;
        float lastMouseY;
        private void RotateCamera()
        {
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                float x = lastMouseX - ms.X;
                float y = lastMouseY - ms.Y;

                Vector3 direction = camera.CameraPosition - camera.CameraLookAt;
                float distance = direction.Length();
                Vector3 pos = camera.CameraPosition;
                pos.Y += y;
                pos.X += x;
                pos.Normalize();
                pos *= distance;
                camera.CameraPosition = pos;
            }
            lastMouseX = ms.X;
            lastMouseY = ms.Y;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            particleSystem.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}

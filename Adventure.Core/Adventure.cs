using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Adventure.Core;

public sealed class Adventure : Game
{
    public const int ResolutionWidth = 256;
    public const int ResolutionHeight = 256;

    public Adventure()
    {
        Instance = this;
        Window.Title = "Adventure Game 2000";
        Window.AllowUserResizing = true;
        Content = new ContentManager(Services, "Content");
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        GraphicsDeviceManager.HardwareModeSwitch = false;
        GraphicsDeviceManager.IsFullScreen = false;
        GraphicsDeviceManager.PreferredBackBufferWidth = ResolutionWidth;
        GraphicsDeviceManager.PreferredBackBufferHeight = ResolutionHeight;
        IsFixedTimeStep = true;
        IsMouseVisible = false;
    }

    public static Adventure Instance { get; private set; }
    public static GameTime Time { get; private set; }
    public static bool IsPaused { get; set; }
    public GraphicsDeviceManager GraphicsDeviceManager { get; }
    public VirtualBackBuffer VirtualBackBuffer { get; private set; }
    public Renderer Renderer { get; private set; }
    public Camera Camera { get; private set; }
    public CoroutineRunner Coroutines { get; } = new();
    public Session Session { get; private set; }

    protected override void Initialize()
    {
        VirtualBackBuffer = new VirtualBackBuffer(GraphicsDevice, ResolutionWidth, ResolutionHeight);
        Window.ClientSizeChanged += (o, e) => VirtualBackBuffer.FitToNativeBackBuffer();
        Renderer = new Renderer(GraphicsDevice);
        Camera = new Camera(ResolutionWidth, ResolutionHeight);
    }

    protected override void LoadContent()
    {
        ContentCache.Fonts.Default = Content.Load<SpriteFont>("fonts/font");
        ContentCache.Gfx.Cursor = Content.Load<Texture2D>("art/cursor");
        ContentCache.Gfx.Player = Content.Load<Texture2D>("art/player/player");
        ContentCache.Gfx.Fire = Content.Load<Texture2D>("art/player/fire");
        ContentCache.Gfx.Barrel = Content.Load<Texture2D>("art/common/barrel");
        ContentCache.Gfx.Explosion = Content.Load<Texture2D>("art/common/explosion-1");
        ContentCache.Gfx.LargeDoor = Content.Load<Texture2D>("art/large_door");
        ContentCache.Gfx.PressurePlate = Content.Load<Texture2D>("art/pressure_plate");
        ContentCache.Vfx.SolidColor = Content.Load<Effect>("shaders/SolidColor");
        ContentCache.Sfx.Shoot = Content.Load<SoundEffect>("audio/fireball_shoot");
        ContentCache.Sfx.Explosion = Content.Load<SoundEffect>("audio/explosion4");

        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        VirtualBackBuffer.Dispose();
        Renderer.Dispose();
        Content.Unload();
    }

    protected override void Update(GameTime gameTime)
    {
        Time = gameTime;
        Input.Update(VirtualBackBuffer);
        Session?.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        // Set render target to virtual backbuffer and use virtual viewport.
        Renderer.SetTarget(VirtualBackBuffer);
        Renderer.SetViewport(VirtualBackBuffer.Viewport);
        Renderer.Clear(Color.Black);

        Renderer.BeginDraw(Camera.TransformationMatrix);

        Renderer.EndDraw();

        // Set target to screen and set viewport to letterboxed viewport.
        Renderer.SetTarget(null);
        Renderer.SetViewport(VirtualBackBuffer.NativeViewport);
        Renderer.Clear(Color.Black);
        Renderer.BeginDraw(VirtualBackBuffer.ScaleMatrix);

        // Draw the virtual backbuffer to the screen.
        Renderer.Draw(VirtualBackBuffer, Vector2.Zero);
        Renderer.EndDraw();
    }

    public async void StartSession()
    {
        if (Session == null)
        {
            Session = new Session(new NetworkContext());

            try
            {
                await Session.ConnectAsync();
            }
            catch (Exception exception) 
            {
                Session = null;

                // TODO: Display error message.

                Debug.WriteLine("Error when making connection to server: " + exception.Message);
            }
        }
    }
}

using System.Numerics;
using Chroma.Commander.Environment;
using Chroma.Commander.Expressions;
using Chroma.Diagnostics;
using Chroma.Graphics;
using Chroma.Input;

namespace Chroma.Commander.TestApp
{
    public class App : Game
    {
        private DebugConsole _console;
        private Texture _texture;

        [ConsoleVariable("gfx_timestep")]
        private int TimeStep
        {
            get => FixedTimeStepTarget;
            set => FixedTimeStepTarget = value;
        }

        [ConsoleVariable("gfx_limit")]
        private bool LimitFramerate
        {
            get => Graphics.LimitFramerate;
            set => Graphics.LimitFramerate = value;
        }

        [ConsoleVariable("gfx_vsync")]
        private VerticalSyncMode VSync
        {
            get => Graphics.VerticalSyncMode;
            set => Graphics.VerticalSyncMode = value;
        }

        [ConsoleVariable("cl_showfps")]
        private byte ShowFpsMode { get; set; }

        [ConsoleVariable("app_wintitle")]
        private string _winTitle;

        public App() : base(new(false, false))
        {
            Window.Mode.SetWindowed(1024, 600);
        }

        [ConsoleCommand("test_cmd")]
        [ConsoleCommand("test_cmd2")]
        [ConsoleCommand("test_cmd3")]
        internal static void TestCommand(DebugConsole console, params ExpressionValue[] args)
        {
            foreach (var arg in args)
            {
                console.Print(arg.ToString());
            }
        }

        protected override void LoadContent()
        {
            _console = new DebugConsole(Window);
            _texture = Content.Load<Texture>("backdrop.jpg");
            _texture.VirtualResolution = Window.Size;
            _console.RegisterStaticEntities();
            _console.RegisterInstanceEntities(this);
        }

        protected override void Draw(RenderContext context)
        {
            context.DrawTexture(_texture, Vector2.Zero, Vector2.One, Vector2.Zero, 0);
            _console.Draw(context);
        }

        protected override void WheelMoved(MouseWheelEventArgs e)
        {
            _console.WheelMoved(e);
        }

        protected override void Update(float delta)
        {
            Window.Title = $"{PerformanceCounter.FPS:F3} FPS";

            _console.Update(delta);
        }

        protected override void KeyPressed(KeyEventArgs e)
        {
            _console.KeyPressed(e);
        }

        protected override void TextInput(TextInputEventArgs e)
        {
            _console.TextInput(e);
        }
    }
}
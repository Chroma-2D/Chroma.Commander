using System.Globalization;
using System.Numerics;
using Chroma.Diagnostics;
using Chroma.Graphics;
using Chroma.Input;

namespace Chroma.Commander.TestApp
{
    public class App : Game
    {
        private DebugConsole _console;
        private Texture _texture;
        
        public App() : base(new(false, false))
        {
            Window.Mode.SetWindowed(1024, 600);
        }

        protected override void LoadContent()
        {
            _console = new DebugConsole(Window);
            _console.RawInputHandler = HandleConsoleInput;
            _texture = Content.Load<Texture>("backdrop.jpg");
            _texture.VirtualResolution = Window.Size;
        }

        private int HandleConsoleInput(ref string[] tokens)
        {
            return 0;
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
            Window.Title = PerformanceCounter.FPS.ToString(CultureInfo.InvariantCulture);
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
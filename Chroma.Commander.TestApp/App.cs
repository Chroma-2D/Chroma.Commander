using System.Numerics;
using Chroma.Diagnostics;
using Chroma.Graphics;
using Chroma.Input;

namespace Chroma.Commander.TestApp
{
    public class App : Game
    {
        private InGameConsole _console;
        private Texture _texture;
        
        public App() : base(new(false, false))
        {
            Window.GoWindowed(new(1024, 600));
        }

        protected override void LoadContent()
        {
            _console = new InGameConsole(Window);
            _texture = Content.Load<Texture>("G:\\Pictures\\Cyberpunk 2077\\photomode_14122020_204041.png");
            _texture.VirtualResolution = Window.Size;
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
            Window.Title = PerformanceCounter.FPS.ToString();
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
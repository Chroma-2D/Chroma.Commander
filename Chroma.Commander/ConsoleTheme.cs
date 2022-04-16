using Chroma.Graphics;

namespace Chroma.Commander
{
    public class ConsoleTheme
    {
        public Color BackgroundColor { get; set; } = new(0, 0, 0, 200);
        public Color BorderColor { get; set; } = Color.CornflowerBlue;
        public Color TextColor { get; set; } = Color.White;
    }
}
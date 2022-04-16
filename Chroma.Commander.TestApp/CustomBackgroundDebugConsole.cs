using System.Numerics;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering.TrueType;
using Chroma.Windowing;

namespace Chroma.Commander.TestApp
{
    public class CustomBackgroundDebugConsole : DebugConsole
    {
        private Texture BackgroundTexture { get; }
        
        private string _watermark = "Chroma.Commander v1.0a\nrussian warship go fuck yourself";
        
        public CustomBackgroundDebugConsole(Window window, Texture backgroundTexture, int maxLines = 20) 
            : base(window, maxLines)
        {
            BackgroundTexture = backgroundTexture;
            BackgroundTexture.ColorMask = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            
            BackgroundTexture.VirtualResolution = Dimensions;
            Theme.BorderColor = Color.CornflowerBlue;
        }

        protected override void DrawBackdrop(RenderContext context)
        {
            context.DrawTexture(BackgroundTexture, Vector2.Zero);

            var measure = TrueTypeFont.Default.Measure(_watermark);
            context.DrawString(
                _watermark, 
                Dimensions.Width - measure.Width - 4,
                Dimensions.Height - measure.Height - 4,
                new Color(1f, 1f, 1f, 0.4f)
            );
        }
    }
}
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using Chroma.Commander.Expressions;
using Chroma.Diagnostics;
using Chroma.Graphics;
using Chroma.Input;

namespace Chroma.Commander.TestApp
{
    using Chroma.ContentManagement;

    public class App : Game
    {
        private CustomBackgroundDebugConsole _console;
        private Texture _appbackdrop;
        private Texture _consolebg;

        [ConsoleVariable("gfx_timestep", Description = "fixed time step target in frames per second")]
        private int TimeStep
        {
            get => FixedTimeStepTarget;
            set => FixedTimeStepTarget = value;
        }

        [ConsoleVariable("gfx_limit", Description = "whether or not framerate limiter is enabled")]
        private bool LimitFramerate
        {
            get => Graphics.LimitFramerate;
            set => Graphics.LimitFramerate = value;
        }

        [ConsoleVariable("gfx_vsync", Description = "control vertical synchronization")]
        private VerticalSyncMode VSync
        {
            get => Graphics.VerticalSyncMode;
            set => Graphics.VerticalSyncMode = value;
        }

        [ConsoleVariable("cl_showfps", Description = "diagnostic purposes only, different modes of performance printout")]
        private byte ShowFpsMode { get; }

        [ConsoleVariable("app_wintitle", Description = "global window title")]
        private string _winTitle;

        public App() : base(new(false, false))
        {
            Window.Mode.SetWindowed(1024, 600);
        }

        [ConsoleCommand(
            "test_cmd", 
            DefaultArguments = new object[] { 20, "test", false }, 
            Description = "it's a test command...")]
        internal static void TestCommand(DebugConsole console, params ExpressionValue[] args)
        {
            foreach (var arg in args)
            {
                console.Print(arg.ToString());
            }
        }

        [ConsoleCommand("help")]
        private static void HelpCommand(DebugConsole console, params ExpressionValue[] args)
        {
            console.Print("--- COMMANDS ---");
            foreach (var command in console.Commands)
            {
                var sb = new StringBuilder();
                sb.Append(command.Trigger);
                
                if (command.DefaultArguments != null)
                {
                    sb.Append(" [");
                    sb.Append(string.Join(
                        ' ', 
                        command.DefaultArguments.Select(
                            x => x.ToConsoleStringRepresentation()
                        )
                    ));
                    sb.Append("]");
                }

                sb.Append(" - ");
                sb.Append(command.Description);
                console.Print(sb.ToString());
            }
            
            console.Print("--- VARIABLES ---");
            foreach (var variable in console.Variables)
            {
                console.Print($"{variable.Name} : {variable.Type} - {variable.Description}");
            }
        }

        [ConsoleCommand("test_instance",
            Description = "test instance command or something",
            DefaultArguments = new object[] { 666, true, "yes", VerticalSyncMode.Adaptive })]
        private void InstanceCommand(DebugConsole console, params ExpressionValue[] args)
        {
            console.Print("Yay!");
        }

        protected override void Initialize(IContentProvider content)
        {
            _consolebg = content.Load<Texture>("console_bg.jpg");
            _console = new CustomBackgroundDebugConsole(Window, _consolebg);
            
            _appbackdrop = content.Load<Texture>("backdrop.jpg");
            _appbackdrop.VirtualResolution = Window.Size;
            _console.RegisterStaticEntities();
            _console.RegisterInstanceEntities(this);
        }

        protected override void Draw(RenderContext context)
        {
            context.DrawTexture(_appbackdrop, Vector2.Zero, Vector2.One, Vector2.Zero, 0);
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
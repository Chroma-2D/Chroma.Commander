using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Chroma.Commander.Expressions;
using Chroma.Commander.Expressions.Syntax;
using Chroma.Commander.Text;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering.TrueType;
using Chroma.Input;
using Chroma.MemoryManagement;
using Chroma.Windowing;

namespace Chroma.Commander
{
    public partial class DebugConsole : DisposableResource
    {
        private enum State
        {
            SlidingUp,
            SlidingDown,
            Hidden,
            Visible
        }

        private Window _window;
        private RenderTarget _target;
        private Vector2 _offset;
        private TrueTypeFont _ttf;

        private ScrollBuffer _scrollBuffer;
        private List<string> _scrollBufferWindow;

        private InputHistory _inputHistory;
        private InputLine _inputLine;
        private State _state = State.Hidden;

        private ConsoleCommandRegistry _commandRegistry;
        private ConsoleVariableRegistry _conVarRegistry;

        public float SlidingSpeed { get; set; } = 2750;
        public KeyCode ToggleKey { get; set; } = KeyCode.Grave;

        public List<ConsoleVariableInfo> Variables => _conVarRegistry.RetrieveConVarList();
        public List<ConsoleCommandInfo> Commands => _commandRegistry.RetrieveCommandInfoList();

        public DebugConsole(Window window, int maxLines = 20)
        {
            _window = window;

            LoadFont();
            _target = new RenderTarget(
                window.Size.Width,
                maxLines * _ttf!.Height + 2 + _ttf.Height + 2
            );

            _offset.Y = -_target.Height;
            _scrollBuffer = new ScrollBuffer(maxLines);

            _inputHistory = new InputHistory();
            _inputLine = new InputLine(
                new(0, maxLines * _ttf.Height),
                _ttf,
                _target.Width / 8,
                HandleUserInput
            );

            _commandRegistry = new ConsoleCommandRegistry(this);
            _conVarRegistry = new ConsoleVariableRegistry();
        }

        public void RegisterStaticEntities()
        {
            var asm = Assembly.GetCallingAssembly();

            foreach (var type in asm.GetTypes())
            {
                RegisterStaticCommands(type);
                RegisterStaticConVars(type);
            }
        }

        public void RegisterInstanceEntities(object owner)
        {
            RegisterInstanceCommands(owner);
            RegisterInstanceConVars(owner);
        }

        public void Draw(RenderContext context)
        {
            if (_state == State.Hidden)
                return;

            context.RenderTo(_target, () =>
            {
                DrawBackdrop(context);

                for (var i = 0; i < _scrollBufferWindow.Count; i++)
                {
                    context.DrawString(
                        _ttf,
                        _scrollBufferWindow[i],
                        new(0, _ttf.Height * i),
                        Color.White
                    );
                }

                _inputLine.Draw(context);

                RenderSettings.LineThickness = 2;
                context.Line(
                    new(0, _target.Height - 1),
                    new(_target.Width, _target.Height - 1),
                    ColorScheme.Border
                );
            });

            context.DrawTexture(
                _target,
                _offset,
                Vector2.One,
                Vector2.Zero,
                0
            );
        }

        public void Update(float delta)
        {
            if (_state == State.SlidingDown)
            {
                if (_offset.Y == 0)
                {
                    _state = State.Visible;
                }
                else
                {
                    _offset.Y += SlidingSpeed * delta;

                    if (_offset.Y > 0)
                        _offset.Y = 0;
                }
            }
            else if (_state == State.SlidingUp)
            {
                if (_offset.Y <= -_target.Height)
                {
                    _state = State.Hidden;
                }
                else
                {
                    _offset.Y -= SlidingSpeed * delta;

                    if (_offset.Y <= -_target.Height)
                        _offset.Y = -_target.Height;
                }
            }

            if (_state == State.Hidden)
                return;

            _scrollBufferWindow = _scrollBuffer.GetWindow();
            _inputLine.Update(delta);
        }

        public void KeyPressed(KeyEventArgs e)
        {
            if (e.KeyCode == ToggleKey)
            {
                if (_state == State.Hidden || _state == State.SlidingUp)
                {
                    _state = State.SlidingDown;
                }
                else if (_state == State.Visible || _state == State.SlidingDown)
                {
                    _state = State.SlidingUp;
                }
            }

            if (_state == State.Hidden || _state == State.SlidingUp)
                return;

            if (e.KeyCode == KeyCode.Up)
            {
                _inputHistory.Previous();                
                _inputLine.Set(_inputHistory.CurrentEntry);
            }
            else if (e.KeyCode == KeyCode.Down)
            {
                var wasAtEnd = _inputHistory.IsAtEnd;
                _inputHistory.Next();

                if (wasAtEnd && _inputHistory.IsAtEnd)
                {
                    _inputLine.Clear();
                }
                else
                {
                    _inputLine.Set(_inputHistory.CurrentEntry);
                }
            }
            else
            {
                _inputLine.KeyPressed(e);
            }
        }

        public void TextInput(TextInputEventArgs e)
        {
            if (_state == State.Hidden || _state == State.SlidingUp)
                return;

            if (Keyboard.IsKeyDown(ToggleKey))
                return;

            _inputLine.TextInput(e);
        }

        public void WheelMoved(MouseWheelEventArgs e)
        {
            if (_state != State.Visible)
                return;

            if (e.Motion.Y > 0)
            {
                _scrollBuffer.ScrollUp();
            }
            else if (e.Motion.Y < 0)
            {
                _scrollBuffer.ScrollDown();
            }
        }

        protected virtual void DrawBackdrop(RenderContext context)
        {
            context.Clear(ColorScheme.Background);
        }

        private void LoadFont()
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Chroma.Commander.Resources.PxPlus_ToshibaSat_8x14.ttf");

            _ttf = new TrueTypeFont(
                stream,
                16,
                string.Join("", CodePage.BuildCodePage437Plus())
            );
        }

        private void HandleUserInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            _inputHistory.AddToHistory(input);
            _scrollBuffer.ScrollToEnd();

            Print(input);

            try
            {
                var parser = new Parser(input);
                var directive = parser.Parse();
                Visit(directive);
            }
            catch (InvalidOperationException e)
            {
                Print(e.Message);
            }
            catch (FormatException e)
            {
                Print(e.Message);
            }
            catch (ExpressionException e)
            {
                Print(e.Message);
            }
            catch (EntityNotFoundException e)
            {
                Print(e.Message);
            }
            catch (ConVarReadException e)
            {
                Print(e.Message);
            }
            catch (ConVarWriteException e)
            {
                Print(e.Message);
            }
            catch (ConVarTypeMismatchException e)
            {
                Print(e.Message);
            }
            catch (ConVarOutOfRangeException e)
            {
                Print(e.Message);
            }
            catch (ConVarMisuseException e)
            {
                Print(e.Message);
            }
            catch (ConVarConversionException e)
            {
                Print(e.Message);
            }
        }

        protected override void FreeManagedResources()
        {
            _ttf.Dispose();
        }
    }
}
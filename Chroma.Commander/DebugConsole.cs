using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Chroma.Commander.Expressions;
using Chroma.Commander.Expressions.Syntax;
using Chroma.Commander.Expressions.Syntax.AST;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering.TrueType;
using Chroma.Input;
using Chroma.MemoryManagement;
using Chroma.Windowing;
using Color = Chroma.Graphics.Color;

namespace Chroma.Commander
{
    public class DebugConsole : DisposableResource
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
        private int _maxLines;

        private ScrollBuffer _scrollBuffer;
        private List<string> _scrollBufferWindow;

        private InputLine _inputLine;

        private State _state = State.Hidden;

        private ConsoleCommandRegistry _commandRegistry;

        public float SlidingSpeed { get; set; } = 2000;
        public KeyCode ToggleKey { get; set; } = KeyCode.Grave;
        public ConsoleInputHandler RawInputHandler { get; set; }

        public DebugConsole(Window window, int maxLines = 20)
        {
            _window = window;
            _maxLines = maxLines;

            LoadFont();
            _target = new RenderTarget(
                window.Size.Width,
                maxLines * _ttf.Height + 2 + _ttf.Height + 2
            );

            _offset.Y = -_target.Height;
            _scrollBuffer = new ScrollBuffer(maxLines);
            _inputLine = new InputLine(
                new(0, maxLines * _ttf.Height),
                _ttf,
                _target.Width / 8,
                HandleUserInput
            );

            _commandRegistry = new ConsoleCommandRegistry(this);
        }

        public void RegisterEntities()
        {
            var asm = Assembly.GetCallingAssembly();

            foreach (var type in asm.GetTypes())
            {
                RegisterCommands(type);
                RegisterConVars(type);
            }
        }

        private void RegisterCommands(Type type)
        {
            foreach (var method in type.GetMethods(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                foreach (var attr in method.GetCustomAttributes<ConsoleCommandAttribute>())
                {
                    var trigger = attr.Trigger;
                    var cmdDelegate = method.CreateDelegate<ConsoleCommand>();

                    if (cmdDelegate == null)
                    {
                        throw new InvalidOperationException(
                            $"Attempt to register a method with invalid signature as '{trigger}'.");
                    }

                    _commandRegistry.Register(trigger, cmdDelegate);
                }
            }
        }

        private void RegisterConVars(Type t)
        {
            
        }

        public void Print(string value)
            => PushToOutputBuffer(value);

        public void Print(object value)
            => Print(value.ToString());

        public void Print(bool value)
            => Print(value.ToString());

        public void Print(sbyte value)
            => Print(value.ToString());

        public void Print(byte value)
            => Print(value.ToString());

        public void Print(short value)
            => Print(value.ToString());

        public void Print(ushort value)
            => Print(value.ToString());

        public void Print(int value)
            => Print(value.ToString());

        public void Print(uint value)
            => Print(value.ToString());

        public void Print(long value)
            => Print(value.ToString());

        public void Print(ulong value)
            => Print(value.ToString());

        public void Print(float value)
            => Print(value, CultureInfo.InvariantCulture);

        public void Print(float value, IFormatProvider provider)
            => Print(value.ToString(provider));

        public void Print(double value)
            => Print(value.ToString(CultureInfo.InvariantCulture));

        public void Print(double value, IFormatProvider provider)
            => Print(value.ToString(provider));

        public void Draw(RenderContext context)
        {
            if (_state == State.Hidden)
                return;

            context.RenderTo(_target, () =>
            {
                context.Clear(ColorScheme.Background);

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

            _inputLine.KeyPressed(e);
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

        private void LoadFont()
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Chroma.Commander.Resources.PxPlus_ToshibaSat_8x14.ttf");

            _ttf = new TrueTypeFont(stream, 16, string.Join("", CodePage.BuildCodePage437Plus()));
        }

        private void HandleUserInput(string input)
        {
            Print(input);

            try
            {
                var parser = new Parser(input);
                var directive = parser.Parse();

                Visit(directive);
            }
            catch (FormatException e)
            {
                Print($"Bad input format: {e.Message}");
            }
            catch (ExpressionException e)
            {
                Print($"Expression evaluation error: {e.Message}");
            }
            catch (EntityNotFoundException e)
            {
                Print(e.Message);
            }
            catch (Exception e)
            {
                Print($"Unhandled exception: {e.Message}");
                Print(e.StackTrace);
            }
        }

        private void PushToOutputBuffer(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                _scrollBuffer.Push(string.Empty);
                return;
            }

            var sb = new StringBuilder();
            var strings = new List<string>();

            for (var i = 0; i < input.Length; i++)
            {
                sb.Append(input[i]);

                if (sb.Length >= _target.Width / 8 || i == input.Length - 1)
                {
                    strings.Add(sb.ToString());
                    sb.Clear();
                }
            }

            foreach (var s in strings)
            {
                _scrollBuffer.Push(s);
            }
        }

        protected override void FreeManagedResources()
        {
            _ttf.Dispose();
        }

        private void Visit(DirectiveNode dir)
        {
            if (dir.Child is AssignNode asgn)
            {
                Visit(asgn);
            }
            else if (dir.Child is InvocationNode inv)
            {
                Visit(inv);
            }
            else
            {
                throw new ExpressionException($"Unsupported node type {dir.GetType()}.");
            }
        }

        private void Visit(AssignNode asgn)
        {
        }

        private ExpressionValue Visit(BinOpNode binOp)
        {
            var left = Visit(binOp.Left);
            var right = Visit(binOp.Right);
            
            switch (binOp.Type)
            {
                case BinOpNode.BinOp.Add:
                {
                    if (left.ValueType == ExpressionValue.Type.Number
                        && right.ValueType == ExpressionValue.Type.Number)
                    {
                        return new(left.Number + right.Number);
                    }
                    else
                    {
                        return new(left.ToString() + right.ToString());
                    }
                }

                case BinOpNode.BinOp.Subtract:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Subtraction is only valid for numbers.");
                    }

                    return new(left.Number - right.Number);
                }

                case BinOpNode.BinOp.Modulo:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Modulo is only valid for numbers.");
                    }

                    if (right.Number == 0)
                    {
                        throw new ExpressionException("Pamiętaj chemiku młody, nie dziel kurwo przez zero.");
                    }

                    return new(left.Number % right.Number);
                }

                case BinOpNode.BinOp.Divide:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Division is only valid for numbers.");
                    }

                    if (right.Number == 0)
                    {
                        throw new ExpressionException("Pamiętaj chemiku młody, nie dziel kurwo przez zero.");
                    }

                    return new(left.Number / right.Number);
                }

                case BinOpNode.BinOp.Multiply:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Multiplication is only valid for numbers.");
                    }

                    return new(left.Number * right.Number);
                }
                
                default: throw new ExpressionException($"Invalid binary operation type '{binOp.Type}'.");
            }
        }

        private ExpressionValue Visit(EntityReferenceNode entRef)
        {
            return ExpressionValue.Zero;
        }
        
        private void Visit(InvocationNode inv)
        {
            if (_commandRegistry.Exists(inv.Target.EntityName))
            {
                var args = new List<ExpressionValue>();

                foreach (var arg in inv.Arguments)
                    args.Add(Visit(arg));

                _commandRegistry.Invoke(inv.Target.EntityName, args.ToArray());
            }
            // else if (_convarRegistry.Exists(inv.Target.EntityName))
            // {
            //     
            // }
            else
            {
                throw new EntityNotFoundException(inv.Target.EntityName);
            }
        }

        private ExpressionValue Visit(StringNode str)
        {
            return new(str.Value);
        }

        private ExpressionValue Visit(NumberNode num)
        {
            return new(num.Value);
        }

        private ExpressionValue Visit(UnOpNode unOp)
        {
            var value = Visit(unOp.Right);

            if (value.ValueType != ExpressionValue.Type.Number)
            {
                throw new ExpressionException("Unary operation on a non-numerical type.");
            }

            return unOp.Type switch
            {
                UnOpNode.UnOp.Minus => new(-value.Number),
                UnOpNode.UnOp.Plus => value,
                _ => throw new ExpressionException($"Invalid unary operation '{unOp.Type}'.")
            };
        }

        private ExpressionValue Visit(ExpressionNode exprNode)
        {
            return exprNode switch
            {
                BinOpNode binOp => Visit(binOp),
                UnOpNode unOp => Visit(unOp),
                EntityReferenceNode entRef => Visit(entRef),
                StringNode str => Visit(str),
                NumberNode num => Visit(num),
                _ => throw new ExpressionException($"Invalid expression node type {exprNode.GetType().Name}")
            };
        }

        private void Visit(AstNode astNode)
        {
            if (astNode is DirectiveNode dir)
                Visit(dir);

            throw new ExpressionException($"Unsupported AST node type {astNode.GetType().Name}");
        }
    }
}
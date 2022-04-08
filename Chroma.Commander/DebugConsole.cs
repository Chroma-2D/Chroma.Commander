﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Chroma.Commander.Expressions.Lexical;
using Chroma.Commander.Expressions.Syntax;
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
                var tokens = Tokenizer.Tokenize(input);
                
                if (RawInputHandler?.Invoke(ref tokens) < 0)
                    return;

                var parser = new Parser(input);
                var directive = parser.Parse();
                
                var tokensWithoutTrigger = tokens.Skip(1).ToArray();
            }
            catch (FormatException e)
            {
                Print($"Bad input format: {e.Message}");
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
    }
}
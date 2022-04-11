using System.Collections.Generic;
using System.Linq;

namespace Chroma.Commander
{
    internal class InputHistory
    {
        private int _historyIndex = -1;
        private List<string> _history = new();

        public string CurrentEntry
        {
            get
            {
                if (_historyIndex < 0)
                    return string.Empty;

                if (_historyIndex >= _history.Count)
                    return string.Empty;

                return _history[_historyIndex];
            }
        }

        public bool IsAtEnd
             => _historyIndex == _history.Count - 1;

        public bool IsAtStart
            => _historyIndex == 0;

        public void AddToHistory(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            if (input.StartsWith(' '))
                return;

            _history.Add(input);
            End();
        }

        public void Next()
        {
            if (!_history.Any())
                return;

            _historyIndex++;

            if (_historyIndex >= _history.Count)
                _historyIndex = _history.Count - 1;
        }

        public void Previous()
        {
            if (!_history.Any())
                return;

            if (_historyIndex <= 0)
                return;

            _historyIndex--;
        }

        public void End()
        {
            _historyIndex = _history.Count;
        }
    }
}
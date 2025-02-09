using System.Collections.Generic;
using System.Linq;

namespace R3_Samples.DemoScenes
{
    public sealed class ResultText
    {
        public readonly List<string> _results = new List<string>();

        private readonly int _maxResults = 10;

        public ResultText(int maxResults)
        {
            _maxResults = maxResults;
        }

        public void AddResult(string result)
        {
            _results.Add(result);
            if (_results.Count > _maxResults)
            {
                _results.RemoveAt(0);
            }
        }

        public string ToText()
        {
            return _results.Aggregate("", (acc, x) => acc + x + "\n");
        }
    }
}
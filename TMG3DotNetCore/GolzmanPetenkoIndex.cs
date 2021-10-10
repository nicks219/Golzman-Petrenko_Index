using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TMG3DotNetCore
{
    public class GolzmanPetenkoIndex
    {
        private readonly Dictionary<float, List<string>> _dict = new();
        private readonly HashSet<float> _identicalIndices = new();
        private static readonly HashSet<char> _russian = new("01234567890ячсмитьбюфывапролджэйцукенгшщзхъёЯЧСМИТЬБЮФЫВАПРОЛДЖЭЙЦУКЕНГШЩЗХЪЁ");
        private static readonly HashSet<char> _english = new("0123456789zxcvbnmasdfghjklqwertyuiopZXCVBNMASDFGHJKLQWERTYUIOP");

        public GolzmanPetenkoIndex(Stream stream)
        {
            InitializeIndices(stream);
        }

        public HashSet<float> CreateIdenticalIndices()
        {
            _identicalIndices.Clear();
            _dict.Where(data => data.Value.Count > 1)
                .Select(data => _identicalIndices
                .Add(data.Key))
                .Count();
            return _identicalIndices;
        }

        /// <summary>
        /// Gets a list of lines with the same indices
        /// </summary>
        /// <returns>List of lines</returns>
        public List<string> GetIdenticalPhrases()
        {
            if (_identicalIndices.Count == 0)
            {
                CreateIdenticalIndices();
            }

            List<string> result = new();
            foreach (var index in _identicalIndices)
            {
                foreach (var line in _dict[index])
                {
                    result.Add(line);
                }
            }
            return result;
        }

        private void InitializeIndices(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                float index;
                string[] res = line.Split('|');
                index = CalculatePetrenkoIndex(res[0]);
                if (res.Length == 2)
                {
                    //English phrase: adds comment index
                    index += CalculatePetrenkoIndex(res[1]);
                }

                if (_dict.TryGetValue(index, out var list))
                {
                    list.Add(line);
                }
                else
                {
                    _dict.Add(index, new List<string> { line });
                }
            }
        }

        private static float CalculatePetrenkoIndex(string input)
        {
            float index = 0.5f;
            float accumulator = 0;
            int length = 0;
            foreach (var r in input)
            {
                if (_russian.Contains(r) || _english.Contains(r))
                {
                    accumulator += index;
                    index++;
                    length++;
                }
            }
            accumulator *= length;
            return accumulator;
        }
    }
}

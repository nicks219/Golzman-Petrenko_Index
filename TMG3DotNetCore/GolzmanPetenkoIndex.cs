using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TMG3DotNetCore
{
    public class GolzmanPetenkoIndex
    {
        public HashSet<float> IdenticalIndices { get; private set; } = new();
        private readonly Dictionary<float, List<string>> _generalDict = new();
        private static readonly HashSet<char> _russian = new("01234567890ячсмитьбюфывапролджэйцукенгшщзхъёЯЧСМИТЬБЮФЫВАПРОЛДЖЭЙЦУКЕНГШЩЗХЪЁ");
        private static readonly HashSet<char> _english = new("0123456789zxcvbnmasdfghjklqwertyuiopZXCVBNMASDFGHJKLQWERTYUIOP");

        public GolzmanPetenkoIndex(Stream stream)
        {
            InitializeAllIndices(stream);
            IdenticalIndices = CreateIdenticalIndices();
        }

        /// <summary>
        /// Gets a dictionary of lines with the same indices
        /// </summary>
        /// <returns>Dictionary of the identical indices and lines</returns>
        public Dictionary<float, List<string>> GetIdenticalPhrases()
        {
            return IdenticalIndices.ToDictionary(index => index, index => _generalDict[index].ToList());
        }

        private void InitializeAllIndices(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] res = line.Split('|');
                float index = CalculatePetrenkoIndex(res[0]);
                if (res.Length == 2)
                {
                    //English phrase: adds comment index
                    index += CalculatePetrenkoIndex(res[1]);
                }

                if (_generalDict.TryGetValue(index, out var list))
                {
                    list.Add(line);
                }
                else
                {
                    _generalDict.Add(index, new List<string> { line });
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

        private HashSet<float> CreateIdenticalIndices()
        {
            return 
                _generalDict
                .Where(data => data.Value.Count > 1)
                .Select(data => data.Key)
                .ToHashSet();
        }
    }
}
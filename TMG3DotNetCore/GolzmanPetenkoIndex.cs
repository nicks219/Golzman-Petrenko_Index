using System.Collections.Generic;
using System.IO;

namespace TMG3DotNetCore
{
    public class GolzmanPetenkoIndex
    {
        private readonly Dictionary<float, List<string>> _dict = new();
        private readonly HashSet<float> _identicalIndexes = new();
        private static readonly HashSet<char> _russian = new("01234567890ячсмитьбюфывапролджэйцукенгшщзхъёЯЧСМИТЬБЮФЫВАПРОЛДЖЭЙЦУКЕНГШЩЗХЪЁ");
        private static readonly HashSet<char> _english = new("0123456789zxcvbnmasdfghjklqwertyuiopZXCVBNMASDFGHJKLQWERTYUIOP");

        public GolzmanPetenkoIndex(Stream stream)
        {
            CreateIndexes(stream);
        }

        public HashSet<float> CreateIdenticalIndexes()
        {
            _identicalIndexes.Clear();
            foreach (var index in _dict)
            {
                if (index.Value.Count > 1)
                {
                    _identicalIndexes.Add(index.Key);
                }
            }
            return _identicalIndexes;
        }

        public List<string> GetIdenticalPhrases()
        {
            if (_identicalIndexes.Count == 0)
            {
                CreateIdenticalIndexes();
            }

            List<string> result = new();
            foreach (var index in _identicalIndexes)
            {
                foreach (var phrase in _dict[index])
                {
                    result.Add(phrase);
                }
            }
            return result;
        }

        private void CreateIndexes(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            string phrase;
            while ((phrase = streamReader.ReadLine()) != null)
            {
                float acc;
                string[] res = phrase.Split('|');
                acc = CalculatePetrenkoIndex(res[0]);
                if (res.Length == 2)
                {
                    //English phrase: add comments
                    acc += CalculatePetrenkoIndex(res[1]);
                }

                if (_dict.TryGetValue(acc, out var list))
                {
                    list.Add(phrase);
                }
                else
                {
                    _dict.Add(acc, new List<string> { phrase });
                }
            }
        }

        private float CalculatePetrenkoIndex(string input)
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

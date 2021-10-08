using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TMG3DotNetCore
{
    class Program
    {
        private static readonly string _russian = "01234567890ячсмитьбюфывапролджэйцукенгшщзхъёЯЧСМИТЬБЮФЫВАПРОЛДЖЭЙЦУКЕНГШЩЗХЪЁ";
        private static readonly string _english = "0123456789zxcvbnmasdfghjklqwertyuiopZXCVBNMASDFGHJKLQWERTYUIOP";
        private static readonly Dictionary<float, List<string>> _dict = new();
        private static readonly UTF8Encoding _uniEncoding = new();
        private static readonly HashSet<float> _identical = new();

        static void Main(string[] args)
        {
            byte[] firstString = _uniEncoding.GetBytes(
            "Не выходи из комнаты, не совершай ошибку.\n" +
            "Load up on guns and bring your friends. It' |    !!!\n" +
            "Hello, how low | Nirvana");
            using Stream stream = new MemoryStream(100);
            stream.Write(firstString, 0, firstString.Length);
            stream.Seek(0, SeekOrigin.Begin);

            CreateIndexes(stream);
            FindIdenticalIndexes();
            PrintIdenticalPhrases();
        }

         public static void CreateIndexes(Stream stream)
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
            Console.WriteLine();
        }

        public static void FindIdenticalIndexes()
        {
            _identical.Clear();
            foreach(var index in _dict)
            {
                if (index.Value.Count > 1)
                {
                    _identical.Add(index.Key);
                }
            }
        }

        public static void PrintIdenticalPhrases()
        {
            foreach (var index in _identical)
            {
                Console.WriteLine(index);
                foreach (var phrase in _dict[index])
                {
                    Console.WriteLine(phrase);
                }
                Console.WriteLine("\n");
            }
        }

        private static float CalculatePetrenkoIndex(string test2)
        {
            float index = 0.5f;
            float acc = 0;
            int length = 0;

            foreach (var r in test2)
            {
                bool letterOrDigitRussian = _russian.IndexOf(r) != -1;
                bool letterOrDigitEnglish = _english.IndexOf(r) != -1;
                if (letterOrDigitRussian || letterOrDigitEnglish)
                {
                    acc += index;
                    index++;
                    length++;
                }
            }
            acc *= length;
            return acc;
        }
    }
}
using System;
using System.Collections.Generic;

namespace TMG3DotNetCore
{
    class Program
    {
        private static readonly string _russian = "01234567890ячсмитьбюфывапролджэйцукенгшщзхъёЯЧСМИТЬБЮФЫВАПРОЛДЖЭЙЦУКЕНГШЩЗХЪЁ";
        private static readonly string _english = "0123456789zxcvbnmasdfghjklqwertyuiopZXCVBNMASDFGHJKLQWERTYUIOP";
        private static readonly Dictionary<float, List<string>> _dict = new Dictionary<float, List<string>>();

        static void Main(string[] args)
        {
            string[] test = new[] { "Не выходи из комнаты, не совершай ошибку.",
                "Load up on guns and bring your friends. It' |    !!!",
                "Hello, how low | Nirvana" };
            CreateIndexes(test);
            PrintIdeticalIndexes();
        }

         public static void CreateIndexes(string[] test)
        {
            float acc;
            foreach (var phrase in test)
            {
                string[] res = phrase.Split('|');
                acc = CalculatePetrenkoIndex(res[0]);
                if (res.Length == 2)
                {
                    //English: add comments
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

        public static void PrintIdeticalIndexes()
        {
            foreach(var index in _dict)
            {
                if (index.Value.Count > 1)
                {
                    Console.WriteLine(index.Key);
                    foreach(var phrase in index.Value)
                    {
                        Console.WriteLine(phrase);
                    }
                    Console.WriteLine("\n");
                }
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
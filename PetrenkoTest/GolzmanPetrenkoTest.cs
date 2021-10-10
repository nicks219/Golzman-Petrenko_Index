using NUnit.Framework;
using System.IO;
using System.Text;
using TMG3DotNetCore;
using System.Linq;

namespace PetrenkoTest
{
    public class Tests
    {
        private static readonly UTF8Encoding _uniEncoding = new();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestUsingTxtFile()
        {
            GolzmanPetenkoIndex golzmanPetenkoIndex = BuildIndices("test.txt");

            Assert.IsTrue(golzmanPetenkoIndex.IdenticalIndices.Contains(17968.5f));
            Assert.AreEqual(1, golzmanPetenkoIndex.GetIdenticalPhrases().Count);
            Assert.AreEqual("Не выходи из комнаты, не совершай ошибку.", golzmanPetenkoIndex.GetIdenticalPhrases().FirstOrDefault().Value[0]);
            Assert.AreEqual("Load up on guns and bring your friends. It' |    !!!", golzmanPetenkoIndex.GetIdenticalPhrases().FirstOrDefault().Value[1]);
        }

        [Test]
        public void TestUsingMemoryStread()
        {
            GolzmanPetenkoIndex golzmanPetenkoIndex = BuildMemoryStream();
            Assert.AreEqual("Load up on guns and bring your friends. It' |    !!!", golzmanPetenkoIndex.GetIdenticalPhrases().FirstOrDefault().Value[1]);
        }


        private GolzmanPetenkoIndex BuildIndices(string fileName)
        {
            var type = GetType();
            var path = type.FullName!.Replace(nameof(Tests), $"TestData.{fileName}");
            using var stream = type.Assembly.GetManifestResourceStream(path);
            return new GolzmanPetenkoIndex(stream);
        }

        private static GolzmanPetenkoIndex BuildMemoryStream()
        {
            byte[] firstString = _uniEncoding.GetBytes(
           "Не выходи из комнаты, не совершай ошибку.\n" +
           "Load up on guns and bring your friends. It' |    !!!\n" +
           "Hello, how low | Nirvana");
            using Stream stream = new MemoryStream(90);
            stream.Write(firstString, 0, firstString.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return new GolzmanPetenkoIndex(stream);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WordExercise.Controllers;
using WordExercise.Dtos;
using WordExercise.Services;

namespace WordApiTest.IntegrationTests
{
    [TestFixture]
    public class WordsControllerTests
    {
        private readonly Mock<IEnglishDictionaryService> _mockEnglishDictionaryService;
        private const string _boardDefinition = "DEM,RCO,AOY";

        public WordsControllerTests()
        {
            _mockEnglishDictionaryService = new Mock<IEnglishDictionaryService>();
            _mockEnglishDictionaryService.Setup(s => s.IsValidWord("ROARED")).Returns(true);
            _mockEnglishDictionaryService.Setup(s => s.IsValidWord(It.Is<string>(w => w != "ROARED"))).Returns(false);
        }

        [Test]
        public void WordsController_GetValidWords()
        {
            var controller = new WordsController(_mockEnglishDictionaryService.Object);

            var result = controller.GetValidWords(_boardDefinition);
            var response = result.Value as ValidWordsResponse;

            Assert.IsTrue(response.Words.Contains("ROARED"), "ROARED is not found");
        }
    }
}

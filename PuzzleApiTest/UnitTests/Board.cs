using Moq;
using NUnit.Framework;
using System.Linq;
using WordExercise.Business;
using WordExercise.Services;

namespace WordApiTest.UnitTests
{
    [TestFixture]
    public class BoardTests
    {
        private readonly Mock<IEnglishDictionaryService> _mockEnglishDictionaryService;

        public BoardTests()
        {
            _mockEnglishDictionaryService = new Mock<IEnglishDictionaryService>();
            _mockEnglishDictionaryService.Setup(s => s.IsValidWord(It.IsAny<string>())).Returns(true);
        }

        [Test]
        public void Board_ValidationRejectsInvalidBoard()
        {
            var boardDefinition = "AAA,BB";

            var board = new Board2(boardDefinition, _mockEnglishDictionaryService.Object);
            var isValid = board.Validate(out string message);

            Assert.IsFalse(isValid, "The given board definition is invalid.");
            Assert.AreEqual(message, "The given board cannot be defined by a square matrix.");
        }

        [Test]
        public void Board_ValidationAcceptsValidBoard()
        {
            var boardDefinition = "AAA,BBB,CCC";

            var board = new Board2(boardDefinition, _mockEnglishDictionaryService.Object);
            var isValid = board.Validate(out string message);

            Assert.IsTrue(isValid, "The given board definition is valid.");
            Assert.AreEqual(message, "");
        }

        [Test]
        public void Board_FindsAllWordCandidates()
        {
            var boardDefinition = "DEM,RCO,AOY";
            var board = new Board(boardDefinition, _mockEnglishDictionaryService.Object);
            board.Initialize();
            var allCandidates = board.GetWordCandidates().ToList();

            // A 3x3 matrix contains 16 substrings of length at least 3; here, these are
            Assert.Multiple(() =>
            {
                Assert.IsTrue(allCandidates.Contains("ROARED"));
                Assert.IsTrue(allCandidates.Contains("ORDERER"));
                Assert.IsTrue(allCandidates.Contains("RECORDER"));
                Assert.IsTrue(allCandidates.Contains("DEMOCRACY"));
            });
        }

        [Test]
        public void Board_FindsWord()
        {
            var boardDefinition = "DEM,RCO,AOY";
            var board = new Board(boardDefinition, _mockEnglishDictionaryService.Object);
            board.Initialize();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(board.WordExists("ROARED"));
                Assert.IsTrue(board.WordExists("ORDERER"));
                Assert.IsTrue(board.WordExists("RECORDER"));
                Assert.IsTrue(board.WordExists("DEMOCRACY"));
            });
        }

        [Test]
        public void Board_FindsLongWordCandidatesIn20x20Matrix()
        {
            var boardDefinition = "VALANTUTBOTSTTNECODA,TESISPONYSDYNNEVLREP,TODEPUUALEETEMEEAIAA,BEEUOTPERLDIMEVCNLSL,ARRETEAMIENMHEVFETTA,UIIYDEACACERSTEEPDIA,NEVOETIMTTCOIRETNIRP,TIEROTNUAOEFNODESAEL,YRSLIVERQRDIOTNACEDA,YSOBREPUSAANMPRHLRUC,AGONYCIXOTRUDUNUAERI,YACNIZCILERTAOHUMPUT,TSIPAREHTERAPSMULPGS,STUNODENEPOHHSTATUEI,DNEUAREPOTOSSUPSIRCT,NYTNEMESABUROUNDUNLA,AREPEETSIRUTCNUPUCAT,RACCENTABEMORDNILAPS,TSPUCELBATROPSTUBRAT,SSSSPELTENORRABRATES";
            var board = new Board2(boardDefinition, _mockEnglishDictionaryService.Object);
            var allCandidates = board.GetWordCandidates().ToList();

            Assert.IsTrue(allCandidates.Count == 22116, "A 16x16 matrix contains 22116 substrings of length at least 3");

            Assert.IsTrue(allCandidates.Contains("SYNOPSIS"));
            Assert.IsTrue(allCandidates.Contains("QUIETUDE"));
            Assert.IsTrue(allCandidates.Contains("PALINDROME"));
            Assert.IsTrue(allCandidates.Contains("UNIFORMITY"));
            Assert.IsTrue(allCandidates.Contains("DERMATOLOGY"));
            Assert.IsTrue(allCandidates.Contains("ADMONISHMENT"));
            Assert.IsTrue(allCandidates.Contains("ARACHNOPHOBIA"));
        }

        [Test]
        public void Board_WordExistsReturnsTrueIfGivenWordExistsOnBoard()
        {
            var boardDefinition = "VALANTUTBOTSTTNECODA,TESISPONYSDYNNEVLREP,TODEPUUALEETEMEEAIAA,BEEUOTPERLDIMEVCNLSL,ARRETEAMIENMHEVFETTA,UIIYDEACACERSTEEPDIA,NEVOETIMTTCOIRETNIRP,TIEROTNUAOEFNODESAEL,YRSLIVERQRDIOTNACEDA,YSOBREPUSAANMPRHLRUC,AGONYCIXOTRUDUNUAERI,YACNIZCILERTAOHUMPUT,TSIPAREHTERAPSMULPGS,STUNODENEPOHHSTATUEI,DNEUAREPOTOSSUPSIRCT,NYTNEMESABUROUNDUNLA,AREPEETSIRUTCNUPUCAT,RACCENTABEMORDNILAPS,TSPUCELBATROPSTUBRAT,SSSSPELTENORRABRATES";
            var board = new Board2(boardDefinition, _mockEnglishDictionaryService.Object);

            Assert.IsTrue(board.WordExists("quietude"), "The word 'quietude' appears on the board.");
            Assert.IsFalse(board.WordExists("quietudes"), "The word 'quietudes' does not appear on the board.");
        }
    }
}

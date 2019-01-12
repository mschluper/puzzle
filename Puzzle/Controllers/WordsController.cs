using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordExercise.Business;
using WordExercise.Dtos;
using WordExercise.Services;

namespace WordExercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private static IEnglishDictionaryService _englishDictionaryService;
        const int allottedNumberOfSeconds = 5;

        public WordsController(IEnglishDictionaryService englishDictionaryService)
        {
            if (_englishDictionaryService == null)
            {
                _englishDictionaryService = englishDictionaryService;
                _englishDictionaryService.Initialize();
            }
        }

        // GET: api/Words/TTZZA,ONIZU,TEIUQ,EPZUQ,ISAUQ/QUINT
        [HttpGet("{boardDefinition}/{word}", Name = "DoesWordExistOnBoard")]
        public ActionResult DoesWordExist(string boardDefinition, string word)
        {
            var board = new Board(boardDefinition, _englishDictionaryService);
            (var isValid, var message) = board.Validate();
            if (!isValid)
            {
                return BadRequest(message);
            }

            return Ok(board.WordExists(word));
        }

        // GET: api/Words/TTZZA,ONIZU,TEIUQ,EPZUQ,ISAUQ or api/Words/LHEALTHERE,AEALEAPRCH,RPEARLOAEA,OEADIEECTE,ILRMMNTLOK,VTZZGTWTIT,ANIZAIEOIS,HEIUUCNILE,EZIUQXREUU,BUMPOORTAQ
        [HttpGet("v2/{boardDefinition}", Name = "GetValidWords2")]
        public ObjectResult GetValidWords2(string boardDefinition)
        {
            var board = new Board2(boardDefinition, _englishDictionaryService);

            if (!board.Validate(out string validationMessage))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse { Message = validationMessage });
            }

            var endTime = DateTime.Now.AddSeconds(allottedNumberOfSeconds);

            var validWords = new List<string>();
            using (board.GetWordCandidates().ToObservable()
                .TakeUntil(endTime)
                .Subscribe(s =>
                {
                    if (_englishDictionaryService.IsValidWord(s))
                    {
                        validWords.Add(s);
                    }
                }))
            {
                Debug.WriteLine($">>> Found {validWords.Count} valid words on the board in the allotted time ({allottedNumberOfSeconds} seconds).");
            }

            var boardDisplay = boardDefinition.Split(new char[] { ',' }).ToList();
            return StatusCode(StatusCodes.Status200OK, new ValidWordsResponse
            {
                Board = boardDisplay,
                Words = validWords.Distinct().OrderBy(w => w).ToList()
            });
        }

        // GET: api/Words/TTZZA,ONIZU,TEIUQ,EPZUQ,ISAUQ or api/Words/LHEALTHERE,AEALEAPRCH,RPEARLOAEA,OEADIEECTE,ILRMMNTLOK,VTZZGTWTIT,ANIZAIEOIS,HEIUUCNILE,EZIUQXREUU,BUMPOORTAQ
        [HttpGet("{boardDefinition}", Name = "GetValidWords")]
        public ObjectResult GetValidWords(string boardDefinition)
        {
            var board = new Board(boardDefinition, _englishDictionaryService);

            (var isValid, var message) = board.Validate();
            if (!isValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse { Message = message });
            }

            board.Initialize();

            var endTime = DateTime.Now.AddSeconds(allottedNumberOfSeconds);

            var validWords = new List<string>();
            using (board.GetWordCandidates().ToObservable()
                .TakeUntil(endTime)
                .Subscribe(s =>
                {
                    if (s.Length >= 3 && _englishDictionaryService.IsValidWord(s))
                    {
                        validWords.Add(s);
                    }
                }))
            {
                Debug.WriteLine($">>> Found {validWords.Count} valid words on the board in the allotted time ({allottedNumberOfSeconds} seconds).");
            }

            var boardDisplay = boardDefinition.Split(new char[] { ',' }).ToList();
            return StatusCode(StatusCodes.Status200OK, new ValidWordsResponse
            {
                Board = boardDisplay,
                Words = validWords.Distinct().OrderBy(w => w).ToList()
            });
        }
    }
}

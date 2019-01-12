using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WordExercise.Services;

namespace WordExercise.Business
{
    public class Board2
    {
        private string _boardDefinition;
        private IEnglishDictionaryService _englishDictionaryService;

        public Board2(string boardDefinition, IEnglishDictionaryService englishDictionaryService)
        {
            _boardDefinition = boardDefinition;
            _englishDictionaryService = englishDictionaryService;
        }

        public bool Validate(out string message)
        {
            message = String.Empty;
            var BoardRows = _boardDefinition.Split(new char[] { ',' });
            var size = BoardRows.Length;
            // The board is square so each row must have [size] characters
            var isValid = BoardRows.Where(r => r.Length == size).Count() == size;
            if (!isValid)
            {
                message = "The given board cannot be defined by a square matrix.";
                return false;
            }
            return true;
        }

        [Conditional("DEBUG")]
        private void Delay()
        {
            //System.Threading.Thread.Sleep(500);
        }

        public IEnumerable<string> GetWordCandidates()
        {
            var BoardRows = _boardDefinition.Split(new char[] { ',' });
            var size = BoardRows.Length;
            // Horizontal
            foreach (string row in BoardRows)
            {
                for (int i = 0; i < size; i++)
                {
                    // Candidates's length must be at least 3
                    for (int j = 3; j <= size - i; j++)
                    {
                        var candidate = row.Substring(i, j);
                        yield return candidate;
                        var candidateAsArray = candidate.ToCharArray();
                        Array.Reverse(candidateAsArray);
                        yield return new String(candidateAsArray);
                    }
                }
            }

            Delay();

            // Vertical
            var columns = new string[size];
            for (int i = 0; i < size; i++)
            {
                columns[i] = new string(BoardRows.Select(row => row[i]).ToArray());
            }
            foreach (string column in columns)
            {
                for (int i = 0; i < size; i++)
                {
                    // Candidates's length must be at least 3
                    for (int j = 3; j <= size - i; j++)
                    {
                        var candidate = column.Substring(i, j);
                        yield return candidate;
                        var candidateAsArray = candidate.ToCharArray();
                        Array.Reverse(candidateAsArray);
                        yield return new String(candidateAsArray);
                    }
                }
            }

            Delay();

            // Diagonal-positive slope: Shift left first, then same as columns (above)
            var shiftedRows = new List<string>();
            var rowIndex = 0;
            BoardRows.ToList().ForEach(row => shiftedRows.Add(new string(' ', rowIndex) + row + new string(' ', size - rowIndex++)));
            var diagonals = new string[2 * size - 1];
            for (int i = 0; i < 2 * size - 1; i++)
            {
                diagonals[i] = new string(shiftedRows.Select(row => row[i]).ToArray()).Trim();
            }
            foreach (string diagonal in diagonals)
            {
                for (int i = 0; i < size; i++)
                {
                    // Candidates's length must be at least 3
                    var trimmedSize = diagonal.Length;
                    for (int j = 3; j <= trimmedSize - i; j++)
                    {
                        var candidate = diagonal.Substring(i, j);
                        yield return candidate;
                        var candidateAsArray = candidate.ToCharArray();
                        Array.Reverse(candidateAsArray);
                        yield return new String(candidateAsArray);
                    }
                }
            }

            Delay();

            // Diagonal-negative slope: Shift right first, then same as columns (above)
            shiftedRows = new List<string>();
            rowIndex = 0;
            BoardRows.ToList().ForEach(row => shiftedRows.Add(new string(' ', size - rowIndex - 1) + row + new string(' ', rowIndex++)));
            diagonals = new string[2 * size - 1];
            for (int i = 0; i < 2 * size - 1; i++)
            {
                diagonals[i] = new string(shiftedRows.Select(row => row[i]).ToArray()).Trim();
            }
            foreach (string diagonal in diagonals)
            {
                for (int i = 0; i < 2 * size - 1; i++)
                {
                    // Candidates's length must be at least 3
                    var trimmedSize = diagonal.Length;
                    for (int j = 3; j <= trimmedSize - i; j++)
                    {
                        var candidate = diagonal.Substring(i, j);
                        yield return candidate;
                        var candidateAsArray = candidate.ToCharArray();
                        Array.Reverse(candidateAsArray);
                        yield return new String(candidateAsArray);
                    }
                }
            }
        }

        public bool WordExists(string word)
        {
            if (!_englishDictionaryService.IsValidWord(word))
            {
                return false;
            }

            var candidates = GetWordCandidates();
            var comparer = new StringComparerCaseInsensitive();
            return candidates.Contains(word, comparer);
        }
    }
}

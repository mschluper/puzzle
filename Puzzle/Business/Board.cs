using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WordExercise.Services;

namespace WordExercise.Business
{
    public class Board
    {
        private string _boardDefinition;
        private IEnglishDictionaryService _englishDictionaryService;
        private List<Tile> _tiles;

        public Board(string boardDefinition, IEnglishDictionaryService englishDictionaryService)
        {
            _boardDefinition = boardDefinition;
            _englishDictionaryService = englishDictionaryService;
            _tiles = new List<Tile>();
        }

        public (bool, string) Validate()
        {
            var message = String.Empty;
            var BoardRows = _boardDefinition.Split(new char[] { ',' });
            var size = BoardRows.Length;
            // The board is square so each row must have [size] characters
            var isValid = BoardRows.Where(r => r.Length == size).Count() == size;
            if (!isValid)
            {
                message = "The given board cannot be defined by a square matrix.";
                return (false, message);
            }
            return (true, message);
        }

        public void Initialize()
        {
            var rows = _boardDefinition.Split(new char[] { ',' });
            var size = rows.Length;
            foreach (var row in rows)
            {
                for (var i = 0; i < row.Length; i++)
                {
                    _tiles.Add(new Tile(row[i]));
                }
            }

            if (size <= 1)
            {
                return;
            }

            // tiles on corners; each have three neighbors
            _tiles[0].AddNeighbor(_tiles[1]);
            _tiles[0].AddNeighbor(_tiles[size]);
            _tiles[0].AddNeighbor(_tiles[size + 1]);
            _tiles[size - 1].AddNeighbor(_tiles[size - 2]);
            _tiles[size - 1].AddNeighbor(_tiles[2 * size - 1]);
            _tiles[size - 1].AddNeighbor(_tiles[2 * size - 2]);
            _tiles[size * (size - 1)].AddNeighbor(_tiles[size * (size - 2)]);
            _tiles[size * (size - 1)].AddNeighbor(_tiles[size * (size - 2) + 1]);
            _tiles[size * (size - 1)].AddNeighbor(_tiles[size * (size - 1) + 1]);
            _tiles[size * size - 1].AddNeighbor(_tiles[size * (size - 1) - 2]);
            _tiles[size * size - 1].AddNeighbor(_tiles[size * (size - 1) - 1]);
            _tiles[size * size - 1].AddNeighbor(_tiles[size * size - 2]);

            if (size <= 2)
            {
                return;
            }

            // tiles on sides that are not corners; each have five neighbors; these appear in first and last row, firt and last column
            // first row
            for (var i = 1; i < size - 1; i++)
            {
                _tiles[i].AddNeighbor(_tiles[i - 1]);
                _tiles[i].AddNeighbor(_tiles[size + i - 1]);
                _tiles[i].AddNeighbor(_tiles[size + i]);
                _tiles[i].AddNeighbor(_tiles[size + i + 1]);
                _tiles[i].AddNeighbor(_tiles[i + 1]);
            }
            // last row
            var offset = size * (size - 1);
            for (var i = 1; i < size - 1; i++)
            {
                _tiles[offset + i].AddNeighbor(_tiles[offset + i - 1]);
                _tiles[offset + i].AddNeighbor(_tiles[offset - size + i - 1]);
                _tiles[offset + i].AddNeighbor(_tiles[offset - size + i]);
                _tiles[offset + i].AddNeighbor(_tiles[offset - size + i + 1]);
                _tiles[offset + i].AddNeighbor(_tiles[offset + i + 1]);
            }
            // first column
            for (var i = size; i < size * (size - 1); i = i + size)
            {
                _tiles[i].AddNeighbor(_tiles[i - size]);
                _tiles[i].AddNeighbor(_tiles[i - size + 1]);
                _tiles[i].AddNeighbor(_tiles[i + 1]);
                _tiles[i].AddNeighbor(_tiles[i + size + 1]);
                _tiles[i].AddNeighbor(_tiles[i + size]);
            }
            // last column
            for (var i = 2 * size - 1; i <= size * (size - 1); i = i + size)
            {
                _tiles[i].AddNeighbor(_tiles[i - size]);
                _tiles[i].AddNeighbor(_tiles[i - size - 1]);
                _tiles[i].AddNeighbor(_tiles[i - 1]);
                _tiles[i].AddNeighbor(_tiles[i + size - 1]);
                _tiles[i].AddNeighbor(_tiles[i + size]);
            }

            // other tiles: 8 neighbors (3 on row above, 2 on same row, 3 on row below)
            for (var i = 1; i < size - 1; i++)      // i is row number (0 ... size-1)
            {
                for (var j = 1; j < size - 1; j++)  // j is column number (0 ... size-1)
                {
                    _tiles[i + j * size].AddNeighbor(_tiles[i + (j - 1) * size - 1]);
                    _tiles[i + j * size].AddNeighbor(_tiles[i + (j - 1) * size]);
                    _tiles[i + j * size].AddNeighbor(_tiles[i + (j - 1) * size + 1]);

                    _tiles[i + j * size].AddNeighbor(_tiles[i + j * size - 1]);
                    _tiles[i + j * size].AddNeighbor(_tiles[i + j * size + 1]);

                    _tiles[i + j * size].AddNeighbor(_tiles[i + (j + 1) * size - 1]);
                    _tiles[i + j * size].AddNeighbor(_tiles[i + (j + 1) * size]);
                    _tiles[i + j * size].AddNeighbor(_tiles[i + (j + 1) * size + 1]);
                }
            }
        }

        public IEnumerable<string> GetWordCandidates(int maxLoops = 8)
        {
            _tiles.ForEach(t => t.Consume(new List<string> { String.Empty }));

            var iterationCount = 0;
            while (iterationCount < maxLoops)
            {
                _tiles.ForEach(t => t.Freeze());
                _tiles.ForEach(t => t.Push());
                foreach (var tile in _tiles)
                {
                    var candidates = tile.Pull();
                    foreach (var candidate in candidates)
                    {
                        //Debug.WriteLine("+++ " + candidate);
                        yield return candidate;
                    }
                }
                iterationCount++;
            }
        }

        public bool WordExists(string word)
        {
            if (!_englishDictionaryService.IsValidWord(word))
            {
                return false;
            }

            var answer = false;
            foreach (var tile in _tiles)
            {
                answer = answer || tile.CanBeRead(word);
                if (answer) break;
            }
            return answer;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordExercise.Business
{
    public class Tile
    {
        private char _letter;
        private IEnumerable<string> _candidates;
        private IEnumerable<string> _frozenCandidates;
        private List<Tile> _neighbors;

        public Tile(char letter)
        {
            _letter = letter;
            _candidates = new List<string>();
            _neighbors = new List<Tile>();
        }

        public void AddNeighbor(Tile tile)
        {
            _neighbors.Add(tile);
        }

        public void Freeze()
        {
            _frozenCandidates = _candidates.ToList();
            _candidates = new List<string>();
        }

        public void Push()
        {
            _neighbors.ForEach(n => n.Consume(_frozenCandidates));
        }

        public void Consume(IEnumerable<string> candidates)
        {
            _candidates = _candidates.Concat(candidates.Select(c => _letter + c));
        }

        public IEnumerable<string> Pull()
        {
            return _candidates;
        }

        public bool CanBeRead(string sequence)
        {
            if (sequence[0] != _letter)
            {
                return false;
            }
            if (sequence.Length == 1)
            {
                return true;
            }
            var answer = false;
            foreach(var tile in _neighbors)
            {
                answer = answer || tile.CanBeRead(sequence.Substring(1));
                if (answer) break;
            }
            return answer;
        }

    }

    //public static class ExtensionMethods
    //{
    //    public static bool AllTrue(this List<Tile> tiles, Func<Tile, string, bool> question)
    //    {
    //        var answer = true;

    //        foreach (var tile in tiles)
    //        {
    //            answer = answer && question(tile);
    //        }
    //        return answer;
    //    }
    //}
}

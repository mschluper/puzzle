using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordExercise.Dtos
{
    public class ValidWordsResponse
    {
        public List<string> Board { get; set; }

        public List<string> Words { get; set; }
    }
}

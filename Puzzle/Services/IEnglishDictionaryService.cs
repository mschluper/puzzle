using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordExercise.Services
{
    public interface IEnglishDictionaryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void Initialize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        bool IsValidWord(string word);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetAllWordsAsync();

        Task<string> LookUpAsync(string word);
    }
}

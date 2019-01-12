using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WordExercise.Services
{
    public class EnglishDictionaryService : IEnglishDictionaryService
    {
        const string dictionaryUrl = "https://raw.githubusercontent.com/matthewreagan/webstersEnglishDictionary/master/dictionary.json";
        private HashSet<string> _words;
        private bool isInitialized = false;

        public EnglishDictionaryService()
        {
            
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            // The spec just asks for validation of words, not for looking up their definition.
            // Therefore lets just keep the minimum in memory: the collection of valid words.
            _words = GetDictionary().Result.Keys.ToHashSet();
            isInitialized = true;
        }

        public async Task<IEnumerable<string>> GetAllWordsAsync()
        {
            var dictionary = await GetDictionary();
            return dictionary.Keys;
        }

        public async Task<string> LookUpAsync(string word)
        {
            var dictionary = await GetDictionary();
            if (dictionary.TryGetValue(word.ToLower(), out string meaning))
            {
                return meaning;
            }
            return String.Empty;
        }

        public bool IsValidWord(string word)
        {
            return _words.Contains(word.ToLower());
        }

        private async Task<Dictionary<string,string>> GetDictionary()
        {
            var words = new Dictionary<string,string>();

            using (HttpClient client = new HttpClient())

            //Setting up the response...         

            using (HttpResponseMessage res = await client.GetAsync(dictionaryUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();

                if (data != null)
                {
                    words = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                }

                return words;
            }
        }
    }
}

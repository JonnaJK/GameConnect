using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using System.Text.RegularExpressions;

namespace GameConnect.Helpers
{
    public static class BannedWordHelper
    {
        public static string CensorWords(this string text, List<BannedWord>? bannedWords)
        {
            string result = text;

            if(bannedWords == null)
                return result;

            foreach (var word in bannedWords)
            {
                if (result.ToLower().Contains(word.Title.ToLower()))
                {
                    string pattern = $@"\b{word.Title}\b";

                    string replace = @"";
                    for (int i = 0; i < word.Title.Length; i++)
                    {
                        replace += "*";
                    }

                    RegexOptions options = RegexOptions.IgnoreCase;
                    Regex regex = new Regex(pattern, options);
                    result = regex.Replace(result, replace);
                }
            }
            return result;
        }
    }
}

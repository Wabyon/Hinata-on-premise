using System.Collections.Generic;

namespace Hinata.Search
{
    public class SearchCondition
    {
        public ICollection<string> KeyWords { get; private set; }

        public bool IncluidePrivate { get; set; }

        public SearchCondition()
        {
            KeyWords = new List<string>();
        }

        public SearchCondition(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                KeyWords = new List<string>();
                return;
            }

            KeyWords = param.Replace("　", " ").Split(' ');
        }
    }
}
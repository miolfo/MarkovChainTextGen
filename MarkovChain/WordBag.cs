using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkovChain
{
    class WordBag
    {
        public List<string> Words { get; private set; }
        private List<int> counts;

        public WordBag()
        {
            Words = new List<string>();
            counts = new List<int>();
        }

        public void Add(string word)
        {
            string handlable = word.ToLower();
            int i = getIndexOf(handlable);
            if (i == -1)
            {
                Words.Add(handlable);
                counts.Add(1);
            }
            else
            {
                counts[i]++;
            }
        }

        public int GetOccurrences(string word)
        {
            string handlable = word.ToLower();
            int i = getIndexOf(handlable);
            if (i == -1) throw new KeyNotFoundException();
            return counts[i];
        }

        private int getIndexOf(string word)
        {
            string handlable = word.ToLower();
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].Equals(handlable)) return i;
            }
            return -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkovChain
{
    class MarkovChainGenerator
    {
        Random r;
        Dictionary<string[], WordBag> transitionMap;
        public MarkovChainGenerator()
        {
            transitionMap = new Dictionary<string[], WordBag>();
            r = new Random();
        }

        public void Train(string text)
        {
            //Split the text into sentences
            string[] sentences = text.Split('.');
            string[] trimmed = new string[sentences.Length];
            for(int i = 0; i < sentences.Length; i++)
            {
                //string sentence = sentences[i].TrimStart(' ');
                //trimmed[i] = sentence;
                string sentence = sentences[i];
                if (sentence.Length > 0 && sentence[0] == ' ')
                {
                    sentence = sentence.Remove(0, 1);
                }
                trimmed[i] = sentence;
            }

            //If any empty sentences, remove them


            for(int i = 0; i < trimmed.Length; i++)
            {
                string[] words = trimmed[i].Split(' ');
                if (words.Length < 2) continue;
                for(int j = -1; j < words.Length; j++)
                {
                    //if(j == 0) Console.WriteLine(words[0] + "!");
                    string[] keys = new string[2];
                    if(j == -1)
                    {
                        keys[0] = "";
                        keys[1] = "";
                    }
                    else if (j == 0) keys[0] = "";
                    else keys[0] = words[j - 1];

                    if(j != -1) keys[1] = words[j];
                    
                    if (!mapContainsKey(keys))
                    {
                        WordBag wb = new WordBag();
                        if (j + 1 < words.Length) wb.Add(words[j + 1]);
                        transitionMap.Add(keys, wb);
                    }
                    else
                    {
                        int index = getIndexOfKey(keys);
                        if (j + 1 < words.Length) transitionMap.ElementAt(index).Value.Add(words[j + 1]);
                    }
                }
            }
        }

        public string GenerateSentence(int maxLength)
        {
            StringBuilder sb = new StringBuilder();
            string[] sentence = new string[maxLength];
            for(int i = 0; i < maxLength; i++)
            {
                //First, find the ["",""] key to get possible starting words
                if (i == 0)
                {
                    int index = getIndexOfKey(new string[] { "", "" });
                    if (index == -1) break;
                    string startingWord = "";
                    //For some reason, empty words still exist
                    while (startingWord.Equals(""))
                    {
                        startingWord = determineWord(transitionMap.ElementAt(index).Value);
                    }
                    sentence[i] = capitalize(startingWord);
                }
                //Else if in second iteration, find a key with ["",sentence[0]] 
                else if(i == 1)
                {
                    int index = getIndexOfKey(new string[] { "", sentence[0] });
                    string word = determineWord(transitionMap.ElementAt(index).Value);
                    sentence[i] = word;
                }
                //After first two iterations
                else
                {
                    int index = getIndexOfKey(new string[] { sentence[i - 2], sentence[i - 1] });
                    if (index == -1) break;
                    string word = determineWord(transitionMap.ElementAt(index).Value);
                    if (!word.Equals("")) sentence[i] = word;
                }
            }
            string sentenceString = string.Join(" ", sentence);
            string trimmed = sentenceString.TrimEnd(' ');
            return trimmed + ".";
        }

        public void PrintTransitionMap()
        {
            foreach(KeyValuePair<string[], WordBag> pair in transitionMap)
            {
                Console.Write("[\"" + pair.Key[0] + "\",\"" + pair.Key[1]+"\"] = {");
                List<string> words = pair.Value.Words;
                foreach(string s in words)
                {
                    Console.Write("\"" + s + "\" = " + pair.Value.GetOccurrences(s) + ",");
                }
                Console.Write("}\n");
            }
        }

        private string determineWord(WordBag wb)
        {
            List<string> words = wb.Words;
            List<string> accumulatedWords = new List<string>();
            foreach(string word in words)
            {
                int occ = wb.GetOccurrences(word);
                for (int i = 0; i < occ; i++) accumulatedWords.Add(word);
            }
            if (accumulatedWords.Count == 0) return "";
            int rndm = r.Next(accumulatedWords.Count - 1);
            return accumulatedWords[rndm];
        }

        private string capitalize(string word)
        {
            if (String.IsNullOrEmpty(word))
                return "";
            return word.First().ToString().ToUpper() + word.Substring(1);
        }

        private bool mapContainsKey(string[] key)
        {
            foreach(KeyValuePair<string[], WordBag> pair in transitionMap)
            {
                if (sameKey(key, pair.Key)) return true;
            }
            return false;
        }

        private int getIndexOfKey(string[] key)
        {
            for(int i = 0; i < transitionMap.Count; i++)
            {
                if (sameKey(transitionMap.ElementAt(i).Key, key)) return i;
            }
            return -1;
        }

        private bool sameKey(string[] k1, string[] k2)
        {

            for(int i = 0; i < k1.Length; i++)
            {
                //TODO: Why error sometimes wtf
                try
                {
                    if (k1[i].ToLower().Equals(k2[i].ToLower())) continue;
                    else return false;
                }
                catch (Exception)
                {
                    //Console.WriteLine("asd?");
                }
            }
            return true;
        }
    }
}

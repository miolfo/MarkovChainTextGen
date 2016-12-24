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
            //string[] trimmed = new string[sentences.Length];
            List<string> trimmedSentences = new List<string>();
            for (int i = 0; i < sentences.Length; i++)
            {
                string sentence = sentences[i];
                string trm = trimSentence(sentence);
                if (!trm.Equals("")) trimmedSentences.Add(trm);
            }

            string[] trimmed = trimmedSentences.ToArray();

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
                        if (j + 1 < words.Length) addWordToKey(keys, words[j + 1]);
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
                    string startingWord = "";
                    WordBag bag = getWordBagOfKey(new string[] { "", "" });
                    while (startingWord.Equals(""))
                    {
                        startingWord = determineWord(bag);
                    }
                    sentence[i] = capitalize(startingWord);
                }
                //Else if in second iteration, find a key with ["",sentence[0]] 
                else if(i == 1)
                {
                    /*int index = getIndexOfKey(new string[] { "", sentence[0] });
                    string word = determineWord(transitionMap.ElementAt(index).Value);*/
                    WordBag bag = getWordBagOfKey(new string[] { "", sentence[0] });
                    string word = determineWord(bag);
                    sentence[i] = word;
                }
                //After first two iterations
                else
                {
                    /*int index = getIndexOfKey(new string[] { sentence[i - 2], sentence[i - 1] });
                    if (index == -1) break;
                    string word = determineWord(transitionMap.ElementAt(index).Value);
                    if (!word.Equals("")) sentence[i] = word;*/
                    WordBag bag = getWordBagOfKey(new string[] { sentence[i - 2], sentence[i - 1] });
                    if (bag == null) break;
                    string word = determineWord(bag);
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

        private string trimSentence(string untrimmed)
        {
            untrimmed = untrimmed.Trim();
            return untrimmed;
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
            int rndm = r.Next(accumulatedWords.Count);
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
            //Since the key is a string[], we can't use ContainsKey() function.
            //This loop is a performance bottleneck
            //TODO: Concat keys into string instead of string[]?
            foreach(KeyValuePair<string[], WordBag> pair in transitionMap)
            {
                if (sameKey(key, pair.Key)) return true;
            }
            return false;
        }

        private WordBag getWordBagOfKey(string[] key)
        {
            foreach(var pair in transitionMap)
            {
                if (sameKey(pair.Key, key)) return pair.Value;
            }
            return null;
        }

        private void addWordToKey(string[] key, string word)
        {
            foreach(var pair in transitionMap)
            {
                if (sameKey(pair.Key, key)) pair.Value.Add(word);
            }
        }

        private bool sameKey(string[] k1, string[] k2)
        {
            for(int i = 0; i < k1.Length; i++)
            {
                if (k1[i] == null || k2[i] == null) continue;
                if (k1[i].ToLower().Equals(k2[i].ToLower())) continue;
                else return false;
            }
            return true;
        }
    }
}

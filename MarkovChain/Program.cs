using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkovChain
{
    class Program
    {
        static void Main(string[] args)
        {
            MarkovChainGenerator gen = new MarkovChainGenerator();
            gen.Train(SampleText.sample);
            gen.PrintTransitionMap();

            for (int i = 0; i < 20; i++)
            {
                string generated = gen.GenerateSentence(15);
                Console.WriteLine("generated sentence: ");
                Console.WriteLine(generated);
            }
        }
    }
}

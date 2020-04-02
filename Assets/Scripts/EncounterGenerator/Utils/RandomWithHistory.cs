using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Utils
{
    // TODO: Can't we think of a better name?
    // Class that can generate a random element from a given set, recording history of generated elements and avoids generating of the same elements multiple times in a row. 
    class RandomWithHistory<T>
    {
        Dictionary<T, int> RandomWeights = new Dictionary<T, int>();

        public T RandomElementFromSequence(IEnumerable<T> sequence)
        {
            if (!sequence.Any())
            {
                return default;
            }
            var toReturn =  sequence.GetWeightedRandomElementOrDefault(element => RandomWeights.ContainsKey(element) ? RandomWeights[element] : 100);
            foreach (var key in RandomWeights.Keys.ToList())
            {
                RandomWeights[key]++;
            }
            RandomWeights[toReturn] = 1;
            return toReturn;
        }
    }
}

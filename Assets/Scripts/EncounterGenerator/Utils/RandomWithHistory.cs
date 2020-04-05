using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extension;

namespace Assets.Scripts.EncounterGenerator.Utils
{
    // TODO: Can't we think of a better name?
    // Class that can generate a random element from a given set, recording history of generated elements and avoids generating of the same elements multiple times in a row. 
    class RandomWithHistory<T>
    {
        readonly Dictionary<T, int> randomWeights = new Dictionary<T, int>();

        public T RandomElementFromSequence(IEnumerable<T> sequence)
        {
            if (!sequence.Any())
            {
                return default;
            }
            var toReturn =  sequence.GetWeightedRandomElementOrDefault(element => randomWeights.ContainsKey(element) ? randomWeights[element] : 100);
            foreach (var key in randomWeights.Keys.ToList())
            {
                randomWeights[key]++;
            }
            randomWeights[toReturn] = 1;
            return toReturn;
        }
    }
}

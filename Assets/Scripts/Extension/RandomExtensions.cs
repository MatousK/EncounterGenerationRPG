using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Extension
{
    /// <summary>
    /// Gives IEnumerable the option to retrieve a random element of the sequence, possibly with a weight.
    /// </summary>
    static class RandomExtensions
    {
        /// <summary>
        /// The delegate for calculating a weight for an element in an enumerable.
        /// The larger the number, the greater the likelihood of being selected.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="element">The element for which we want to know the weight.</param>
        /// <returns>Weight of the element </returns>
        public delegate float WeightSelector<in T>(T element);
        /// <summary>
        /// Retrieve a random element from the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">Sequence of elements from which we want a random element</param>
        /// <returns>Random element from the sequence.</returns>
        public static T GetRandomElementOrDefault<T>(this IEnumerable<T> sequence)
        {
            return sequence.GetWeightedRandomElementOrDefault(p => 1);
        }
        /// <summary>
        /// Retrieve a random element from the sequence. Elements have a weight assigned by <paramref name="weightSelector"/>. Elements with larger weight are more likely to be selected.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence of elements for which we want a random element.</param>
        /// <param name="weightSelector">Assigns each element its weight.</param>
        /// <returns>The random element.</returns>
        public static T GetWeightedRandomElementOrDefault<T>(this IEnumerable<T> sequence, WeightSelector<T> weightSelector)
        {
            sequence = sequence.Where(element => weightSelector(element) > 0);
            var totalWeight = sequence.Sum(element => weightSelector(element));
            var requestedElement = UnityEngine.Random.Range(0, totalWeight);
            foreach (var element in sequence)
            {
                var elementWeight = weightSelector(element);
                if (requestedElement <= elementWeight)
                {
                    return element;
                }
                requestedElement -= elementWeight;
            }
            UnityEngine.Debug.Assert(!sequence.Any(), "We should always get a result if the sequence is not empty.");
            return default;
        }
    }
}
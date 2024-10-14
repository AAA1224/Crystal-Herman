using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KoboldTools
{
    /// <summary>
    /// Provides extensions to lists.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">Any type.</typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        /// <summary>
        /// Randomly selects the specified number of elements from the given
        /// list without placing back elements.
        /// </summary>
        /// <returns>The randomly chosen list elements.</returns>
        /// <param name="list">List.</param>
        /// <param name="number">Number.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> SelectRandom<T>(this IList<T> list, int number)
        {
            var output = new List<T>();

            if (number <= list.Count)
            {
                var indices = Enumerable.Range(0, list.Count).ToList();
                var rng = new System.Random();
                for (var i = 0; i < number; i++)
                {
                    var j = rng.Next(indices.Count);
                    output.Add(list[indices[j]]);
                    indices.RemoveAt(j);
                }
            }

            return output;
        }
        /// <summary>
        /// Creates a deep string representation of a generic list.
        /// </summary>
        /// <returns>The string representation of the list.</returns>
        /// <param name="list">List.</param>
        public static string ToVerboseString<T>(this IList<T> list)
        {
            return String.Format("[{0}]", String.Join(", ", list.Select(e => e.ToString()).ToArray()));
        }
    }
}

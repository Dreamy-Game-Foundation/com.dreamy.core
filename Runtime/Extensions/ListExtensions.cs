using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dreamy.Core
{
    public static class ListExtensions
    {
        /// <summary>Executes <paramref name="action"/> for each element in the sequence.</summary>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
                action(item);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
            => list == null || list.Count == 0;

        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty())
                throw new InvalidOperationException("Cannot get random element from a null or empty list.");
            return list[Random.Range(0, list.Count)];
        }

        public static T GetRandomOrDefault<T>(this IList<T> list, T defaultValue = default)
            => list.IsNullOrEmpty() ? defaultValue : list[Random.Range(0, list.Count)];

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static void AddIfNotContains<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            if (list.Count > 0)
                list.RemoveAt(list.Count - 1);
        }
    }
}

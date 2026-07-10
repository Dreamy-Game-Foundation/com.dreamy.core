using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dreamy.Core
{
    public static class ListExtensions
    {
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

        public static bool TryGetAt<T>(this IReadOnlyList<T> list, int index, out T value)
        {
            if (list != null && index >= 0 && index < list.Count)
            {
                value = list[index];
                return true;
            }

            value = default;
            return false;
        }

        public static T GetLastOrDefault<T>(this IReadOnlyList<T> list, T defaultValue = default)
            => list == null || list.Count == 0 ? defaultValue : list[list.Count - 1];

        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (list == null || firstIndex == secondIndex)
            {
                return;
            }

            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null || match == null)
            {
                return 0;
            }

            var removedCount = 0;
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (!match(list[i]))
                {
                    continue;
                }

                list.RemoveAt(i);
                removedCount++;
            }

            return removedCount;
        }

        public static void AddRangeUnique<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null || items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                list.AddIfNotContains(item);
            }
        }
    }
}

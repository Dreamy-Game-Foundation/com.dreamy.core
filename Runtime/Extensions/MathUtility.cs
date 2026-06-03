using UnityEngine;

namespace Dreamy.Core
{
    public static class MathUtility
    {
        /// <summary>Remaps a value from one range to another.</summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
            => Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));

        /// <summary>Returns true with the given probability (0–1).</summary>
        public static bool Chance(float probability) => Random.value <= Mathf.Clamp01(probability);

        /// <summary>Weighted random selection. Returns index of chosen weight.</summary>
        public static int WeightedRandom(int[] weights)
        {
            int total = 0;
            foreach (var w in weights) total += w;

            int r = Random.Range(0, total);
            int cumulative = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (r < cumulative) return i;
            }
            return weights.Length - 1;
        }

        public static bool Approximately(float a, float b, float tolerance = 0.001f)
            => Mathf.Abs(a - b) < tolerance;
    }
}

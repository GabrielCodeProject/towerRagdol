using UnityEngine;

namespace RagdollRealms.Core
{
    public class SeededRandom
    {
        private System.Random _random;

        public int Seed { get; private set; }

        public SeededRandom(int seed)
        {
            Seed = seed;
            _random = new System.Random(seed);
        }

        public void Reset()
        {
            _random = new System.Random(Seed);
        }

        public void Reset(int newSeed)
        {
            Seed = newSeed;
            _random = new System.Random(newSeed);
        }

        public int Range(int min, int maxExclusive)
        {
            return _random.Next(min, maxExclusive);
        }

        public float Range(float min, float max)
        {
            return min + (float)_random.NextDouble() * (max - min);
        }

        public float Value => (float)_random.NextDouble();

        public Vector2 InsideUnitCircle()
        {
            float angle = Range(0f, Mathf.PI * 2f);
            float radius = Mathf.Sqrt(Value);
            return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        public int WeightedIndex(float[] weights)
        {
            float total = 0f;
            foreach (var w in weights) total += w;

            float roll = Range(0f, total);
            float cumulative = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative) return i;
            }

            return weights.Length - 1;
        }
    }
}

using System.Collections.Generic;
using NUnit.Framework;

namespace Dreamy.Core.Tests
{
    public sealed class CoreExtensionsTests
    {
        [Test]
        public void TryGetAt_WhenIndexIsValid_ReturnsTrueAndValue()
        {
            IReadOnlyList<int> values = new List<int> { 10, 20, 30 };

            var found = values.TryGetAt(1, out var value);

            Assert.That(found, Is.True);
            Assert.That(value, Is.EqualTo(20));
        }

        [Test]
        public void RemoveAll_RemovesMatchingItems()
        {
            var values = new List<int> { 1, 2, 3, 4, 5 };

            var removedCount = values.RemoveAll(value => value % 2 == 0);

            Assert.That(removedCount, Is.EqualTo(2));
            Assert.That(values, Is.EqualTo(new[] { 1, 3, 5 }));
        }

        [Test]
        public void ToPascalCase_RemovesSeparatorsAndCapitalizesWords()
        {
            var result = "coin_collect-reward".ToPascalCase();

            Assert.That(result, Is.EqualTo("CoinCollectReward"));
        }

        [Test]
        public void ToSnakeCase_SplitsPascalCaseWords()
        {
            var result = "CoinCollectReward".ToSnakeCase();

            Assert.That(result, Is.EqualTo("coin_collect_reward"));
        }

        [Test]
        public void IsNearlyZero_UsesTolerance()
        {
            Assert.That(0.00001f.IsNearlyZero(), Is.True);
            Assert.That(0.01f.IsNearlyZero(), Is.False);
        }
    }
}

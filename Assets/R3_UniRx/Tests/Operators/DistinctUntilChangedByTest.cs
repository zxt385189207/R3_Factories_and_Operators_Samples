using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class DistinctUntilChangedByTest
    {
        [Test]
        public void R3_DistinctUntilChangedBy_値を加工してその結果を用いて連続した重複を除外する()
        {
            var subject = new R3.Subject<int>();

            // 3で割った余り
            using var list = subject.DistinctUntilChangedBy(x => x % 3).ToLiveList();

            subject.OnNext(1); // 1
            subject.OnNext(2); // 2
            subject.OnNext(2); // 2
            subject.OnNext(3); // 0
            subject.OnNext(3); // 0
            subject.OnNext(4); // 1
            subject.OnNext(1); // 1
            subject.OnNext(1); // 1
            subject.OnNext(2); // 2


            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 2 }, list.ToArray());
        }

        [Test]
        public void UniRx_DistinctUntilChangedByに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
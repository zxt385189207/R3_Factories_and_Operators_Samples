using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class PairwiseTest
    {
        [Test]
        public void R3_Pairwise_直前の値とペアにする()
        {
            using var subject = new R3.Subject<int>();

            var list = subject.Pairwise().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            Assert.AreEqual(3, list.Count);
            CollectionAssert.AreEqual(new[]
            {
                (1, 2),
                (2, 3),
                (3, 4)
            }, list);
        }

        [Test]
        public void UniRx_Pairwise()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<Pair<int>>();
            subject.Pairwise().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            Assert.AreEqual(3, list.Count);
            CollectionAssert.AreEqual(new[]
            {
                new Pair<int>(1, 2),
                new Pair<int>(2, 3),
                new Pair<int>(3, 4)
            }, list);
        }
    }
}
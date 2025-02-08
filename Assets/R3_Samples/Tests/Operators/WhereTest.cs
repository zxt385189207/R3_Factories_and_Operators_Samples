using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class WhereTest
    {
        [Test]
        public void R3_Where_条件を満たす値だけを通す()
        {
            using var subject = new R3.Subject<int>();

            // 偶数のみを通す
            var result = subject.Where(x => x % 2 == 0).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 2, 4 }, result);
        }

        private record ThresholdHolder(int Threshold)
        {
            public int Threshold { get; } = Threshold;
        }

        [Test]
        public void R3_Where_条件を満たす値だけを通す_stateを指定する()
        {
            using var subject = new R3.Subject<int>();

            var thresholdHolder = new ThresholdHolder(3);

            // しきい値以上のみを通す
            // Stateを使うことでクロージャの生成を防止できる
            var result = subject
                .Where(
                    state: thresholdHolder,
                    predicate: (x, th) => x >= th.Threshold)
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 3, 4 }, result);
        }

        [Test]
        public void UniRx_Where()
        {
            using var subject = new UniRx.Subject<int>();

            var result = new List<int>();

            // 偶数のみを通す
            subject.Where(x => x % 2 == 0).Subscribe(result.Add);
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 2, 4 }, result);
        }
    }
}
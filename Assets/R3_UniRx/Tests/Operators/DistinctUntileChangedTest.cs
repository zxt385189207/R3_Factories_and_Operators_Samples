using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class DistinctUntilChangedTest
    {
        [Test]
        public void R3_DistinctUntilChanged_連続して重複したメッセージを除外する()
        {
            var subject = new R3.Subject<int>();

            var list = subject.DistinctUntilChanged().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnNext(3);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 1, 4, 3 }, list.ToArray());
        }

        [Test]
        public void UniRx_DistinctUntilChanged()
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            subject.DistinctUntilChanged().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnNext(3);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 1, 4, 3 }, list.ToArray());
        }
    }
}
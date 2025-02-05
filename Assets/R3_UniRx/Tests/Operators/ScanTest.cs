using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ScanTest
    {
        [Test]
        public void R3_Scan_OnNextが発行されるたびに過去の結果を用いて畳み込み計算を行い結果をOnNextとして発行する()
        {
            using var subject = new R3.Subject<int>();

            using var list = subject.Scan((acc, x) => acc + x).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnCompleted();

            // 1, 1+2, 1+2+3, 1+2+3+4, 1+2+3+4+5
            CollectionAssert.AreEqual(new[] { 1, 3, 6, 10, 15 }, list);
        }

        [Test]
        public void UniRx_Scan()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            subject.Scan((acc, x) => acc + x).Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnCompleted();

            // 1, 1+2, 1+2+3, 1+2+3+4, 1+2+3+4+5
            CollectionAssert.AreEqual(new[] { 1, 3, 6, 10, 15 }, list);
        }
    }
}
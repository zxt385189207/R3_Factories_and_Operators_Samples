using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class DistinctTest
    {
        [Test]
        public void R3_Distinct_過去に発行された値を含め重複を除外する()
        {
            var subject = new R3.Subject<int>();

            var list = subject.Distinct().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, list.ToArray());
        }
        
        [Test]
        public void UniRx_Distinct()
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<int>();
           subject.Distinct().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, list.ToArray());
        }
    }
}
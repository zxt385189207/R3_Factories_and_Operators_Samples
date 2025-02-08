using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class DistinctByTest
    {
        [Test]
        public void R3_DistinctBy_値を加工してその結果を用いて重複を除外する()
        {
            var subject = new R3.Subject<int>();

            // 3で割った余りで重複を除外する
            using var list = subject.DistinctBy(x => x % 3).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list.ToArray());
        }

        [Test]
        public void UniRx_Distinct()
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            
            // 3で割った余りで重複を除外する
            subject.Distinct(x => x % 3).Subscribe(list.Add);
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list.ToArray());
        }
    }
}
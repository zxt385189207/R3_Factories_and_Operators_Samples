using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class SkipWhileTest
    {
        [Test]
        public void R3_SkipWhile_条件を満たす間はOnNextを無視する()
        {
            using var subject = new R3.Subject<int>();

            var results = subject.SkipWhile(x => x < 3).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);　// ここから流れる
            subject.OnNext(4);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4,
                2,
                1
            }, results);
        }
        
        [Test]
        public void R3_SkipWhile_条件を満たす間はOnNextを無視するwithIndex()
        {
            using var subject = new R3.Subject<int>();

            // Indexを使ったSkipWhile
            // Indexが2以下の間は無視
            var results = subject.SkipWhile((_,index) => index <= 2).ToLiveList();

            subject.OnNext(1); // Index: 0
            subject.OnNext(2); // Index: 1
            subject.OnNext(3); // Index: 2
            subject.OnNext(4); // ここから流れる
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                4,
                2,
                1
            }, results);
        }
        
        [Test]
        public void UniRx_SkipWhile()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            subject.SkipWhile(x => x < 3).Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);　// ここから流れる
            subject.OnNext(4);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4,
                2,
                1
            }, list);
        }
    }
}
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class SelectTest
    {
        [Test]
        public void R3_Select_入力値を変換して出力する()
        {
            using var subject = new R3.Subject<int>();

            // 2倍する
            using var list = subject.Select(x => x * 2).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8, 10 }, list);
        }

        [Test]
        public void R3_Select_入力値を変換して出力しつつ同時にIndexを付与する()
        {
            using var subject = new R3.Subject<int>();

            // 2倍する
            using var list = subject.Select((x, index) => (x * 2, index)).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                (2, 0),
                (4, 1),
                (6, 2),
                (8, 3),
                (10, 4)
            }, list);
        }

        [Test]
        public void UniRx_Select()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<int>();

            // 2倍する
            subject.Select(x => x * 2).Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8, 10 }, list);
        }
    }
}
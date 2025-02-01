using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TakeWhileTest
    {
        [Test]
        public void R3_TakeWhile_条件を満たす間はOnNextを通過させる()
        {
            using var subject = new R3.Subject<int>();

            // 3未満の間だけ通過
            var results = subject.TakeWhile(x => x < 3).Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3); // 条件を満たさないのでここでOnCompleted
            subject.OnNext(4);

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);
        }

        [Test]
        public void UniRx_TakeWhile()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();

            // 3未満の間だけ通過
            subject.TakeWhile(x => x < 3).Materialize().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3); // 条件を満たさないのでここでOnCompleted
            subject.OnNext(4);

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(UniRx.NotificationKind.OnCompleted, list[2].Kind);
        }
    }
}
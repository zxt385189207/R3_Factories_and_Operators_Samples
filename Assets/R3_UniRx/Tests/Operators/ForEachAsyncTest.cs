using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ForEachAsyncTest
    {
        [Test]
        public void R3_ForEachAsync_購読処理自体をTask化する()
        {
            using var subject = new R3.Subject<int>();
            var list = new List<int>();

            // Subscribe()とほぼ同等に扱えるが、戻り値がTaskである
            var subscriptionTask = subject.ForEachAsync(x => list.Add(x));

            // まだ完了していない
            Assert.IsFalse(subscriptionTask.IsCompleted);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            // まだ完了していない
            Assert.IsFalse(subscriptionTask.IsCompleted);

            subject.OnCompleted();
            // 完了
            Assert.IsTrue(subscriptionTask.IsCompleted);
            Assert.AreEqual(new[] { 1, 2, 3 }, list);
        }

        [Test]
        public void UniRx_ForEachAsync_UniRxのForEachAsyncはR3と挙動が異なる()
        {
            using var subject = new UniRx.Subject<int>();
            var list = new List<int>();
            var isSubscriptionCompleted = false;

            // UniRxではIObservable<Unit>となり、
            // 元のObservableが完了したタイミングで1つだけOnNext(Unit)+OnCompleted()を発行する
            IObservable<UniRx.Unit> subscription = subject.ForEachAsync(x => list.Add(x));

            // 購読
            subscription.Subscribe(_ => { }, () => isSubscriptionCompleted = true);

            // まだ完了していない
            Assert.IsFalse(isSubscriptionCompleted);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            // まだ完了していない
            Assert.IsFalse(isSubscriptionCompleted);

            subject.OnCompleted();
            // 完了
            Assert.IsTrue(isSubscriptionCompleted);
            Assert.AreEqual(new[] { 1, 2, 3 }, list);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TimeoutTest
    {
        [Test]
        public async Task R3_Timeout_OnNextの間隔が一定以上開いたらタイムアウト()
        {
            using var subject = new R3.Subject<int>();

            // 100ms間OnNextが到達しなかったらOnCompleted(TimeoutException)を発行
            var results =
                subject.Timeout(TimeSpan.FromMilliseconds(100), TimeProvider.System)
                    .Materialize()
                    .ToLiveList();

            // 最初のOnNextはどれだけ遅延しても問題ない
            await UniTask.Delay(TimeSpan.FromMilliseconds(300));

            CollectionAssert.IsEmpty(results);

            subject.OnNext(1);
            subject.OnNext(2);

            // 200ms開いた
            await UniTask.Delay(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(3, results.Count);

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);

            // Timeout
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);
            Assert.IsInstanceOf<TimeoutException>(results[2].Error);
        }
        
        [Test]
        public async Task UniRx_Timeout()
        {
            using var subject = new UniRx.Subject<int>();

            var results = new List<UniRx.Notification<int>>();

            // 100ms間OnNextが到達しなかったらOnCompleted(TimeoutException)を発行
            subject.Timeout(TimeSpan.FromMilliseconds(100))
                .Materialize()
                .Subscribe(results.Add);

            // UniRxでは購読直後からタイマーが稼働するため、最初のOnNextも遅延したらタイムアウトする
            // await UniTask.Delay(TimeSpan.FromMilliseconds(200));

            CollectionAssert.IsEmpty(results);

            subject.OnNext(1);
            subject.OnNext(2);

            // 200ms開いた
            await UniTask.Delay(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(3, results.Count);

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);

            // Timeout
            Assert.AreEqual(UniRx.NotificationKind.OnError, results[2].Kind);
            Assert.IsInstanceOf<TimeoutException>(results[2].Exception);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class TimeoutFrameTest
    {
        [Test]
        public void R3_TimeoutFrame_OnNextの間隔が一定フレーム以上開いたらタイムアウト()
        {
            var fakeFrameProvider = new FakeFrameProvider();
            using var subject = new R3.Subject<int>();

            //　3F間OnNextが到達しなかったらOnCompleted(TimeoutException)を発行
            var results =
                subject.TimeoutFrame(3, fakeFrameProvider)
                    .Materialize()
                    .ToLiveList();
            
            CollectionAssert.IsEmpty(results);

            subject.OnNext(1);
            subject.OnNext(2);

            // 3F以上あいた
            fakeFrameProvider.Advance(5); 

            Assert.AreEqual(3, results.Count);

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);

            // Timeout
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);
            Assert.IsInstanceOf<TimeoutException>(results[2].Error);
        }
        
        [Test]
        public async Task UniRx_TimeoutFrame()
        {
            using var subject = new UniRx.Subject<int>();

            var results = new List<UniRx.Notification<int>>();

            // 100ms間OnNextが到達しなかったらOnCompleted(TimeoutException)を発行
            subject.TimeoutFrame(3)
                .Materialize()
                .Subscribe(results.Add);

            // UniRxでは購読直後からタイマーが稼働するため、最初のOnNextも遅延したらタイムアウトする
            // await UniTask.DelayFrame(5);

            CollectionAssert.IsEmpty(results);

            subject.OnNext(1);
            subject.OnNext(2);

            await UniTask.DelayFrame(5);

            Assert.AreEqual(3, results.Count);

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);

            // TimeoutFrame
            Assert.AreEqual(UniRx.NotificationKind.OnError, results[2].Kind);
            Assert.IsInstanceOf<TimeoutException>(results[2].Exception);
        }
    }
}
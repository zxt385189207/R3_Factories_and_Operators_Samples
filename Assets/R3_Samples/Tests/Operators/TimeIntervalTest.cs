using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class TimeIntervalTest
    {
        [Test]
        public async Task R3_TimeInterval_OnNextが到達したら直前のOnNextからの経過時間を発行する()
        {
            using var subject = new R3.Subject<int>();

            var results = subject.TimeInterval(TimeProvider.System).ToLiveList();

            subject.OnNext(1); // 0ms
            subject.OnNext(2); // 0ms
            subject.OnNext(3); // 0ms

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.Realtime);

            subject.OnNext(4); // 100ms

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.Realtime);

            subject.OnNext(5); // 100ms

            Assert.AreEqual(1, results[0].Value);
            Assert.LessOrEqual((results[0].Interval - TimeSpan.Zero).TotalMilliseconds, 5f); // だいたい0ms

            Assert.AreEqual(2, results[1].Value);
            Assert.LessOrEqual((results[1].Interval - TimeSpan.Zero).TotalMilliseconds, 5f); // だいたい0ms

            Assert.AreEqual(3, results[2].Value); 
            Assert.LessOrEqual((results[2].Interval - TimeSpan.Zero).TotalMilliseconds, 5f); // だいたい0ms

            Assert.AreEqual(4, results[3].Value);
            Assert.LessOrEqual((results[3].Interval - TimeSpan.FromMilliseconds(100)).TotalMilliseconds, 10f); // だいたい100ms

            Assert.AreEqual(5, results[4].Value);
            Assert.LessOrEqual((results[4].Interval - TimeSpan.FromMilliseconds(100)).TotalMilliseconds, 10f); // だいたい100ms
        }

        [Test]
        public async Task UniRx_TimeInterval()
        {
            using var subject = new UniRx.Subject<int>();

            // Schedulerを指定した時間計測版
            var results = new List<TimeInterval<int>>();
            subject.TimeInterval(Scheduler.ThreadPool).Subscribe(results.Add);

            subject.OnNext(1); // 0ms
            subject.OnNext(2); // 0ms
            subject.OnNext(3); // 0ms

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.Realtime);

            subject.OnNext(4); // 100ms

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.Realtime);

            subject.OnNext(5); // 100ms

            Assert.AreEqual(1, results[0].Value);
            Assert.LessOrEqual((results[0].Interval - TimeSpan.Zero).TotalMilliseconds, 5f); // だいたい0ms

            Assert.AreEqual(2, results[1].Value);
            Assert.LessOrEqual((results[1].Interval - TimeSpan.Zero).TotalMilliseconds, 5f); // だいたい0ms

            Assert.AreEqual(3, results[2].Value);
            Assert.LessOrEqual((results[2].Interval - TimeSpan.Zero).TotalMilliseconds, 5f); // だいたい0ms

            Assert.AreEqual(4, results[3].Value);
            Assert.LessOrEqual((results[3].Interval - TimeSpan.FromMilliseconds(100)).TotalMilliseconds, 10f); // だいたい100ms

            Assert.AreEqual(5, results[4].Value);
            Assert.LessOrEqual((results[4].Interval - TimeSpan.FromMilliseconds(100)).TotalMilliseconds, 10f); // だいたい100ms
        }

        [Test]
        public async Task UniRx_FrameTimeInterval()
        {
            // Unityのフレームを使った時間計測版

            // Editorテストでは正しく計測できなかったのでIgnore
            Assert.Ignore();
            return;

            using var subject = new UniRx.Subject<int>();

            var results = new List<TimeInterval<int>>();

            // Unityのフレームを使った時間計測版
            subject.FrameTimeInterval().Subscribe(results.Add);

            subject.OnNext(1); // 0ms
            subject.OnNext(2); // 0ms
            subject.OnNext(3); // 0ms

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.Realtime);

            subject.OnNext(4); // 100ms

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.Realtime);

            subject.OnNext(5); // 100ms

            Assert.AreEqual(1, results[0].Value);
            Assert.LessOrEqual((results[0].Interval - TimeSpan.Zero).TotalMilliseconds, 20f); // だいたい0ms

            Assert.AreEqual(2, results[1].Value);
            Assert.LessOrEqual((results[1].Interval - TimeSpan.Zero).TotalMilliseconds, 20f); // だいたい0ms
            
            Assert.AreEqual(3, results[2].Value);
            Assert.LessOrEqual((results[2].Interval - TimeSpan.Zero).TotalMilliseconds, 20f); // だいたい0ms

            Assert.AreEqual(4, results[3].Value);
            Assert.LessOrEqual((results[3].Interval - TimeSpan.FromMilliseconds(100)).TotalMilliseconds, 20f); // だいたい100ms

            Assert.AreEqual(5, results[4].Value);
            Assert.LessOrEqual((results[4].Interval - TimeSpan.FromMilliseconds(100)).TotalMilliseconds, 20f); // だいたい100ms
        }
    }
}
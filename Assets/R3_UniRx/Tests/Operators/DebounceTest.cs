using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public class DebounceTest
    {
        [Test]
        public async Task R3_Debounce_メッセージの流量を減らす()
        {
            var subject = new R3.Subject<int>();

            // 連続して値が発行された場合は値が落ち着いてから100ms後に最後の値を発行する
            // -> OnNextがまとめて発行されたときに、最後に値が発行されてから一定時間経過後に最後のOnNextを1つだけ発行する。
            var list = subject.Debounce(TimeSpan.FromMilliseconds(100), UnityTimeProvider.Update).ToLiveList();

            // 本来はFakeTimeProviderを使ってテストしたいがUnityでうまく動作しないため、実際の時間を使う
            subject.OnNext(1);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(4);
            subject.OnNext(5); // ここから100ms経過すると5が発行される
            await UniTask.Delay(TimeSpan.FromMilliseconds(150), DelayType.Realtime);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 5 }, list);
        }

        [Test]
        public async Task UniRx_Throttle()
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            subject
                .Throttle(TimeSpan.FromMilliseconds(100), Scheduler.MainThread)
                .Subscribe(list.Add);

            subject.OnNext(1);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(4);
            subject.OnNext(5); // ここから100ms経過すると5が発行される
            await UniTask.Delay(TimeSpan.FromMilliseconds(150), DelayType.Realtime);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 5 }, list);
        }
    }
}
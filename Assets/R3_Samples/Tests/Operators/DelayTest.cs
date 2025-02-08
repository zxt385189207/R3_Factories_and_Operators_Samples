using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class DelayTest
    {
        [Test]
        public async Task R3_Delay_各メッセージ発行を指定した時間分だけ遅らせる()
        {
            using var subject = new R3.Subject<int>();

            // 100ms遅らせる
            using var list = subject.Delay(TimeSpan.FromMilliseconds(100), TimeProvider.System).Materialize().ToLiveList();

            subject.OnNext(1);
            CollectionAssert.IsEmpty(list); // まだ発行されていない

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(R3.NotificationKind.OnNext, list[0].Kind); // 発行されている
            Assert.AreEqual(1, list[0].Value);

            // --

            subject.OnErrorResume(new Exception());
            Assert.AreEqual(1, list.Count); // まだ発行されていない

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(2, list.Count); // 発行されている
            Assert.AreEqual(R3.NotificationKind.OnErrorResume, list[1].Kind);

            // -

            subject.OnCompleted();
            Assert.AreEqual(2, list.Count); // まだ発行されていない

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(3, list.Count); // 発行されている
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[2].Kind);
        }

        [Test]
        public async Task UniRx_Delay()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();

            // 100ms遅らせる
            subject
                .Delay(TimeSpan.FromMilliseconds(100), Scheduler.ThreadPool)
                .Materialize()
                .Subscribe(list.Add);

            subject.OnNext(1);
            CollectionAssert.IsEmpty(list); // まだ発行されていない

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(UniRx.NotificationKind.OnNext, list[0].Kind); // 発行されている
            Assert.AreEqual(1, list[0].Value);

            // --

            // UniRxではOnErrorが発行されるとDelayは無視される点がR3とは異なる
            // OnCompletedは遅延する
            subject.OnError(new Exception());
            Assert.AreEqual(2, list.Count);
        }
    }
}
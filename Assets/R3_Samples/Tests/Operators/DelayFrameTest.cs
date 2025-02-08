using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class DelayFrameTest
    {
        [Test]
        public void R3_DelayFrame_各メッセージ発行を指定したフレーム分だけ遅らせる()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 1F遅らせる
            using var list = subject.DelayFrame(1, fakeFrameProvider).Materialize().ToLiveList();

            subject.OnNext(1);
            CollectionAssert.IsEmpty(list); // まだ発行されていない

            fakeFrameProvider.Advance();

            Assert.AreEqual(R3.NotificationKind.OnNext, list[0].Kind); // 発行されている
            Assert.AreEqual(1, list[0].Value);

            // --

            subject.OnErrorResume(new Exception());
            Assert.AreEqual(1, list.Count); // まだ発行されていない

            fakeFrameProvider.Advance();

            Assert.AreEqual(2, list.Count); // 発行されている
            Assert.AreEqual(R3.NotificationKind.OnErrorResume, list[1].Kind);

            // -

            subject.OnCompleted();
            Assert.AreEqual(2, list.Count); // まだ発行されていない

            fakeFrameProvider.Advance();

            Assert.AreEqual(3, list.Count); // 発行されている
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[2].Kind);
        }

        [Test]
        public async Task UniRx_DelayFrame()
        {
            // フレーム依存のテストは不安定すぎるためIgnore
            Assert.Ignore();
            return;
            
            using var subject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();

            // 1F遅らせる
            subject
                .DelayFrame(1)
                .Materialize()
                .Subscribe(list.Add);
            
            subject.OnNext(1);
            CollectionAssert.IsEmpty(list); // まだ発行されていない

            // FrameCountが変わるまで待つ
            await UniTask.DelayFrame(1);

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
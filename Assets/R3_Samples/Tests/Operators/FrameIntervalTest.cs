using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class FrameIntervalTest
    {
        [Test]
        public void R3_FrameInterval_直前のOnNextから何フレーム経過したかをセットにする()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            // 3フレームを進めておく
            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();

            using var subject = new R3.Subject<string>();

            using var list = subject.FrameInterval(fakeFrameProvider).ToLiveList();

            subject.OnNext("A");
            fakeFrameProvider.Advance();

            subject.OnNext("B");
            subject.OnNext("C");

            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();

            subject.OnNext("D");
            subject.OnCompleted();

            Assert.AreEqual(new (long, string)[]
            {
                (0, "A"),
                (1, "B"),
                (0, "C"),
                (2, "D"),
            }, list);
        }

        [Test]
        public async Task UniRx_FrameInterval()
        {
            // フレーム依存のテストは不安定すぎるためIgnore
            Assert.Ignore();
            return;
            
            // UniRxのFrameIntervalはUnityEngine.Time.frameCountを使っている

            using var subject = new UniRx.Subject<string>();
            var list = new List<FrameInterval<string>>();

            subject.FrameInterval().Subscribe(x => list.Add(x));

            subject.OnNext("A");

            // FrameCountが変わるまで待つ
            await UniTask.DelayFrame(1);
            await UniTask.DelayFrame(1);

            subject.OnNext("B");

            await UniTask.DelayFrame(1);

            subject.OnNext("C");
            subject.OnNext("D");

            subject.OnCompleted();

            Assert.AreEqual(new[]
            {
                new FrameInterval<string>("A", 0),
                new FrameInterval<string>("B", 2),
                new FrameInterval<string>("C", 1),
                new FrameInterval<string>("D", 0),
            }, list);
        }
    }
}
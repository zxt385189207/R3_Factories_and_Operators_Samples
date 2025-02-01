using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ThrottleLastFrameTest
    {
        [Test]
        public void R3_ThrottleLastFrame_OnNextが到達したら一定フレーム遮断したのちに最後を発行()
        {
            var fakeFrameProvider = new FakeFrameProvider();
            
            using var subject = new R3.Subject<int>();

            // 一度OnNextが到達したら次以降のOnNextを3F間無視して、最後に発行されたOnNextを発行する
            var results = subject.ThrottleLastFrame(3,fakeFrameProvider).ToLiveList();

            subject.OnNext(1); // 遮断開始

            fakeFrameProvider.Advance();

            subject.OnNext(2); // まだ遮断中

            fakeFrameProvider.Advance();

            subject.OnNext(3); // まだ遮断中、だが最後に発行されたOnNextなので後で通過

            fakeFrameProvider.Advance(); // 解除

            subject.OnNext(4); // 次の遮断開始

            CollectionAssert.AreEqual(new[]
            {
                3
            }, results);
        }


        [Test]
        public async Task UniRx_SampleFrame()
        {
            // UniRxではSampleFrameが同じ意味を持つOperator
            
            // Editorテストではフレームカウントが不安定なためテストが全く安定しないのでIgnore
            // PlayModeTestなら問題ないはず…
            Assert.Ignore();
            return;

            using var subject = new UniRx.Subject<int>();

            var results = new List<int>();
            
            subject.SampleFrame(3).Subscribe(results.Add);

            subject.OnNext(1); // 遮断開始

            await UniTask.DelayFrame(1);

            subject.OnNext(2); // まだ遮断中

            await UniTask.DelayFrame(1);

            subject.OnNext(3); // まだ遮断中、だが最後に発行されたOnNextなので後で通過

            await UniTask.DelayFrame(1); // 解除

            subject.OnNext(4); // 遮断開始

            CollectionAssert.AreEqual(new[]
            {
                3
            }, results);
        }
    }
}
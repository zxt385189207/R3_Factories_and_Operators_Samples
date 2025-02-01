using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ThrottleFirstFrameTest
    {
        [Test]
        public void R3_ThrottleFirstFrame_OnNextが到達したら次以降のOnNextを一定フレーム無視する()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 一度OnNextが到達したら次以降のOnNextを3F間無視する
            var results = subject.ThrottleFirstFrame(3, fakeFrameProvider).ToLiveList();

            subject.OnNext(1); // pass 1F

            fakeFrameProvider.Advance();

            subject.OnNext(2); // 2F ignore

            fakeFrameProvider.Advance();

            subject.OnNext(3); // 3F ignore

            fakeFrameProvider.Advance(); // reset

            subject.OnNext(4); // 1F pass

            fakeFrameProvider.Advance();

            subject.OnNext(5); // 2F ignore

            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, results);
        }


        [Test]
        public async Task UniRx_ThrottleFirstFrame()
        {
            // Editorテストではフレームカウントが不安定なためテストが全く安定しないのでIgnore
            // PlayModeTestなら問題ないはず…
            Assert.Ignore();
            return;
            
            using var subject = new UniRx.Subject<int>();
            
            var results = new List<int>();
            
            // 一度OnNextが到達したら次以降のOnNextを3F間無視する
            subject.ThrottleFirstFrame(3, FrameCountType.EndOfFrame).Subscribe(results.Add);
            
            subject.OnNext(1); // pass
            
            await UniTask.Yield();
                
            subject.OnNext(2); // ignore
            
            await UniTask.Yield();
            
            subject.OnNext(3); // ignore
            
            await UniTask.Yield();
            
            subject.OnNext(4); // ignore
            
            await UniTask.Yield();
            
            subject.OnNext(5); // pass
            
            CollectionAssert.AreEqual(new[]
            {
                1,
                5
            }, results);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class DebounceFrameTest
    {
        [Test]
        public void R3_DebounceFrame_OnNextが落ち着くのを待って最後の１つを発行する()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 連続して値が発行された場合は値が落ち着いてから5F後に最後の値を発行する
            using var list = subject.DebounceFrame(5, fakeFrameProvider).ToLiveList();

            // 1F目
            subject.OnNext(1);
            fakeFrameProvider.Advance();
            // 2F目
            subject.OnNext(2);
            subject.OnNext(3);
            fakeFrameProvider.Advance();
            // 3F目
            subject.OnNext(4);

            fakeFrameProvider.Advance(); // 1
            fakeFrameProvider.Advance(); // 2
            fakeFrameProvider.Advance(); // 3
            fakeFrameProvider.Advance(); // 4
            CollectionAssert.IsEmpty(list.ToArray()); // まだ値は発行されていない

            fakeFrameProvider.Advance(); // 5
            CollectionAssert.AreEqual(new[] { 4 }, list.ToArray()); // 5F後に最後の値が発行されている
        }


        [Test]
        public async Task UniRx_ThrottleFrame()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            subject
                .ThrottleFrame(5)
                .Subscribe(list.Add);

            subject.OnNext(1);
            await UniTask.Yield();
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.Yield();
            subject.OnNext(4);
            await UniTask.Yield(); // 0F目
            
            await UniTask.Yield(); // 1
            await UniTask.Yield(); // 2
            await UniTask.Yield(); // 3
            await UniTask.Yield(); // 4
            CollectionAssert.IsEmpty(list.ToArray()); // まだ値は発行されていない

            await UniTask.Yield(); // 5
            CollectionAssert.AreEqual(new[] { 4 }, list.ToArray()); // 5F後に最後の値が発行されている
        }
    }
}
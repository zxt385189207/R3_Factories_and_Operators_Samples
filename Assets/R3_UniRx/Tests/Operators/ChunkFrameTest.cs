using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using UnityEngine.TestTools;

namespace R3_UniRx.Tests.Operators
{
    public class ChunkFrameTest
    {
        [Test]
        public void R3_ChunkFrame_指定したフレーム区間内に発行されたOnNextをまとめて1つのOnNextして発行する()
        {
            // 時間管理をFakeFrameProviderで行う
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 1フレームごとにまとめる
            var list = subject.ChunkFrame(1, fakeFrameProvider).ToLiveList();

            // 1F目
            subject.OnNext(1);
            fakeFrameProvider.Advance();

            // 2F目
            subject.OnNext(2);
            subject.OnNext(3);
            fakeFrameProvider.Advance();

            // 3F目
            subject.OnNext(4);
            fakeFrameProvider.Advance();

            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1 },
                new[] { 2, 3 },
                new[] { 4 }
            }, list.ToArray());
        }

        [Test]
        public void R3_ChunkFrame_指定したフレーム区間内に発行されたOnNextを上限を指定してまとめる()
        {
            // 時間管理をFakeFrameProviderで行う
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 1フレームごとにまとめるが、3個溜まったら一度出力する
            var list = subject.ChunkFrame(1, 3, fakeFrameProvider).ToLiveList();

            // 1F目
            subject.OnNext(1);
            fakeFrameProvider.Advance();

            // 2F目
            subject.OnNext(2);
            subject.OnNext(3);
            fakeFrameProvider.Advance();

            // 3F目
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            fakeFrameProvider.Advance();

            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1 },
                new[] { 2, 3 },
                new[] { 4, 5, 6 },
                new[] { 7 }
            }, list.ToArray());
        }


        [UnityTest]
        public IEnumerator UniRx_BatchFrame() => UniTask.ToCoroutine(async () =>
        {
            // UniRxではBatchFrame
            using var subject = new UniRx.Subject<int>();

            var list = new List<IList<int>>();
            subject
                .BatchFrame(1, FrameCountType.Update)
                .Subscribe(x => list.Add(x));

            // 1F目
            subject.OnNext(1);
            await UniTask.NextFrame();
            // 2F目
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.NextFrame();
            // 3F目
            subject.OnNext(4);
            subject.OnCompleted();
            await UniTask.NextFrame();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1 },
                new[] { 2, 3 },
                new[] { 4 }
            }, list.ToArray());
        });
    }
}
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using Assert = UnityEngine.Assertions.Assert;
using Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ObserveOnTest
    {
        [Test]
        public void R3_ObserveOn_実行コンテキストを切り替える()
        {
            // 仮にUpdateとFixedUpdateのFrameProviderがあったとする（Fakeだけど）
            var updateFrameProvider = new FakeFrameProvider();
            var fixedUpdateFrameProvider = new FakeFrameProvider();

            // Update間隔で値を発行されたイベントを、FixedUpdateタイミングに変更する、みたいな
            using var list = Observable.IntervalFrame(1, updateFrameProvider)
                .Index() // 連番を振る
                .ObserveOn(fixedUpdateFrameProvider)
                .ToLiveList();

            // まだ値が流れていない
            CollectionAssert.IsEmpty(list);

            // Updateタイミングが進んでObservable.IntervalFrame(1, updateFrameProvider)が値を発行
            updateFrameProvider.Advance();

            // まだ値が流れていない
            CollectionAssert.IsEmpty(list);

            updateFrameProvider.Advance();
            updateFrameProvider.Advance(); // 3フレーム進んだ

            // まだ値が流れていない
            CollectionAssert.IsEmpty(list);

            // やっとFixedUpdateが1フレーム進んだ
            fixedUpdateFrameProvider.Advance();

            // FixedUpdateが進んだことでObserveOnから値が流れている
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, list);
        }

        [Test]
        public async Task UniRx_ObserveOn()
        {
            // Unityメインスレッドで発行した値を、
            // スレッドプールに移動したあとにSelectを実行し、
            // またメインスレッドに戻ってくる
            var result = await UniRx.Observable.Return(1, Scheduler.MainThread)
                .ObserveOn(Scheduler.ThreadPool)
                .Select(_ => Thread.CurrentThread.ManagedThreadId)
                .ObserveOn(Scheduler.MainThread)
                .ToTask();

            // 今ここのスレッド（たぶんメインスレッド）とは違うスレッドでSelectが実行されている
            Assert.AreNotEqual(Thread.CurrentThread.ManagedThreadId, result);
        }
    }
}
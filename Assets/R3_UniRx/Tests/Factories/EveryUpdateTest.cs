using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class EveryUpdateTest
    {
        [Test]
        public void EveryUpdate_毎フレームUnitを発行する()
        {
            using var cts = new CancellationTokenSource();
            var fakeFrameProvider = new FakeFrameProvider(); // テスト用のフレームカウントを提供するクラス

            // FakeFrameProviderを使って毎フレームUnitを発行するObservableを作成する
            var observable = Observable.EveryUpdate(fakeFrameProvider, cts.Token);

            using var list = observable.Materialize().ToLiveList();

            // まだフレームが進んでいないのでOnNextもOnCompletedも発行されない
            CollectionAssert.IsEmpty(list);

            // フレームを進める
            fakeFrameProvider.Advance();

            // 1フレーム進んだのでOnNextが発行される
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(Unit.Default, list[0].Value);

            // さらにフレームを進める
            fakeFrameProvider.Advance();

            // 1フレーム進んだのでOnNextが発行される
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[1].Kind);
            Assert.AreEqual(Unit.Default, list[1].Value);

            // CancellationTokenがキャンセルされるとOnCompletedが発行される
            cts.Cancel();
            
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[2].Kind);
        }
    }
}
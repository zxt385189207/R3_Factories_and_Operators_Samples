using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class TimerFrameTest
    {
        [Test]
        public void TimerFrame_指定したフレーム後に1回発行する()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // 3F後にOnNextを発行し、その後OnCompletedを発行する
            using var list = Observable
                .TimerFrame(3, fakeFrameProvider, ct)
                .Materialize()
                .ToLiveList();

            CollectionAssert.IsEmpty(list);

            fakeFrameProvider.Advance(3);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }

        [Test]
        public void TimerFrame_指定したフレーム待ったあとに指定フレーム間隔で発行を繰り返す()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // 3F後にOnNextを発行し、その後は1FごとにOnNextを発行する
            using var list = Observable
                .TimerFrame(3, 1, fakeFrameProvider)
                .Take(3) // 無限に続くので3回だけ取得して打ち止める
                .Materialize()
                .ToLiveList();

            CollectionAssert.IsEmpty(list);

            fakeFrameProvider.Advance(3);

            Assert.AreEqual(1, list.Count);

            fakeFrameProvider.Advance(1);

            Assert.AreEqual(2, list.Count);

            fakeFrameProvider.Advance(1);

            Assert.AreEqual(4, list.Count);

            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(NotificationKind.OnNext, list[1].Kind);
            Assert.AreEqual(NotificationKind.OnNext, list[2].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
    }
}
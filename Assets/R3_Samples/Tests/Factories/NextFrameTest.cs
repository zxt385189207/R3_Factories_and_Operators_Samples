using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class NextFrameTest
    {
        [Test]
        public void NextFrame_次のフレームでOnNextとOnCompletedを発行する()
        {
            var fakeFrameProvider = new FakeFrameProvider();
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            using var list = Observable.NextFrame(fakeFrameProvider, token).Materialize().ToLiveList();

            Assert.IsEmpty(list);

            // ややこしいが、この時点ではまだ「0フレーム目」
            fakeFrameProvider.Advance();
            // 2回Advance、つまり確実に「次のフレームに進んだ時」に発火する
            fakeFrameProvider.Advance();
          
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            
            // CancellationTokenが発火するとOnCompletedが発行される
            // 今回は既に完了済みなので意味はない
            cts.Cancel();
        }
    }
}
using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class NextFrameTest
    {
        [Test]
        public void NextFrame_次のフレームでOnNextとOnCompletedを発行する()
        {
            var fakeFrameProvider = new FakeFrameProvider();
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            using var list = Observable.NextFrame(fakeFrameProvider, token).ToLiveList();

            Assert.IsEmpty(list);

            fakeFrameProvider.Advance();
            
            Assert.AreEqual(new[]
            {
                NotificationKind.OnNext,
                NotificationKind.OnCompleted,
            }, list);
            
            // CancellationTokenが発火するとOnCompletedが発行される
            // 今回は既に完了済みなので意味はない
            cts.Cancel();
        }
    }
}
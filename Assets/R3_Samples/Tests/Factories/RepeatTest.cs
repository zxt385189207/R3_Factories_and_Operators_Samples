using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class RepeatTest
    {
        [Test]
        public void Repeat_同じ値を繰り返し発行する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            using var list = Observable.Repeat("test", 3, ct).Materialize().ToLiveList();

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("test", list[0].Value);
            Assert.AreEqual("test", list[1].Value);
            Assert.AreEqual("test", list[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);

            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
    }
}
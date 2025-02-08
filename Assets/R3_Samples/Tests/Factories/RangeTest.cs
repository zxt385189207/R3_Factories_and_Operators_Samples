using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class RangeTest
    {
        [Test]
        public void Range_連番を発行する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            using var list = Observable.Range(0, 3, ct).Materialize().ToLiveList();

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(0, list[0].Value);
            Assert.AreEqual(1, list[1].Value);
            Assert.AreEqual(2, list[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
    }
}
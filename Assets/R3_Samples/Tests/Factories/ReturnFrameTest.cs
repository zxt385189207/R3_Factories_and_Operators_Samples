using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class ReturnFrameTest
    {
        [Test]
        public void ReturnFrame_値を1つだけ発行する_フレーム指定()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // 3フレーム後にOnNextを発行し、その後OnCompletedを発行する
            using var list = Observable.ReturnFrame("test", 3, fakeFrameProvider, ct).Materialize().ToLiveList();

            CollectionAssert.IsEmpty(list);

            fakeFrameProvider.Advance(3);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("test", list[0].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);

            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
    }
}
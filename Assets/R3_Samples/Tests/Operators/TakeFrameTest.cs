using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class TakeFrameTest
    {
        [Test]
        public void R3_TakeFrame_指定したフレーム間だけOnNextを通過させる()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 3Fだけ通過
            using var list = subject.TakeFrame(3, fakeFrameProvider).Materialize().ToLiveList();

            subject.OnNext(1);
            fakeFrameProvider.Advance();

            subject.OnNext(2);
            fakeFrameProvider.Advance();

            subject.OnNext(3);
            fakeFrameProvider.Advance();

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(3, list[2].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[3].Kind);
        }

        [Test]
        public void UniRx_TakeFrameは存在しない()
        {
            Assert.Ignore();
        }
    }
}
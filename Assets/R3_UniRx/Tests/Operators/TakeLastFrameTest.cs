using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TakeLastFrameTest
    {
        [Test]
        public void R3_TakeLastFrame_OnCompleted発行時に最後から指定したフレーム間だけOnNextを通過させる()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 最後2Fだけ通過
            var list = subject.TakeLastFrame(2, fakeFrameProvider).ToLiveList();

            subject.OnNext(1);
            fakeFrameProvider.Advance();

            subject.OnNext(2); // -2F ここまで対象
            fakeFrameProvider.Advance();

            subject.OnNext(3); // -1F
            fakeFrameProvider.Advance();

            subject.OnNext(4); // -0F

            // まだOnCompletedしていないので通過しない
            CollectionAssert.IsEmpty(list);

            subject.OnCompleted();

            // 最後2Fだけ発行されている
            CollectionAssert.AreEqual(new[] { 2, 3, 4 }, list);
        }

        [Test]
        public void UniRx_TakeLastFrameは存在しない()
        {
            Assert.Ignore();
        }
    }
}
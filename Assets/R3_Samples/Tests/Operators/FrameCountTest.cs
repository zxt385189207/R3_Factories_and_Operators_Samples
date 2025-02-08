using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class FrameCountTest
    {
        [Test]
        public void R3_FrameCount_何フレーム目に発行されたかとセットにする()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            // 3フレームを進めておく
            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();

            using var subject = new R3.Subject<string>();

            using var list = subject.FrameCount(fakeFrameProvider).ToLiveList();

            subject.OnNext("A");
            fakeFrameProvider.Advance();

            subject.OnNext("B");
            subject.OnNext("C");

            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();

            subject.OnNext("D");
            subject.OnCompleted();

            Assert.AreEqual(new (long, string)[]
            {
                (3, "A"),
                (4, "B"),
                (4, "C"),
                (6, "D"),
            }, list);
        }
        
        [Test]
        public void UniRx_FrameCountに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
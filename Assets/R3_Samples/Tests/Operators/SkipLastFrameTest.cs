using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class SkipLastFrameTest
    {
        [Test]
        public void R3_SkipLastFrame_完了から指定したフレーム間のOnNextを無視する()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 完了から2F前までのOnNextを無視
            using var list = subject.SkipLastFrame(2, fakeFrameProvider).ToLiveList();
            
            subject.OnNext(1); 
            fakeFrameProvider.Advance();
            
            subject.OnNext(2); 
            fakeFrameProvider.Advance();
            
            subject.OnNext(3); // ここはOK
            fakeFrameProvider.Advance();
            
            subject.OnNext(4); // -2F目、無視
            fakeFrameProvider.Advance();
            
            subject.OnNext(5);  // -1F目、無視
            subject.OnCompleted();
            
            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                3
            }, list);
        }

        [Test]
        public void UniRx_SkipLastFrameに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class ToLiveListTest
    {
        [Test]
        public void R3_ToLiveList_LiveListに変換する()
        {
            using var subject = new R3.Subject<int>();

            // LiveListは発行されたOnNextをリアルタイムに反映する
            var liveList = subject.ToLiveList();

            subject.OnNext(1);

            CollectionAssert.AreEqual(new[] { 1 }, liveList);

            subject.OnNext(2);

            CollectionAssert.AreEqual(new[] { 1, 2 }, liveList);

            subject.OnNext(3);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);
        }

        [Test]
        public void UniRx_LiveListは存在しない()
        {
            Assert.Ignore();
        }
    }
}
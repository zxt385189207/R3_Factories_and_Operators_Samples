using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public class AppendTest
    {
        [Test]
        public void R3_Append_OnCompleted発行時にObservableの最後に指定された値を挿入する()
        {
            var subject = new R3.Subject<int>();

            // Appendで最後に100を追加する
            var liveList = subject.Append(100).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // Observableはまだ完了していないので100は追加されていない
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);

            // Observableが完了してAppendが発火する
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100 }, liveList);
        }
        
        [Test]
        public void UniRx_Appendは存在しない()
        {
            Assert.Ignore();
        }
    }
}
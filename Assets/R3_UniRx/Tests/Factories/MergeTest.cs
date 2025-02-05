using NUnit.Framework;
using R3;
using Assert = UnityEngine.Assertions.Assert;

namespace R3_UniRx.Tests.Factories
{
    public sealed class MergeTest
    {
        [Test]
        public void Merge_複数のObservableを並列に合成する()
        {
            var firstSubject = new Subject<int>();
            var secondSubject = new Subject<int>();

            using var list = Observable.Merge(firstSubject, secondSubject).ToLiveList();

            // ごちゃまぜに発行
            firstSubject.OnNext(1);
            secondSubject.OnNext(2);
            firstSubject.OnNext(3);
            secondSubject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, list);

            // 片方だけ完了
            firstSubject.OnCompleted();
            // まだ完了していない
            Assert.IsFalse(list.IsCompleted);

            // もう片方も完了
            secondSubject.OnNext(5);
            secondSubject.OnCompleted();

            // 両方完了したら完了する
            Assert.IsTrue(list.IsCompleted);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, list);
        }
    }
}
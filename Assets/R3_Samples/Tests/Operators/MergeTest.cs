using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;
using Assert = UnityEngine.Assertions.Assert;

namespace R3_Samples.Tests.Operators
{
    public sealed class MergeTest
    {
        [Test]
        public void R3_Merge_複数のObservableを並列に合成する()
        {
            var firstSubject = new R3.Subject<int>();
            var secondSubject = new R3.Subject<int>();

            using var list = firstSubject.Merge(secondSubject).ToLiveList();

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

        [Test]
        public void UniRx_Merge()
        {
            var firstSubject = new UniRx.Subject<int>();
            var secondSubject = new UniRx.Subject<int>();

            var list = new List<int>();
            var isCompleted = false;

            firstSubject.Merge(secondSubject).Subscribe(list.Add, () => isCompleted = true);

            // ごちゃまぜに発行
            firstSubject.OnNext(1);
            secondSubject.OnNext(2);
            firstSubject.OnNext(3);
            secondSubject.OnNext(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, list);

            // 片方だけ完了
            firstSubject.OnCompleted();
            // まだ完了していない
            Assert.IsFalse(isCompleted);

            // もう片方も完了
            secondSubject.OnNext(5);
            secondSubject.OnCompleted();

            // 両方完了したら完了する
            Assert.IsTrue(isCompleted);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, list);
        }
    }
}
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ToArrayAsyncTest
    {
        [Test]
        public void R3_ToArrayAsync_配列に変換する()
        {
            using var subject = new R3.Subject<int>();

            var task = subject.ToArrayAsync();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            // 完了
            Assert.IsTrue(task.IsCompleted);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, task.Result);
        }

        [Test]
        public void UniRx_ToArray()
        {
            using var subject = new UniRx.Subject<int>();

            var result = default(int[]);

            // UniRxのToArrayはObservable<T[]>を返す
            subject.ToArray().Subscribe(x => result = x);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // まだ完了していない
            Assert.IsNull(result);

            subject.OnCompleted();

            // 完了
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }
    }
}
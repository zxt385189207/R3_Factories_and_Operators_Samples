using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ToListAsyncTest
    {
        [Test]
        public void R3_ToListAsync_Listに変換する()
        {
            using var subject = new R3.Subject<int>();

            var task = subject.ToListAsync();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            // 完了
            Assert.IsTrue(task.IsCompleted);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, task.Result);
        }

        [Test]
        public void UniRx_ToList()
        {
            using var subject = new UniRx.Subject<int>();

            // IList<T>になる
            var result = default(IList<int>);

            subject.ToList().Subscribe(x => result = x);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // まだ完了していない
            Assert.IsNull(result);

            subject.OnCompleted();

            // 完了
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new　List<int> { 1, 2, 3 }, result);
        }
    }
}
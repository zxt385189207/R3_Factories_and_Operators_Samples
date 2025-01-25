using System;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public class ElementAtAsyncTest
    {
        [Test]
        public async Task R3_ElementAtAsync_購読開始から指定した個数目の要素を取り出す()
        {
            using var subject = new R3.Subject<int>();

            // 4番目の要素を取得する(ゼロオリジン）
            var task = subject.ElementAtAsync(3);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(1);
            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(4);
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(4, await task);
        }

        [Test]
        public void R3_ElementAtAsync_個数が足りない場合は例外()
        {
            using var subject = new R3.Subject<int>();

            // 3番目の要素を取得する
            var task = subject.ElementAtAsync(3);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            // 完了
            subject.OnCompleted();

            // 失敗して例外が発生している
            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await task);
        }

        [Test]
        public void UniRx_ElementAtAsyncに相当するものはない()
        {
            Assert.Ignore();
        }
    }
}
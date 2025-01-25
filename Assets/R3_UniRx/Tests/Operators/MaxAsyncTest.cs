using System;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class MaxAsyncTest
    {
        private record Data(int Value);

        [Test]
        public void R3_MaxAsync_OnNextから最大値を取り出しその結果となる値を返す()
        {
            using var subject = new R3.Subject<Data>();

            var task = subject.MaxAsync(x => x.Value);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(new Data(1));
            subject.OnNext(new Data(100));
            subject.OnNext(new Data(3));
            subject.OnNext(new Data(400));

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            // Observableが完了することでTaskも完了する
            Assert.IsTrue(task.IsCompleted);

            // 結果の「400」が出力される
            Assert.AreEqual(400, task.Result);
        }

        [Test]
        public void R3_MaxAsync_値が存在しない場合は例外()
        {
            using var subject = new R3.Subject<Data>();

            // Task<Data>ではなくTask<int>になっている
            Task<int> task = subject.MaxAsync(x => x.Value);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
        }

        [Test]
        public void UniRx_Maxは存在しない()
        {
            Assert.Ignore();
        }
    }
}
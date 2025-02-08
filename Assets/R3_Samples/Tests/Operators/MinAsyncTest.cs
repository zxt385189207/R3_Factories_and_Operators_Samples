using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class MinAsyncTest
    {
        private record Data(int Value)
        {
            public int Value { get; } = Value;
        }

        [Test]
        public void R3_MinAsync_OnNextから最小値を取り出しその結果となる値を返す()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
           
            using var subject = new R3.Subject<Data>();

            var task = subject.MinAsync(x => x.Value, cancellationToken: ct);

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

            // 結果の「1」が出力される
            Assert.AreEqual(1, task.Result);
        }

        [Test]
        public void R3_MinAsync_値が存在しない場合は例外()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            using var subject = new R3.Subject<Data>();

            // Task<Data>ではなくTask<int>になっている
            Task<int> task = subject.MinAsync(x => x.Value, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
        }

        [Test]
        public void UniRx_Minは存在しない()
        {
            Assert.Ignore();
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class MaxByAsyncTest
    {
        private record Data(int Value)
        {
            public int Value { get; } = Value;
        }

        [Test]
        public async Task R3_MaxByAsync_OnNextから最大値を取り出しその値を含んだOnNext値を返す()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<Data>();

            // Task<Data>である
            Task<Data> task = subject.MaxByAsync(x => x.Value, cancellationToken: ct);

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

            var resultData = await task;

            // 結果の「400」を含んだDataが出力される
            Assert.AreEqual(new Data(400), resultData);
        }

        [Test]
        public void R3_MaxByAsync_値が存在しない場合は例外()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<Data>();

            var task = subject.MaxByAsync(x => x.Value, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
        }

        [Test]
        public void UniRx_MaxByは存在しない()
        {
            Assert.Ignore();
        }
    }
}
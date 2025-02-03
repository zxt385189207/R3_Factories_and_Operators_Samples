using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class WaitAsyncTest
    {
        [Test]
        public void R3_WaitAsync_完了を待つ()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            using var subject = new R3.Subject<int>();

            // WaitAsyncはOnNext値は返さない
            var task = subject.WaitAsync(cancellationToken: ct);

            subject.OnNext(1);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            // 完了
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void UniRx_WaitOrToTask()
        {
            // Waitで完了を待機できるが、同期的に待つため非常に危険
            UniRx.Observable.Return(1).Wait();

            using var subject = new UniRx.Subject<int>();

            // ToTaskでただ完了を待ちたい場合はDefaultIfEmptyを挟まないと
            // OnNextがゼロ個だったときに異常終了となってしまう
            var task = subject.DefaultIfEmpty().ToTask();

            subject.OnCompleted();
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
        }
    }
}
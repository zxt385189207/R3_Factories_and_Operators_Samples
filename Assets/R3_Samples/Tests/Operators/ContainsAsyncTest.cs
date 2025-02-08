using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class ContainsAsyncTest
    {
        [Test]
        public async Task R3_ContainsAsync_指定した値を含んだOnNextが発行されたら即座に完了するTaskに変換する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3が発行されたか？
            // AnyAsyncとの違いはこちらは値そのものを引数に取る。
            var task = subject.ContainsAsync(3, cancellationToken: ct);

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }


        [Test]
        public async Task UniRx_FirstOrDefaultで再現する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new UniRx.Subject<int>();

            // 3が発行されたか？
            var task = subject.FirstOrDefault(x => x == 3).Select(_ => true).DefaultIfEmpty(false).ToTask(ct);

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }
    }
}
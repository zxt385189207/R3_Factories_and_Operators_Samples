using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;


namespace R3_UniRx.Tests.Operators
{
    public class AnyAsyncTest
    {
        [Test]
        public async Task R3_AnyAsync_OnNextが発行されたら即座に完了するTasに変換する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            // OnNext発行するとき
            using var subject1 = new R3.Subject<int>();

            // OnNextが発行されたら完了するTask
            var task = subject1.AnyAsync(cancellationToken: ct);

            Assert.IsFalse(task.IsCompleted);
            subject1.OnNext(0);
            // 完了している
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task); // 結果はtrue

            // --- 

            // OnNext発行しないとき
            using var subject2 = new R3.Subject<int>();
            var task2 = subject2.AnyAsync(cancellationToken: ct);
            Assert.IsFalse(task2.IsCompleted);
            subject2.OnCompleted();

            // 完了している
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsFalse(await task2); // 結果はfalse
        }


        [Test]
        public async Task R3_AnyAsync_条件を満たしたOnNextが発行されたら即座に完了するTasに変換する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3が発行されたか？
            // ContainsAsyncとの違いはこちらはFuncを引数に取る。
            var task = subject.AnyAsync(x => x == 3, cancellationToken: ct);

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }


        [Test]
        public async Task UniRx_AnyAsyncをFirstOrDefaultで再現する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            // Anyは存在しないのでFirstOrDefaultで代用
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
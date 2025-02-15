using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class FirstOrDefaultAsyncTest
    {
        [Test]
        public async Task R3_FirstOrDefaultAsync_一番最初に条件を満たしたOnNextを待つ()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3以上の最初の要素を取得する
            var task = subject.FirstOrDefaultAsync(x => x >= 3, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(1);
            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            // 条件を満たす要素
            subject.OnNext(100);
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(100, await task);
        }

        [Test]
        public void R3_FirstOrDefaultAsync_条件を満たす要素が存在しない場合は既定値()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3以上の最初の要素を取得する
            var task = subject.FirstOrDefaultAsync(x => x >= 3, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            // 完了
            subject.OnCompleted();

            // 成功してintの既定値が返っている
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(0, task.Result);
        }

        [Test]
        public void UniRx_FirstOrDefault()
        {
            using var subject = new UniRx.Subject<int>();

            int? result = null;

            // 3以上の最初の要素を取得する
            subject
                .FirstOrDefault(x => x >= 3)
                .Subscribe(x => result = x);
            
            Assert.IsNull(result);
            
            subject.OnNext(1);
            subject.OnNext(2);
            Assert.IsNull(result);
            
            // 条件を満たす要素
            subject.OnNext(100);
            Assert.AreEqual(100, result);
        }
    }
}
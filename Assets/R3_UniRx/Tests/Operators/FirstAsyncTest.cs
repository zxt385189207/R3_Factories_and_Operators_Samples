using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public class FirstAsyncTest
    {
        [Test]
        public async Task R3_FirstAsync_一番最初に条件を満たしたOnNextを待つ()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3以上の最初の要素を取得する
            var task = subject.FirstAsync(x => x >= 3, cancellationToken: ct);

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
        public void R3_FirstAsync_条件を満たす要素が存在しない場合は例外()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3以上の最初の要素を取得する
            var task = subject.FirstAsync(x => x >= 3, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            // 完了
            subject.OnCompleted();

            // 失敗して例外が発生している
            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
        }

        [Test]
        public void UniRx_First()
        {
            using var subject = new UniRx.Subject<int>();

            int? result = null;

            // 3以上の最初の要素を取得する
            subject
                .First(x => x >= 3)
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
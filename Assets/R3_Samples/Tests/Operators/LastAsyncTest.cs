using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class LastAsyncTest
    {
        [Test]
        public async Task R3_LastAsync_一番最後に条件を満たしたOnNextを待つ()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3以上の最初の要素を取得する
            var task = subject.LastAsync(x => x >= 3, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            // 条件を満たす要素
            subject.OnNext(100);
            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            // 条件を満たす要素
            subject.OnNext(200);
            Assert.IsFalse(task.IsCompleted);

            // 完了
            subject.OnCompleted();

            // Observableが完了することでTaskも完了する
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(200, await task);
        }

        [Test]
        public void R3_LastAsync_条件を満たす要素が存在しない場合は例外()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3以上の最初の要素を取得する
            var task = subject.LastAsync(x => x >= 3, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            // 完了
            subject.OnCompleted();

            // 失敗して例外が発生している
            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
        }

        [Test]
        public void UniRx_Last()
        {
            using var subject = new UniRx.Subject<int>();

            int? result = null;

            // 3以上の最初の要素を取得する
            subject
                .Last(x => x >= 3)
                .Subscribe(x =>
                {
                    if (result != null) Assert.Fail();
                    result = x;
                });

            Assert.IsNull(result);

            // 条件を満たす要素
            subject.OnNext(100);
            subject.OnNext(1);
            subject.OnNext(2);
            Assert.IsNull(result);

            // 条件を満たす要素
            subject.OnNext(200);
            subject.OnCompleted();
            Assert.AreEqual(200, result);
        }
    }
}
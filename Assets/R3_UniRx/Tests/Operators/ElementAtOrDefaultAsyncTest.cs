using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public class ElementAtOrDefaultAsyncTest
    {
        [Test]
        public async Task R3_ElementAtOrDefaultAsync_購読開始から指定した個数目の要素を取り出す()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 4番目の要素を取得する(ゼロオリジン）
            var task = subject.ElementAtOrDefaultAsync(3, cancellationToken: ct);

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
        public async Task R3_ElementAtOrDefaultAsync_個数が足りない場合は既定値()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            using var subject = new R3.Subject<int>();

            // 3番目の要素を取得する
            var task = subject.ElementAtOrDefaultAsync(3, cancellationToken: ct);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            // 完了
            subject.OnCompleted();

            // 成功してintの既定値が返っている
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(0, await task);
        }

        [Test]
        public void UniRx_ElementAtAsyncに相当するものはない()
        {
            Assert.Ignore();
        }
    }
}
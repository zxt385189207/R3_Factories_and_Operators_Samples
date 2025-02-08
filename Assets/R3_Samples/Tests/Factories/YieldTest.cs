using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class YieldTest
    {
        [Test]
        public async Task Yield_スレッドプールからメッセージを発行する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            // スレッドプールからメッセージを発行する
            // ConfigureAwait(false)なのでこのawait以降はスレッドプールが継続する
            await Observable.Yield(ct).FirstAsync(cancellationToken: ct).ConfigureAwait(false);

            var currentThreadId = Thread.CurrentThread.ManagedThreadId;

            Assert.AreNotEqual(mainThreadId, currentThreadId);
        }

        [Test]
        public async Task Yield_指定したTimeProviderの次の実行タイミングでメッセージを発行する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            // スレッドプールに切り替える
            await UniTask.SwitchToThreadPool();

            // ここはスレッドプール上
            var threadId = Thread.CurrentThread.ManagedThreadId;

            // 次のUpdateの実行タイミングでメッセージを発行する
            var executeThreadId = await Observable.Yield(UnityTimeProvider.Update)
                .Select(_ => Thread.CurrentThread.ManagedThreadId)
                .FirstAsync(ct);
            
            // メインスレッドとスレッドプールのIdは違う
            Assert.AreNotEqual(mainThreadId, threadId);

            // Observable.Yieldの実行コンテキストはUnityのメインスレッドだった
            Assert.AreEqual(mainThreadId, executeThreadId);
        }
    }
}
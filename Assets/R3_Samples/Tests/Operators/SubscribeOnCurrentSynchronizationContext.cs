using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class SubscribeOnCurrentSynchronizationContext
    {
        [Test]
        public async Task R3_SubscribeOnCurrentSynchronizationContext_現在のSynchronizationContext上で購読を行う()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            
            // メインスレッドのIDを取得する
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var result = -1;

            // Observableの定義だけ先に行う
            var observable = R3.Observable.Defer(() =>
                {
                    // Subscribe時の実行スレッドのIDを取得してそのままOnNextとして発行する
                    var observableCreationId = Thread.CurrentThread.ManagedThreadId;
                    return R3.Observable.Return(observableCreationId);
                })
                // SyncContextを指定する、すなわちどのタイミングで購読してもメインスレッド上でSubscribeすることになる
                .SubscribeOnCurrentSynchronizationContext();

            await UniTask.RunOnThreadPool(async () =>
            {
                // ここはスレッドプール上である
                Assert.IsTrue(Thread.CurrentThread.IsThreadPoolThread);

                var threadPoolThreadId = Thread.CurrentThread.ManagedThreadId;

                // スレッドプールIdとメインスレッドIdは当然異なる
                Assert.IsTrue(threadPoolThreadId != mainThreadId);

                // スレッドプール上で購読
                await observable.ForEachAsync(x => result = x, cancellationToken: ct);
            }, cancellationToken: ct);

            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }

        [Test]
        public void UniRx_SubscribeOnCurrentSynchronizationContextは存在しない()
        {
            Assert.Ignore();
        }
    }
}
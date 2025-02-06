using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SubscribeOnMainThread
    {
        [Test]
        public async Task R3_SubscribeOnMainThread_メインスレッド上で購読を行う()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            
            // メインスレッドのIDを取得する
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var result = -1;

            await UniTask.RunOnThreadPool(async () =>
            {
                // ここはスレッドプール上である
                Assert.IsTrue(Thread.CurrentThread.IsThreadPoolThread);

                var threadPoolThreadId = Thread.CurrentThread.ManagedThreadId;

                // スレッドプールIdとメインスレッドIdは当然異なる
                Assert.IsTrue(threadPoolThreadId != mainThreadId);

                await R3.Observable.Defer(() =>
                    {
                        // Subscribe時の実行スレッドのIDを取得してそのままOnNextとして発行する
                        var observableCreationId = Thread.CurrentThread.ManagedThreadId;
                        return R3.Observable.Return(observableCreationId);
                    })
                    .SubscribeOnMainThread() // メインスレッド上でSubscribeすることになる
                    .ForEachAsync(x => result = x, cancellationToken: ct);
            }, cancellationToken: ct);

            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }

        [Test]
        public async Task UniRx_SubscribeOnMainThread()
        {
            // メインスレッドのIDを取得する
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var result = -1;

            await UniTask.RunOnThreadPool(async () =>
            {
                // ここはスレッドプール上である
                Assert.IsTrue(Thread.CurrentThread.IsThreadPoolThread);

                var threadPoolThreadId = Thread.CurrentThread.ManagedThreadId;

                // スレッドプールIdとメインスレッドIdは当然異なる
                Assert.IsTrue(threadPoolThreadId != mainThreadId);

                await UniRx.Observable.Defer(() =>
                    {
                        // Subscribe時の実行スレッドのIDを取得してそのままOnNextとして発行する
                        var observableCreationId = Thread.CurrentThread.ManagedThreadId;
                        return UniRx.Observable.Return(observableCreationId);
                    })
                    .SubscribeOnMainThread() // メインスレッド上でSubscribeすることになる
                    .ForEachAsync(x => result = x);
            });

            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }
    }
}
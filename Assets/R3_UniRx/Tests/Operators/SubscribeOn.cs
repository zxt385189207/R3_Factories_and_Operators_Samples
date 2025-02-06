using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    /*
     * 補足: Observable.Return()は購読した瞬間に同期的にOnNextを発行する
     */
    public sealed class SubscribeOn
    {
        [Test]
        public async Task R3_SubscribeOn_指定したSynchronizationContext上で購読を行う()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            
            // Unityが用意しているメインスレッド用のSynchronizationContextを取得する
            var currentSyncContext = SynchronizationContext.Current;

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
                    .SubscribeOn(currentSyncContext) // SyncContextを指定する、すなわちメインスレッド上でSubscribeすることになる
                    .ForEachAsync(x => result = x, cancellationToken: ct);
            }, cancellationToken: ct);

            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }

        [Test]
        public async Task R3_SubscribeOn_指定したFrameProvider上で購読を行う()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            
            // Fakeだけど、UnityのFrameProviderと仮定する
            // Advance()を実行したコンテキストでSubscribeが実行される
            var fakeFrameProvider = new FakeFrameProvider();

            // メインスレッドのIDを取得する
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var result = -1;

            await UniTask.RunOnThreadPool(() =>
            {
                // ここはスレッドプール上である
                Assert.IsTrue(Thread.CurrentThread.IsThreadPoolThread);

                var threadPoolThreadId = Thread.CurrentThread.ManagedThreadId;

                // スレッドプールIdとメインスレッドIdは当然異なる
                Assert.IsTrue(threadPoolThreadId != mainThreadId);

                R3.Observable.Defer(() =>
                    {
                        // Subscribe時の実行スレッドのIDを取得してそのままOnNextとして発行する
                        var observableCreationId = Thread.CurrentThread.ManagedThreadId;
                        return R3.Observable.Return(observableCreationId);
                    })
                    .SubscribeOn(fakeFrameProvider) // SyncContextを指定する、すなわちメインスレッド上でSubscribeすることになる
                    .Subscribe(x => result = x);
            }, cancellationToken: ct);

            // まだSubscribeされてない
            Assert.AreEqual(-1, result);

            // 1F進むことで、SubscribeOnが実行され購読される
            fakeFrameProvider.Advance();

            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }

        [Test]
        public async Task R3_SubscribeOn_指定したTimeProvider上で購読を行う()
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
                    .SubscribeOn(UnityTimeProvider.Update) // UnityのUpdateを指定する、すなわちメインスレッド上でSubscribeすることになる
                    .ForEachAsync(x => result = x, cancellationToken: ct);
            }, cancellationToken: ct);

            
            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }
        
        [Test]
        public async Task UniRx_SubscribeOn()
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
                    .SubscribeOn(Scheduler.MainThread) // メインスレッド上でSubscribeすることになる
                    .ForEachAsync(x => result = x);
            });

            
            // メインスレッド上で購読されたので、メインスレッドのIDが入っている
            Assert.AreEqual(result, mainThreadId);
        }
    }
}
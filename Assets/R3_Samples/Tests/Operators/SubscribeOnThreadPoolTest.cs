using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class SubscribeOnThreadPool
    {
        [Test]
        public async Task R3_SubscribeOnThreadPool_スレッドプール上で購読を行う()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            
            // メインスレッドのIDを取得する
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var result = -1;

            await R3.Observable.Defer(() =>
                {
                    // ここはスレッドプール上である
                    Assert.IsTrue(Thread.CurrentThread.IsThreadPoolThread);

                    // Subscribe時の実行スレッドのIDを取得してそのままOnNextとして発行する
                    var observableCreationId = Thread.CurrentThread.ManagedThreadId;
                    return R3.Observable.Return(observableCreationId);
                })
                .SubscribeOnThreadPool()
                .ForEachAsync(x => result = x, cancellationToken: ct);


            // メインスレッドで購読したが、SubscribeOnThreadPoolによりスレッドプール上で購読されたので
            // スレッドプールのIDが入っている
            Assert.AreNotEqual(result, mainThreadId);
        }

        [Test]
        public async Task UniRx_SubscribeOnでSubscribeOnThreadPoolを代替する()
        {
            // メインスレッドのIDを取得する
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var result = -1;

            await UniRx.Observable.Defer(() =>
                {
                    // ここはスレッドプール上である
                    Assert.IsTrue(Thread.CurrentThread.IsThreadPoolThread);

                    // Subscribe時の実行スレッドのIDを取得してそのままOnNextとして発行する
                    var observableCreationId = Thread.CurrentThread.ManagedThreadId;
                    return UniRx.Observable.Return(observableCreationId);
                })
                .SubscribeOn(Scheduler.ThreadPool)
                .ForEachAsync(x => result = x);


            // メインスレッドで購読したが、SubscribeOnThreadPoolによりスレッドプール上で購読されたので
            // スレッドプールのIDが入っている
            Assert.AreNotEqual(result, mainThreadId);
        }
    }
}
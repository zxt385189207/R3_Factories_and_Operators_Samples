using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ObserveOnMainThreadTest
    {
        [Test]
        public async Task R3_ObserveOnMainThread_実行コンテキストをUnityメインスレッドへ変更する()
        {
            // メインスレッドId
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            // 発行元のスレッドId
            var publishThreadId = -1;
            // 受信したときのスレッドId
            var subscribeThreadId = -1;

            await R3.Observable.Create<int>(async (observer, _) =>
                {
                    await Task.Run(() =>
                        {
                            // スレッドプールに切り替えて、そこのスレッドIdを返す。
                            observer.OnNext(Thread.CurrentThread.ManagedThreadId);
                            observer.OnCompleted();
                        }, _)
                        .ConfigureAwait(false);
                })
                .Do(x => publishThreadId = x)
                .ObserveOnMainThread()
                .ForEachAsync(x => subscribeThreadId = Thread.CurrentThread.ManagedThreadId);

            // 発行元と受信側は違うスレッド
            Assert.AreNotEqual(publishThreadId, subscribeThreadId);
            // 受信側はメインスレッドになっている
            Assert.AreEqual(mainThreadId, subscribeThreadId);
        }

        [Test]
        public async Task UniRx_ObserveOnMainThread()
        {
            // メインスレッドId
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            // 発行元のスレッドId
            var publishThreadId = -1;
            // 受信したときのスレッドId
            var subscribeThreadId = -1;

            await UniRx.Observable.Create<int>(observer =>
                {
                    Task.Run(() =>
                    {
                        // スレッドプールに切り替えて、そこのスレッドIdを返す。
                        observer.OnNext(Thread.CurrentThread.ManagedThreadId);
                        observer.OnCompleted();
                    });
                    return UniRx.Disposable.Empty;
                })
                .Do(x => publishThreadId = x)
                .ObserveOnMainThread()
                .ForEachAsync(x => subscribeThreadId = Thread.CurrentThread.ManagedThreadId);

            // 発行元と受信側は違うスレッド
            Assert.AreNotEqual(publishThreadId, subscribeThreadId);
            // 受信側はメインスレッドになっている
            Assert.AreEqual(mainThreadId, subscribeThreadId);
        }
    }
}
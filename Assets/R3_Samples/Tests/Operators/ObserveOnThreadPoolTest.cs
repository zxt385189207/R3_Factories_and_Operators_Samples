using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class ObserveOnThreadPoolTest
    {
        [Test]
        public async Task R3_ObserveOnThreadPool_実行コンテキストをスレッドプールに切り替える()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            
            // メインスレッドId
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            using var subject = new R3.Subject<R3.Unit>();

            // スレッドプールに切り替えて、そこのスレッドIdを返す。
            var task = subject
                .ObserveOnThreadPool()
                .Select(_ => Thread.CurrentThread.ManagedThreadId)
                .FirstAsync(cancellationToken: ct);

            // メインスレッドで発行
            subject.OnNext(R3.Unit.Default);
            subject.OnCompleted();

            var result = await task;

            // スレッドプールに切り替わっている
            Assert.AreNotEqual(mainThreadId, result);
        }

        [Test]
        public void UniRx_ObserveOnでObserveOnThreadPoolと同じことができる()
        {
            // メインスレッドId
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;

            using var subject = new UniRx.Subject<UniRx.Unit>();

            var result = -1;

            // スレッドプールに切り替えて、そこのスレッドIdを返す。
            subject.ObserveOn(Scheduler.ThreadPool)
                .Select(_ => Thread.CurrentThread.ManagedThreadId)
                .Subscribe(x => result = x);

            // メインスレッドで発行
            subject.OnNext(UniRx.Unit.Default);
            subject.OnCompleted();

            // スレッドプールに切り替わっている
            Assert.AreNotEqual(mainThreadId, result);
        }
    }
}
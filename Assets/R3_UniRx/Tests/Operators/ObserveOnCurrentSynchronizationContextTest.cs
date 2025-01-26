using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ObserveOnCurrentSynchronizationContextTest
    {
        [Test]
        public async Task R3_ObserveOnCurrentSynchronizationContext_SynchronizationContextで実行コンテキストを変更する()
        {
            // SynchronizationContextは存在する
            Assert.IsNotNull(SynchronizationContext.Current);
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
                .ObserveOnCurrentSynchronizationContext()
                .ForEachAsync(x => subscribeThreadId = Thread.CurrentThread.ManagedThreadId);

            // 発行元と受信側は違うスレッド
            Assert.AreNotEqual(publishThreadId, subscribeThreadId);
            // 受信側はメインスレッドになっている
            Assert.AreEqual(mainThreadId, subscribeThreadId);
        }

        [Test]
        public void UniRx_ObserveOnCurrentSynchronizationContextは存在しない()
        {
            Assert.Ignore();
        }
    }
}
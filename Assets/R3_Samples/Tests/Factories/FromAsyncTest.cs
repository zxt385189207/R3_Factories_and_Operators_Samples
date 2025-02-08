using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class FromAsyncTest
    {
        // 適当に待って100を返すメソッド
        private static async ValueTask<int> MethodAsync(UniTask waitTarget, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await waitTarget;
            ct.ThrowIfCancellationRequested();
            return 100;
        }

        [Test]
        public void FromAsync_ValueTaskをObservableに変換する()
        {
            var utcs = AutoResetUniTaskCompletionSource.Create();

            var observable = Observable.FromAsync(ct => MethodAsync(utcs.Task, ct));

            using var list = observable.ToLiveList();

            CollectionAssert.IsEmpty(list);

            utcs.TrySetResult();

            CollectionAssert.AreEqual(new[] { 100 }, list);
        }

        [Test]
        public void FromAsync_ValueTaskをObservableに変換する2()
        {
            // その場でasync/awaitを定義してもよい
            // Observable.Create()に似ているが、Observable.FromAsync()はreturnした値を1個だけしか返せない
            var observable = Observable.FromAsync(async ct =>
            {
                ct.ThrowIfCancellationRequested();
                await Task.CompletedTask;
                return 100;
            });

            using var list = observable.ToLiveList();
            CollectionAssert.AreEqual(new[] { 100 }, list);
        }
    }
}
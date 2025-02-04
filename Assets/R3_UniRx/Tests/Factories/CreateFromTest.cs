using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class CreateFromTest
    {
        private async IAsyncEnumerable<int> CreateAsyncEnumerable(FakeFrameProvider fakeFrameProvider,
            int waitValue,
            [EnumeratorCancellation] CancellationToken ct)
        {
            await fakeFrameProvider.WaitAsync(waitValue, ct: ct);
            yield return 1;
            await fakeFrameProvider.WaitAsync(waitValue, ct: ct);
            yield return 2;
            await fakeFrameProvider.WaitAsync(waitValue, ct: ct);
            yield return 3;
        }

        [Test]
        public void CreateFrom_AsyncEnumerableをObservableに変換する()
        {
            var scx = SynchronizationContext.Current;
            try
            {
                // SynchronizationContextをnullにしないとOnNext発行が遅れる
                // （なぜ…？どこが依存している…？)
                SynchronizationContext.SetSynchronizationContext(null);

                var fakeFrameProvider = new FakeFrameProvider();

                // IAsyncEnumerable<int>を作成し、それをObservableに変換する
                var observable = Observable.CreateFrom(ct => CreateAsyncEnumerable(fakeFrameProvider, 1, ct));

                var list = observable.ToLiveList();

                CollectionAssert.IsEmpty(list);
                fakeFrameProvider.Advance();

                CollectionAssert.AreEqual(new[] { 1 }, list);

                fakeFrameProvider.Advance();

                CollectionAssert.AreEqual(new[] { 1, 2 }, list);

                fakeFrameProvider.Advance();

                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(scx);
            }
        }

        private record Data(int Value);

        [Test]
        public void CreateFrom_AsyncEnumerableをObservableに変換する_State()
        {
            var scx = SynchronizationContext.Current;
            try
            {
                // SynchronizationContextをnullにしないとOnNext発行が遅れる
                // （なぜ…？どこが依存している…？)
                SynchronizationContext.SetSynchronizationContext(null);
                
                var data = new Data(3);

                var fakeFrameProvider = new FakeFrameProvider();

                // IAsyncEnumerable<int>を作成し、それをObservableに変換する
                // デリゲート内で使う値をあらかじめ渡しておくことができる
                var observable =
                    Observable.CreateFrom((data, fakeFrameProvider),
                        (ct, v) => CreateAsyncEnumerable(v.fakeFrameProvider, v.data.Value, ct));

                var list = observable.ToLiveList();

                CollectionAssert.IsEmpty(list);
                fakeFrameProvider.Advance(3);

                CollectionAssert.AreEqual(new[] { 1 }, list);

                fakeFrameProvider.Advance(3);

                CollectionAssert.AreEqual(new[] { 1, 2 }, list);

                fakeFrameProvider.Advance(3);

                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(scx);
            }
        }
    }
}
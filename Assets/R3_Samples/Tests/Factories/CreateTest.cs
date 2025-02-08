using NUnit.Framework;
using R3;
using Disposable = R3.Disposable;

namespace R3_Samples.Tests.Factories
{
    public sealed class CreateTest
    {
        [Test]
        public void Create_手続き的に値を発行するObservableを作成する_同期()
        {
            var observable = Observable.Create<int>(observer =>
            {
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                observer.OnCompleted();

                // Dispose時に呼ばれる処理
                return Disposable.Empty;
            });

            using var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                3
            }, list);
        }

        private record Data(int Value)
        {
            public int Value { get; } = Value;
        }

        [Test]
        public void Create_手続き的に値を発行するObservableを作成する_同期_State()
        {
            var value = new Data(10);

            // デリゲート内で使う値をあらかじめ渡しておくことができる
            var observable = Observable.Create<int, Data>(value, (observer, baseValue) =>
            {
                observer.OnNext(1 + baseValue.Value);
                observer.OnNext(2 + baseValue.Value);
                observer.OnNext(3 + baseValue.Value);
                observer.OnCompleted();

                // Dispose時に呼ばれる処理
                return Disposable.Empty;
            }, false);

            using var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                11,
                12,
                13
            }, list);
        }

        [Test]
        public void Create_手続き的に値を発行するObservableを作成する_非同期()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            // async/awaitを使うこともできる
            var observable = Observable.Create<int>(async (observer, ct) =>
            {
                observer.OnNext(1);
                await fakeFrameProvider.WaitAsync(1, ct);
                observer.OnNext(2);
                await fakeFrameProvider.WaitAsync(1, ct);
                observer.OnNext(3);
                observer.OnCompleted();
            });

            using var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                1
            }, list);

            fakeFrameProvider.Advance();

            CollectionAssert.AreEqual(new[]
            {
                1,
                2
            }, list);

            fakeFrameProvider.Advance();

            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                3
            }, list);
        }


        [Test]
        public void Create_手続き的に値を発行するObservableを作成する_非同期_State()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            var data = new Data(10);

            // async/awaitを使うこともできる
            // デリゲート内で使う値をあらかじめ渡しておくことができる
            var observable = Observable.Create<int, Data>(data, async (observer, baseValue, ct) =>
            {
                observer.OnNext(1 + baseValue.Value);
                await fakeFrameProvider.WaitAsync(1, ct);
                observer.OnNext(2 + baseValue.Value);
                await fakeFrameProvider.WaitAsync(1, ct);
                observer.OnNext(3 + baseValue.Value);
                observer.OnCompleted();
            });

            using var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                11
            }, list);

            fakeFrameProvider.Advance();

            CollectionAssert.AreEqual(new[]
            {
                11,
                12
            }, list);

            fakeFrameProvider.Advance();

            CollectionAssert.AreEqual(new[]
            {
                11,
                12,
                13
            }, list);
        }
    }
}
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public sealed class AggregateAsyncTest
    {
        [Test]
        public async Task R3_AggregateAsync_値の畳み込みを行う()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AggregateAsync((p, c) => p + c);

            Assert.AreEqual(15, result);
        }

        [Test]
        public async Task R3_AggregateAsync_初期値を用いて値の畳み込みを行う()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AggregateAsync(100, (p, c) => p + c); // 初期値は100から始める

            Assert.AreEqual(115, result);
        }

        [Test]
        public async Task R3_AggregateAsync_畳み込み後に最終結果を取り出す()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AggregateAsync(100, (p, c) => p + c, result => result * 100); // 結果を100倍する

            Assert.AreEqual(11500, result);
        }

        [Test]
        public async Task UniRx_Aggregate()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRxObservable.ToObservable(array)
                .Aggregate((p, c) => p + c)
                .ToTask();

            Assert.AreEqual(15, result);
        }
    }
}
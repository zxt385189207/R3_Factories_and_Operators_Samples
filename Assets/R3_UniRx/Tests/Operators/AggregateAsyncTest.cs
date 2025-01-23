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
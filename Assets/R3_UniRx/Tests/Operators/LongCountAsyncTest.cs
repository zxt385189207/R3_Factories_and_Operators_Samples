using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;


namespace R3_UniRx.Tests.Operators
{
    public class LongCountAsyncTest
    {
        [Test]
        public async Task R3_LongCount_購読中に発行された値の個数を返す()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .LongCountAsync();

            Assert.AreEqual(5, result);
        }


        [Test]
        public async Task R3_LongCount_購読中に発行された条件を満たす値の個数を返す()
        {
            // OnCompleted発行時に、購読中に発行された条件を満たす値の個数を返す。
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .LongCountAsync(x => x % 2 == 0); // 偶数の個数を数える

            Assert.AreEqual(2, result);
        }


        [Test]
        public async Task UniRx_AggregateでLongCountを再現する()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRx.Observable.ToObservable(array)
                .Aggregate<int, long>(0, (prev, curr) => curr % 2 == 0 ? prev + 1 : prev)
                .ToTask();

            Assert.AreEqual(2, result);
        }
    }
}
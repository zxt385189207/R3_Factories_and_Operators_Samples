using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class CountAsyncTest
    {
        [Test]
        public async Task R3_Count_購読中に発行された条件を満たす値の個数を返す()
        {
            // OnCompleted発行時に、購読中に発行された条件を満たす値の個数を返す。
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .CountAsync(x => x % 2 == 0); // 偶数の個数を数える

            Assert.AreEqual(2, result);

        }


        [Test]
        public async Task UniRx_AggregateでCountを再現する()
        {

            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRxObservable.ToObservable(array)
                .Aggregate(0, (prev, curr) => curr % 2 == 0 ? prev + 1 : prev)
                .ToTask();

            Assert.AreEqual(2, result);
        }
    }
}
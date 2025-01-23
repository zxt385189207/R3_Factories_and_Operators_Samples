using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class AverageAsyncTest
    {
        [Test]
        public async Task R3_AverageAsync_購読中に発行された値の平均値を求める()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AverageAsync();

            Assert.AreEqual(3.0, result);
        }

        [Test]
        public async Task UniRx_AverageをLINQで再現する()
        {
            // Averageは存在しないのでToArray()してLINQのAverageで代用

            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();

            Assert.AreEqual(3.0, result.Average());
        }
    }
}
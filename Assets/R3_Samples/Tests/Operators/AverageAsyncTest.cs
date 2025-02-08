using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class AverageAsyncTest
    {
        [Test]
        public async Task R3_AverageAsync_購読中に発行された値の平均値を求める()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .AverageAsync(cancellationToken: ct);

            Assert.AreEqual(3.0, result);
        }


        private record Data(int Value)
        {
            public int Value { get; } = Value;
        }

        [Test]
        public async Task R3_AverageAsync_購読中に発行された値の平均値を求めるwithSelector()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            var array = new[] { new Data(1), new Data(2), new Data(3), new Data(4), new Data(5) };

            // Valueプロパティを指定して平均値を求める
            var result = await R3.Observable.ToObservable(array)
                .AverageAsync(x => x.Value, cancellationToken: ct);

            Assert.AreEqual(3.0, result);
        }

        [Test]
        public async Task UniRx_AverageをLINQで再現する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            // Averageは存在しないのでToArray()してLINQのAverageで代用

            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRx.Observable.ToObservable(array)
                .ToArray()
                .ToTask(ct);

            Assert.AreEqual(3.0, result.Average());
        }
    }
}
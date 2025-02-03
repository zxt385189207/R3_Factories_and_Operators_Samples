using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;


namespace R3_UniRx.Tests.Operators
{
    public class CountAsyncTest
    {
        [Test]
        public async Task R3_Count_購読中に発行された値の個数を返す()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .CountAsync(cancellationToken: ct);

            Assert.AreEqual(5, result);
        }


        [Test]
        public async Task R3_Count_購読中に発行された条件を満たす値の個数を返す()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            // OnCompleted発行時に、購読中に発行された条件を満たす値の個数を返す。
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .CountAsync(x => x % 2 == 0, cancellationToken: ct); // 偶数の個数を数える

            Assert.AreEqual(2, result);
        }


        [Test]
        public async Task UniRx_AggregateでCountを再現する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRx.Observable.ToObservable(array)
                .Aggregate(0, (prev, curr) => curr % 2 == 0 ? prev + 1 : prev)
                .ToTask(ct);

            Assert.AreEqual(2, result);
        }
    }
}
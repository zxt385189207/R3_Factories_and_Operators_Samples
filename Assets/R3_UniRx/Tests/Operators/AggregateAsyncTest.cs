using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;


namespace R3_UniRx.Tests.Operators
{
    public sealed class AggregateAsyncTest
    {
        [Test]
        public async Task R3_AggregateAsync_値の畳み込みを行う()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .AggregateAsync((p, c) => p + c, cancellationToken: ct);

            Assert.AreEqual(15, result);
        }

        [Test]
        public async Task R3_AggregateAsync_初期値を用いて値の畳み込みを行う()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .AggregateAsync(100, (p, c) => p + c, cancellationToken: ct); // 初期値は100から始める

            Assert.AreEqual(115, result);
        }

        [Test]
        public async Task R3_AggregateAsync_畳み込み後に最終結果を取り出す()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3.Observable.ToObservable(array)
                .AggregateAsync(
                    seed: 100,
                    (p, c) => p + c,
                    result => result * 100, // 結果を100倍する
                    cancellationToken: ct);

            Assert.AreEqual(11500, result);
        }

        [Test]
        public async Task UniRx_Aggregate()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRx.Observable.ToObservable(array)
                .Aggregate((p, c) => p + c)
                .ToTask(ct);

            Assert.AreEqual(15, result);
        }
    }
}
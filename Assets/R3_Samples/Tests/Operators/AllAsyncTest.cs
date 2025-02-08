using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class AllAsyncTest
    {
        [Test]
        public async Task R3_AllAsync_過去に発行されたすべての値が条件を満たしているかを調べる()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            // OnCompleted発行時に、過去に発行されたすべての値が条件を満たしているかを調べる。
            var array = new[] { 1, 2, 3, 4, 5 };

            // ぜんぶ正の値か? => true
            var result1 = await R3.Observable.ToObservable(array)
                .AllAsync(x => x > 0, cancellationToken: ct);
            Assert.IsTrue(result1);

            // 3を含んでいないか? => false
            var result2 = await R3.Observable.ToObservable(array)
                .AllAsync(x => x != 3, cancellationToken: ct);
            Assert.IsFalse(result2);
        }

        [Test]
        public async Task UniRx_AllAsyncをLINQで再現する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            // Allは存在しないのでToArray()してLINQのAllで代用
            var array = new[] { 1, 2, 3, 4, 5 };

            // ぜんぶ正の値か? => true
            var result1 = await UniRx.Observable.ToObservable(array)
                .ToArray()
                .ToTask(ct);
            Assert.IsTrue(result1.All(x => x > 0));

            // 3を含んでいないか? => false
            var result2 = await UniRx.Observable.ToObservable(array)
                .ToArray()
                .ToTask(ct);
            Assert.IsFalse(result2.All(x => x != 3));
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class AllAsyncTest
    {
        [Test]
        public async Task R3_AllAsync_過去に発行されたすべての値が条件を満たしているかを調べる()
        {
            // OnCompleted発行時に、過去に発行されたすべての値が条件を満たしているかを調べる。
            var array = new[] { 1, 2, 3, 4, 5 };

            // ぜんぶ正の値か? => true
            var result1 = await R3Observable.ToObservable(array)
                .AllAsync(x => x > 0);
            Assert.IsTrue(result1);

            // 3を含んでいないか? => false
            var result2 = await R3Observable.ToObservable(array)
                .AllAsync(x => x != 3);
            Assert.IsFalse(result2);
        }

        [Test]
        public async Task UniRx_AllAsyncをLINQで再現する()
        {
            // Allは存在しないのでToArray()してLINQのAllで代用
            var array = new[] { 1, 2, 3, 4, 5 };

            // ぜんぶ正の値か? => true
            var result1 = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();
            Assert.IsTrue(result1.All(x => x > 0));

            // 3を含んでいないか? => false
            var result2 = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();
            Assert.IsFalse(result2.All(x => x != 3));
        }
    }
}
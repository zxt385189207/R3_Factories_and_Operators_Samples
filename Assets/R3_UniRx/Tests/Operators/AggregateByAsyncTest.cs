using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class AggregateByAsyncTest
    {
        [Test]
        public async Task R3_AggregateByAsync_グループ単位で畳み込み計算する()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AggregateByAsync(
                    // 偶数グループと奇数グループに分けてそれぞれ合計値を計算
                    keySelector: key => key % 2,
                    seed: 0,
                    func: (total, cur) => total + cur);

            var actual = result.ToArray();

            Assert.AreEqual(9, actual[0].Value); // 偶数グループの合計値
            Assert.AreEqual(6, actual[1].Value); // 奇数グループの合計値
        }

        [Test]
        public void UniRx_AggregateByは存在しない()
        {
            Assert.Ignore();
        }
    }
}
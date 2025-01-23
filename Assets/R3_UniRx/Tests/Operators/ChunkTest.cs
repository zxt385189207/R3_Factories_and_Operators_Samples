using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class ChunkTest
    {
        [Test]
        public void R3_Chunk_countとskipを指定して値をまとめる()
        {
            // 1個飛ばしで2つずつまとめる
            // ≒ 現在値と一つ前の値をセットにする
            var observable = R3Observable.Range(1, 5).Chunk(count: 2, skip: 1);

            var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 3, 4 },
                new[] { 4, 5 },
                new[] { 5 }
            }, list.ToArray());
        }


        [Test]
        public async Task UniRx_Buffer()
        {
            // UniRxではBuffer
            var result =
                await UniRxObservable.Range(1, 5)
                    .Buffer(count: 2, skip: 1)
                    .ToArray()
                    .ToTask();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 3, 4 },
                new[] { 4, 5 },
                new[] { 5 }
            }, result.ToArray());
        }
    }
}
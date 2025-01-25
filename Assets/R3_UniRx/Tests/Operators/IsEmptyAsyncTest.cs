using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class IsEmptyAsyncTest
    {
        [Test]
        public async Task R3_IsEmptyAsync_シーケンスが空であるか()
        {
            {
                // OnNextがある場合は空ではない
                var result = await R3.Observable.Return(1).IsEmptyAsync();
                Assert.IsFalse(result);
            }

            {
                // OnCompletedのみは空判定
                var result = await R3.Observable.Empty<int>().IsEmptyAsync();
                Assert.IsTrue(result);
            }
        }

        [Test]
        public async Task UniRx_IsEmptyAsyncは存在しないのでSingleOrDefaultで代替する()
        {
            {
                // OnNextがある場合は空ではない
                var result = await UniRx.Observable.Return(1)
                    .Select(_ => true)
                    .SingleOrDefault()
                    .DefaultIfEmpty(false)
                    .ToTask();

                Assert.IsTrue(result);
            }

            {
                // OnCompletedのみは空判定
                var result = await UniRx.Observable.Empty<int>()
                    .Select(_ => true)
                    .SingleOrDefault()
                    .DefaultIfEmpty(false)
                    .ToTask();

                Assert.IsFalse(result);
            }
        }
    }
}
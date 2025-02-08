using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class IsEmptyAsyncTest
    {
        [Test]
        public async Task R3_IsEmptyAsync_シーケンスが空であるか()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            {
                // OnNextがある場合は空ではない
                var result = await R3.Observable.Return(1).IsEmptyAsync(cancellationToken: ct);
                Assert.IsFalse(result);
            }

            {
                // OnCompletedのみは空判定
                var result = await R3.Observable.Empty<int>().IsEmptyAsync(cancellationToken: ct);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public async Task UniRx_IsEmptyAsyncは存在しないのでSingleOrDefaultで代替する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;
            
            {
                // OnNextがある場合は空ではない
                var result = await UniRx.Observable.Return(1)
                    .Select(_ => true)
                    .SingleOrDefault()
                    .DefaultIfEmpty(false)
                    .ToTask(ct);

                Assert.IsTrue(result);
            }

            {
                // OnCompletedのみは空判定
                var result = await UniRx.Observable.Empty<int>()
                    .Select(_ => true)
                    .SingleOrDefault()
                    .DefaultIfEmpty(false)
                    .ToTask(ct);

                Assert.IsFalse(result);
            }
        }
    }
}
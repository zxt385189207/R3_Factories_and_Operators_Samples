using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SequenceEqualAsyncTest
    {
        [Test]
        public async Task R3_SequenceEqualAsync_2つのObservableの値が一致するか確認する()
        {
            var observable1 = R3.Observable.Range(1, 5);
            var observable2 = new[] { 1, 2, 3, 4, 5 }.ToObservable();

            var result = await observable1.SequenceEqualAsync(observable2);

            Assert.IsTrue(result);
        }

        [Test]
        public void UniRx_SequenceEqualに相当するものは存在しない()
        {
        }
    }
}
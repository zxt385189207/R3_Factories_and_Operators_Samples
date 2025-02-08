using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class ToHashSetAsyncTest
    {
        [Test]
        public void R3_ToHashSetAsync_HashSetに変換する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            using var subject = new R3.Subject<int>();

            var task = subject.ToHashSetAsync(cancellationToken: ct);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            // 完了
            Assert.IsTrue(task.IsCompleted);
            CollectionAssert.AreEqual(new HashSet<int> { 1, 2, 3 }, task.Result);
        }

        [Test]
        public void UniRx_ToHashSetは存在しないのでToArrayなどでがんばる()
        {
            using var subject = new UniRx.Subject<int>();

            var result = default(HashSet<int>);


            subject.ToArray().Subscribe(x => result = x.ToHashSet());

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // まだ完了していない
            Assert.IsNull(result);

            subject.OnCompleted();

            // 完了
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new　HashSet<int> { 1, 2, 3 }, result);
        }
    }
}
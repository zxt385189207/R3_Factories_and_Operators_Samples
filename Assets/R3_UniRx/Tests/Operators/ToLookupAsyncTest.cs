using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ToLookupAsyncTest
    {
        [Test]
        public void R3_ToLookupAsync_Lookupに変換する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            var subject = new R3.Subject<(int Key, string Value)>();

            var task = subject.ToLookupAsync(x => x.Key, x => x.Value, cancellationToken: ct);

            subject.OnNext((1, "a"));
            subject.OnNext((1, "b"));
            subject.OnNext((1, "c"));
            subject.OnNext((2, "A"));
            subject.OnNext((2, "B"));
            subject.OnNext((2, "C"));

            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);

            subject.OnCompleted();

            // 完了
            Assert.IsTrue(task.IsCompleted);

            var result = task.Result;

            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result[1]);
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, result[2]);
        }

        [Test]
        public void UniRx_ToLookupは存在しないのでToArrayなどでがんばる()
        {
            var subject = new UniRx.Subject<(int Key, string Value)>();

            var resultArray = default((int Key, string Value)[]);

            subject.ToArray().Subscribe(x => resultArray = x);

            subject.OnNext((1, "a"));
            subject.OnNext((1, "b"));
            subject.OnNext((1, "c"));
            subject.OnNext((2, "A"));
            subject.OnNext((2, "B"));
            subject.OnNext((2, "C"));
            subject.OnCompleted();
 
            var lookup = resultArray.ToLookup(x => x.Key, x => x.Value);

            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, lookup[1]);
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, lookup[2]);
        }
    }
}
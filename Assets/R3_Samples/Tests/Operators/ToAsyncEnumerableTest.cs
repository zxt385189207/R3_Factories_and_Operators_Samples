using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class ToAsyncEnumerableTest
    {
        [Test]
        public async Task R3_ToAsyncEnumerable_IAsyncEnumerableに変換する()
        {
            // キャンセルすることはないが、CancellationTokenは準備しておく
            var ct = CancellationToken.None;

            var asyncEnumerable = Observable.Range(1, 5).ToAsyncEnumerable(cancellationToken: ct);

            var results = new List<int>();
            
            await foreach (var value in asyncEnumerable)
            {
                results.Add(value);
            }
            
            CollectionAssert.AreEqual(new[]
            {
                1, 2, 3, 4, 5
            }, results);
        }
        
        [Test]
        public async Task UniRx_ToAsyncEnumerableは存在しないがUniTaskで代用可()
        {
            // UniTask.LINQの機能でUniTaskAsyncEnumerableに変換はできる
            var asyncEnumerable = UniRx.Observable.Range(1, 5).ToUniTaskAsyncEnumerable();

            var results = new List<int>();
            
            await foreach (var value in asyncEnumerable)
            {
                results.Add(value);
            }
            
            CollectionAssert.AreEqual(new[]
            {
                1, 2, 3, 4, 5
            }, results);
        }
    }
}
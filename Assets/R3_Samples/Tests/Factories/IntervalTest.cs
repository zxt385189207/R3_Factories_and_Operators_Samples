using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UnityEngine;

namespace R3_Samples.Tests.Factories
{
    public sealed class IntervalTest : MonoBehaviour
    {
        [Test]
        public async Task Interval_一定間隔で値を発行する()
        {
            // 100msごとに値を発行するObservable
            var interval = TimeSpan.FromMilliseconds(100);
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var results = new List<long>();

            // 3回値を発行するまで待機
            await Observable.Interval(interval, TimeProvider.System, ct)
                .Timestamp(TimeProvider.System)
                .Take(3)
                .ForEachAsync(x => results.Add(x.Timestamp), ct);
            
            // だいたい100msごとに値が発行されている
            Assert.AreEqual(3, results.Count);
            Assert.GreaterOrEqual(TimeProvider.System.GetElapsedTime(results[0], results[1]).TotalMilliseconds , 98.0);
            Assert.GreaterOrEqual(TimeProvider.System.GetElapsedTime(results[1], results[2]).TotalMilliseconds , 98.0);
            
            // キャンセルするとOnCompletedが発行される
            // 今回はTake(3)により完了済みなので意味はない
            cts.Cancel();
        }
    }
}
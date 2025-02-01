using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TimestampTest
    {
        [Test]
        public void R3_Timestamp_OnNextが到達したら現在時刻を発行する()
        {
            using var subject = new R3.Subject<int>();

            var result = default((long Timestamp, int Value));
            subject.Timestamp(TimeProvider.System).Subscribe(x => result = x);

            subject.OnNext(1);

            // Value
            Assert.AreEqual(1, result.Value);

            // 発行されたTimestamp
            var timestamp = result.Timestamp;
            // 現在のTimestamp
            var nowTimestamp = TimeProvider.System.GetTimestamp();

            // TimestampからTimeSpanへ変換
            var elapsed = TimeProvider.System.GetElapsedTime(timestamp, nowTimestamp);

            // だいたい数msくらいの差分となるはず
            Assert.LessOrEqual(elapsed.TotalMilliseconds, 10);
        }

        [Test]
        public void UniRx_Timestamp()
        {
            using var subject = new UniRx.Subject<int>();

            var result = default(Timestamped<int>);
            subject.Timestamp().Subscribe(x => result = x);

            subject.OnNext(1);

            // Value
            Assert.AreEqual(1, result.Value);

            // 発行されたTimestamp
            var timestamp = result.Timestamp;
            // 現在のTimestamp
            var nowTimestamp = DateTimeOffset.Now;

            // だいたい数msくらいの差分となるはず
            Assert.LessOrEqual((nowTimestamp - timestamp).TotalMilliseconds, 10);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class TimerTest
    {
        [Test]
        public async Task Timer_指定した時間後に1回発行する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var list = new List<Notification<Unit>>();

            // 100ms後にOnNextを発行し、その後OnCompletedを発行する
            await Observable
                .Timer(TimeSpan.FromMilliseconds(100), TimeProvider.System)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }

        [Test]
        public async Task Timer_指定した時間待ったあとに指定間隔で発行を繰り返す()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var list = new List<Notification<Unit>>();

            // 100ms後にOnNextを発行し、その後は50msごとにOnNextを発行する
            await Observable
                .Timer(
                    dueTime: TimeSpan.FromMilliseconds(100),
                    period: TimeSpan.FromMilliseconds(50),
                    TimeProvider.System)
                .Take(3) // 無限に続くので3回だけ取得して打ち止める
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(NotificationKind.OnNext, list[1].Kind);
            Assert.AreEqual(NotificationKind.OnNext, list[2].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
    }
}
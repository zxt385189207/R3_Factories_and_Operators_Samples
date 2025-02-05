using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UnityEngine;

namespace R3_UniRx.Tests.Factories
{
    public sealed class IntervalFrameTest : MonoBehaviour
    {
        [Test]
        public void IntervalFrame_一定フレーム間隔で値を発行する()
        {
            var fakeFrameProvider = new FakeFrameProvider();
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // 3フレームごとに値を発行するObservable
            using var list = Observable.IntervalFrame(3, fakeFrameProvider, ct)
                .Materialize()
                .ToLiveList();

            // まだ
            CollectionAssert.IsEmpty(list);

            // 2F
            fakeFrameProvider.Advance(2);

            // まだ
            CollectionAssert.IsEmpty(list);

            // 3F目
            fakeFrameProvider.Advance();

            // 1回目の値が発行される
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);

            // +3F
            fakeFrameProvider.Advance(3);

            // 2回目の値が発行される
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[1].Kind);

            // +3F
            fakeFrameProvider.Advance(3);

            // 3回目の値が発行される
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[2].Kind);
            
            // キャンセルするとOnCompletedが発行される
            cts.Cancel();
            
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);
        }
    }
}
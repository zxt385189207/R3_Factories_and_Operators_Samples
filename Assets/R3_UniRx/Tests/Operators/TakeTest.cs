using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TakeTest
    {
        [Test]
        public void R3_Take_購読開始から指定した個数だけ通過させる()
        {
            var subject = new R3.Subject<int>();

            // 購読開始から3つだけ通過
            var list = subject.Take(3).Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(3, list[2].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[3].Kind);
        }

        [Test]
        public async Task R3_Take_購読開始から指定した時間だけ通過させる()
        {
            var subject = new R3.Subject<int>();

            // 購読開始から1秒だけ通過
            var list = subject.Take(TimeSpan.FromSeconds(1), TimeProvider.System).Materialize().ToLiveList();

            subject.OnNext(1);
            await Task.Delay(500);

            subject.OnNext(2);
            await Task.Delay(1000); // 1秒over

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[2].Kind);
        }
        
        [Test]
        public void UniRx_Take_購読開始から指定した個数だけ通過させる()
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();
            
            // 購読開始から3つだけ通過
            subject.Take(3).Materialize().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(3, list[2].Value);
            Assert.AreEqual(UniRx.NotificationKind.OnCompleted, list[3].Kind);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class ReturnTest
    {
        [Test]
        public void Return_値を1つだけ発行する_即座に発行()
        {
            using var list = Observable.Return("test").Materialize().ToLiveList();

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("test", list[0].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
        }


        [Test]
        public async Task Return_値を1つだけ発行する_TimeProviderを指定()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var list = new List<Notification<string>>();

            await Observable
                .Return("test", TimeProvider.System, ct)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("test", list[0].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
        
        [Test]
        public async Task Return_値を1つだけ発行する_時間を指定()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var list = new List<Notification<string>>();

            // 100ms後にOnNextを発行し、その後OnCompletedを発行する
            await Observable
                .Return("test", TimeSpan.FromMilliseconds(100) , TimeProvider.System, ct)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("test", list[0].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
            
            // キャンセルするとOnCompletedが発行される
            // 今回は完了済みなので意味はない
            cts.Cancel();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class ThrowTest
    {
        [Test]
        public void Throw_失敗のOnCompletedを発行する_即座に発行()
        {
            using var list = Observable.Throw<string>(new Exception())
                .Materialize()
                .ToLiveList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            Assert.AreEqual(typeof(Exception), list[0].Error.GetType());
        }

        [Test]
        public async Task Throw_失敗のOnCompletedを発行する_TimeProviderを指定()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var list = new List<Notification<string>>();

            await Observable
                .Throw<string>(new Exception(), TimeProvider.System)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            Assert.AreEqual(typeof(Exception), list[0].Error.GetType());
        }

        [Test]
        public async Task Throw_失敗のOnCompletedを発行する_時間を指定()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var list = new List<Notification<string>>();

            // 100ms後にOnCompletedを発行する
            await Observable
                .Throw<string>(new Exception(), TimeSpan.FromMilliseconds(100), TimeProvider.System)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            Assert.AreEqual(typeof(Exception), list[0].Error.GetType());
        }
    }
}
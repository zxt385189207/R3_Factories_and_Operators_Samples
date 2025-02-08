using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class ReturnOnCompletedTest
    {
        [Test]
        public void Return_OnCompletedを発行する_成功_即座に発行()
        {
            using var list = Observable.ReturnOnCompleted<string>(Result.Success).Materialize().ToLiveList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            Assert.IsNull(list[0].Error);
        }

        [Test]
        public void Return_OnCompletedを発行する_異常終了_即座に発行()
        {
            using var list = Observable.ReturnOnCompleted<string>(Result.Failure(new Exception()))
                .Materialize()
                .ToLiveList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
            Assert.AreEqual(typeof(Exception), list[0].Error.GetType());
        }

        [Test]
        public async Task Return_OnCompletedを発行する_TimeProviderを指定()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var list = new List<Notification<string>>();

            await Observable
                .ReturnOnCompleted<string>(Result.Success, TimeProvider.System)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
        }

        [Test]
        public async Task Return_OnCompletedを発行する_時間を指定()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var list = new List<Notification<string>>();

            // 100ms後にOnCompletedを発行する
            await Observable
                .ReturnOnCompleted<string>(Result.Success, TimeSpan.FromMilliseconds(100), TimeProvider.System)
                .Materialize()
                .ForEachAsync(list.Add, cancellationToken: ct);

            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
        }
    }
}
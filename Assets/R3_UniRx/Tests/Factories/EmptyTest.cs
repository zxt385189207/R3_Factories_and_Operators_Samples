using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class EmptyTest
    {
        [Test]
        public void Empty_OnCompletedを即発行する()
        {
            var observable = Observable.Empty<int>();

            var list = observable.Materialize().ToLiveList();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[0].Kind);
        }
    }
}
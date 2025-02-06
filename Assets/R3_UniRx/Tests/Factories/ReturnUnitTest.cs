using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class ReturnUnitTest
    {
        [Test]
        public void ReturnUnit_値を1つだけ発行する()
        {
            using var list = Observable.ReturnUnit().Materialize().ToLiveList();

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(Unit.Default, list[0].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[1].Kind);
        }
    }
}
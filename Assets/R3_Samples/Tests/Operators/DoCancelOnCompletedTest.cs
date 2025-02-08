using System.Threading;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class DoCancelOnCompletedTest
    {
        [Test]
        public void R3_DoCancelOnCompleted_OnCompleted時にCancellationTokenSourceをキャンセルする()
        {
            using var subject = new R3.Subject<int>();
            using var cts = new CancellationTokenSource();

            subject.DoCancelOnCompleted(cts).Subscribe();

            // まだキャンセルされていない
            Assert.IsFalse(cts.IsCancellationRequested);

            subject.OnNext(1);

            // まだキャンセルされていない
            Assert.IsFalse(cts.IsCancellationRequested);

            subject.OnCompleted();

            // キャンセルされている
            Assert.IsTrue(cts.IsCancellationRequested);
        }

        [Test]
        public void UniRx_DoCancelOnCompletedをDoOnCompletedで再現する()
        {
            using var subject = new UniRx.Subject<int>();
            using var cts = new CancellationTokenSource();

            subject.DoOnCompleted(() => cts.Cancel()).Subscribe();

            Assert.IsFalse(cts.IsCancellationRequested);

            subject.OnNext(1);

            Assert.IsFalse(cts.IsCancellationRequested);

            subject.OnCompleted();

            Assert.IsTrue(cts.IsCancellationRequested);
        }
    }
}
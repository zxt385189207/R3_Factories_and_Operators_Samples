using System;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class OnErrorResumeAsFailureTest
    {
        [Test]
        public async Task R3_OnErrorResumeAsFailure_OnErrorResumeを異常終了に変換する()
        {
            var result = await Observable.Return("one")
                .Select(int.Parse) // OnErrorResumeが発生
                .OnErrorResumeAsFailure() // 異常終了に変換
                .Materialize()
                .FirstAsync();

            // OnCompletedになっている
            Assert.AreEqual(NotificationKind.OnCompleted, result.Kind);

            // 異常終了で例外が格納されている
            Assert.IsTrue(result.Error is FormatException);
        }

        [Test]
        public void UniRx_そもそもObservableの概念が異なるためOnErrorResumeAsFailureは存在しない()
        {
            Assert.Ignore();
        }
    }
}
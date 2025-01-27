using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ReplayFrameTest
    {
        [Test]
        public void R3_ReplayFrame_Observableを別のReplayFrameSubjectへ流し込む()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var parentSubject = new R3.Subject<int>();

            // 直近3Fフレーム分を保持するようにする
            var connectableObservable = parentSubject.ReplayFrame(3, fakeFrameProvider);

            // 接続前に発行する(まだ記録されない)
            parentSubject.OnNext(1);
            parentSubject.OnNext(2);
            parentSubject.OnNext(3);

            // Connectして接続する
            using var disposable = connectableObservable.Connect();

            // 接続後に発行する(記録される)
            parentSubject.OnNext(4);
            fakeFrameProvider.Advance();

            parentSubject.OnNext(5);
            fakeFrameProvider.Advance();

            parentSubject.OnNext(6); // 2つ前のフレーム
            fakeFrameProvider.Advance();

            parentSubject.OnNext(7); // 1つ前のフレーム
            fakeFrameProvider.Advance();

            parentSubject.OnNext(8); // 今のフレーム ここから3F分の値が記録されている

            // 遅れて購読
            var list = connectableObservable.ToLiveList();

            // 直近3Fの値が記録されている
            CollectionAssert.AreEqual(new[] { 6, 7, 8 }, list);
        }

        [Test]
        public void UniRx_ReplayFrameは存在しない()
        {
            Assert.Ignore();
        }
    }
}
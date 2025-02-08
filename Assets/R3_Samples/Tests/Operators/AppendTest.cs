using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public class AppendTest
    {
        [Test]
        public void R3_Append_OnCompleted発行時にObservableの最後に指定された値を挿入する()
        {
            using var subject = new R3.Subject<int>();

            // Appendで最後に100を追加する
            var liveList = subject.Append(100).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // Observableはまだ完了していないので100は追加されていない
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);

            // Observableが完了してAppendが発火する
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100 }, liveList);
        }

        [Test]
        public void R3_Append_OnCompleted発行時にObservableの最後に指定された値を複数挿入する()
        {
            using var subject = new R3.Subject<int>();

            // Appendで最後に100を追加する
            var liveList = subject.Append(new[] { 100, 200, 300 }).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // Observableはまだ完了していないので100は追加されていない
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);

            // Observableが完了してAppendが発火する
            subject.OnCompleted();

            // 追加されている
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100, 200, 300 }, liveList);
        }

        [Test]
        public void R3_Append_OnCompleted発行時にObservableの最後にその場で値を生成して発行する()
        {
            using var subject = new R3.Subject<int>();


            var liveList = subject.Append(valueFactory: () =>
                {
                    // このデリゲートはOnCompleted発行時に呼び出される
                    return 100;
                })
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // Observableはまだ完了していないので100は追加されていない
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);

            // Observableが完了してAppendが発火する
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100 }, liveList);
        }

        [Test]
        public void R3_Append_OnCompleted発行時にObservableの最後にその場で値を生成して発行するwithState()
        {
            using var subject = new R3.Subject<int>();

            // valueFactory内で利用する値を事前に登録できる
            // クロージャが生成されないためGCが避けられる
            var liveList = subject.Append(
                    state: 100,
                    valueFactory: state =>
                    {
                        // このデリゲートはOnCompleted発行時に呼び出される
                        return state + 1;
                    })
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // Observableはまだ完了していないので100は追加されていない
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);

            // Observableが完了してAppendが発火する
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 101 }, liveList);
        }


        [Test]
        public void UniRx_Appendは存在しない()
        {
            Assert.Ignore();
        }
    }
}
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class PrependTest
    {
        [Test]
        public void R3_Prepend_購読直後に指定された値を挿入する()
        {
            using var subject = new R3.Subject<int>();

            // Prependで先頭に100を追加する
            var liveList = subject.Prepend(100).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 100, 1, 2, 3, }, liveList);
        }

        [Test]
        public void R3_Prepend_購読直後に指定された値を複数挿入する()
        {
            using var subject = new R3.Subject<int>();

            // Prependで最後に100を追加する
            var liveList = subject.Prepend(new[] { 100, 200, 300 }).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
            // 追加されている
            CollectionAssert.AreEqual(new[] { 100, 200, 300, 1, 2, 3 }, liveList);
        }

        [Test]
        public void R3_Prepend_その場で値を生成して先頭に挿入する()
        {
            using var subject = new R3.Subject<int>();


            var liveList = subject.Prepend(valueFactory: () => 100)
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 100, 1, 2, 3 }, liveList);
        }

        [Test]
        public void R3_Prepend_その場で値を生成して先頭に挿入するwithState()
        {
            using var subject = new R3.Subject<int>();

            // valueFactory内で利用する値を事前に登録できる
            // クロージャが生成されないためGCが避けられる
            var liveList = subject.Prepend(
                    state: 100,
                    valueFactory: state => state + 1)
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 101, 1, 2, 3 }, liveList);
        }


        [Test]
        public async Task UniRx_StartWith()
        {
            var result = await UniRx.Observable.Range(1, 3)
                .StartWith(100)
                .ToArray()
                .ToTask();
            
            CollectionAssert.AreEqual(new[] { 100, 1, 2, 3 }, result);
        }
    }
}
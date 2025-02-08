using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class PublishTest
    {
        [Test]
        public void R3_Publish_元のObservableを購読して後続に流す()
        {
            // もとになるSubject
            using var subject = new R3.Subject<string>();

            // もとになるObservable、Indexでインデックスを付与する
            var parentObservable = subject.Index();

            // Publishで接続準備
            // ConnectableObservable<(int, string)>である
            ConnectableObservable<(int, string)> connectableObservable = parentObservable.Publish();

            // parentObservable購読開始
            using var disposable = connectableObservable.Connect();

            // もとのSubjectから値が発行されている
            subject.OnNext("a"); // 0
            subject.OnNext("b"); // 1
            subject.OnNext("c"); // 2

            // Publishを介した方のSubscribe
            var publishedSubscribe = -1;
            // 直接のSubscribe
            var rawSubscribe = -1;

            // Publishを介したSubscribe
            connectableObservable.Take(1).Subscribe(x => publishedSubscribe = x.Item1);
            // 直接のSubscribe
            parentObservable.Take(1).Subscribe(x => rawSubscribe = x.Item1);

            // 次の値が発行される
            subject.OnNext("d");

            // Publishを介した方はすでにObservableが稼働中なので、過去に値が発行された形跡がある
            Assert.AreEqual(3, publishedSubscribe);
            // 直接Subscribeした場合は、SubscribeタイミングでIndexが初期化されるため0から始まっている
            Assert.AreEqual(0, rawSubscribe);
        }

        [Test]
        public void R3_Publish_元のObservableを購読して初期値を設定しながら後続に流す()
        {
            // もとになるSubject
            using var subject = new R3.Subject<string>();

            // もとになるObservable
            var parentObservable = subject;

            // Publishで接続準備、初期値を指定した場合はMulticast(BehaviorSubject)と同義
            // ConnectableObservable<string>である
            ConnectableObservable<string> connectableObservable = parentObservable.Publish("init");

            // parentObservable購読開始
            using var disposable = connectableObservable.Connect();

            // 購読開始
            using var list = connectableObservable.ToLiveList();

            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnCompleted();

            // 初期値が発行されている
            CollectionAssert.AreEqual(new[] { "init", "a", "b", "c" }, list);
        }

        [Test]
        public void UniRx_Publish()
        {
            // もとになるSubject
            using var subject = new UniRx.Subject<string>();

            // もとになるObservable、Indexでインデックスを付与する
            var parentObservable = subject.Select((x, i) => (i, x));

            // Publishで接続準備
            // IConnectableObservable<(int, string)>である
            IConnectableObservable<(int, string)> connectableObservable = parentObservable.Publish();

            // parentObservable購読開始
            using var disposable = connectableObservable.Connect();

            // もとのSubjectから値が発行されている
            subject.OnNext("a"); // 0
            subject.OnNext("b"); // 1
            subject.OnNext("c"); // 2

            // Publishを介した方のSubscribe
            var publishedSubscribe = -1;
            // 直接のSubscribe
            var rawSubscribe = -1;

            // Publishを介したSubscribe
            connectableObservable.Take(1).Subscribe(x => publishedSubscribe = x.Item1);
            // 直接のSubscribe
            parentObservable.Take(1).Subscribe(x => rawSubscribe = x.Item1);

            // 次の値が発行される
            subject.OnNext("d");

            // Publishを介した方はすでにObservableが稼働中なので、過去に値が発行された形跡がある
            Assert.AreEqual(3, publishedSubscribe);
            // 直接Subscribeした場合は、SubscribeタイミングでIndexが初期化されるため0から始まっている
            Assert.AreEqual(0, rawSubscribe);
        }
    }
}
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class RefCountTest
    {
        [Test]
        public void R3_RefCount_ConnectableObservableの接続断を自動化する()
        {
            // もとになるSubject
            using var subject = new R3.Subject<string>();

            // もとになるObservable、Indexでインデックスを付与する
            var parentObservable = subject.Index();

            // Publish + RefCountで接続断を自動化
            var secondObservable = parentObservable.Publish().RefCount();

            // （まだ購読者ゼロ)
            subject.OnNext("a"); // Indexはまだ稼働していない

            var compositeDisposable = new R3.CompositeDisposable();

            // 購読者一人目
            using var list1 = secondObservable.ToLiveList();
            compositeDisposable.Add(list1);

            subject.OnNext("b"); // 0 

            // 購読者二人目
            using var list2 = secondObservable.ToLiveList();
            compositeDisposable.Add(list2);

            subject.OnNext("c"); // 1

            // 一人目はIndex = 0, 1が流れている
            CollectionAssert.AreEqual(new[]
            {
                (0, "b"),
                (1, "c")
            }, list1);

            // 二人目は途中参加だが、すでに稼働しているObservableにぶら下がっているため
            // Index = 1が流れている
            CollectionAssert.AreEqual(new[]
            {
                (1, "c")
            }, list2);

            // 購読者が全員いなくなった
            // この時点でPublishが切断
            compositeDisposable.Dispose();

            subject.OnNext("d"); // Indexは稼働していない

            // 新しい購読者が参加
            using var list3 = secondObservable.ToLiveList();

            subject.OnNext("e"); // 0　Indexが初期化されている

            CollectionAssert.AreEqual(new[]
            {
                (0, "e")
            }, list3);
        }

        [Test]
        public void UniRx_RefCount()
        {
            // もとになるSubject
            using var subject = new UniRx.Subject<string>();

            // もとになるObservable、Selectでインデックスを付与する
            var parentObservable = subject.Select((x, i) => (i, x));

            // Publish + RefCountで接続断を自動化
            var secondObservable = parentObservable.Publish().RefCount();

            // （まだ購読者ゼロ)
            subject.OnNext("a"); // Indexはまだ稼働していない

            var compositeDisposable = new UniRx.CompositeDisposable();

            // 購読者一人目
            var list1 = new List<(int, string)>();
            var d1 = secondObservable.Subscribe(list1.Add);
            compositeDisposable.Add(d1);

            subject.OnNext("b"); // 0 

            var list2 = new List<(int, string)>();
            var d2 = secondObservable.Subscribe(list2.Add);
            compositeDisposable.Add(d2);
            
            subject.OnNext("c"); // 1

            // 一人目はIndex = 0, 1が流れている
            CollectionAssert.AreEqual(new[]
            {
                (0, "b"),
                (1, "c")
            }, list1);

            // 二人目は途中参加だが、すでに稼働しているObservableにぶら下がっているため
            // Index = 1が流れている
            CollectionAssert.AreEqual(new[]
            {
                (1, "c")
            }, list2);

            // 購読者が全員いなくなった
            // この時点でPublishが切断
            compositeDisposable.Dispose();

            subject.OnNext("d"); // Indexは稼働していない

            // 新しい購読者が参加
            
            var list3 = new List<(int, string)>();
            secondObservable.Subscribe(list3.Add);

            subject.OnNext("e"); // 0　Indexが初期化されている

            CollectionAssert.AreEqual(new[]
            {
                (0, "e")
            }, list3);
        }
    }
}
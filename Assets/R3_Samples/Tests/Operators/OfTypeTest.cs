using System.Collections.Generic;
using NUnit.Framework;
using R3;
using R3.Collections;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class OfTypeTest
    {
        [Test]
        public void R3_OfType_OnNextの型をキャストする()
        {
            // object型が流れるSubject
            using var subject = new R3.Subject<object>();

            // int型にキャストする
            var castObservable = subject.OfType<object, int>();

            // 発行されたメッセージをすべて格納する
            LiveList<R3.Notification<int>> list = castObservable.Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("3"); // 無視される
            subject.OnNext(4);
            subject.OnCompleted();

            // 1, 2, 4, OnCompletedの4つが流れている
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(4, list[2].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[3].Kind);
        }


        [Test]
        public void UniRx_OfType()
        {
            // object型が流れるSubject
            using var subject = new UniRx.Subject<object>();

            // int型にキャストする
            var castObservable = subject.OfType<object, int>();

            var list = new List<UniRx.Notification<int>>();
            
            // 発行されたメッセージをすべて格納する
            castObservable.Materialize().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("3"); // 無視される
            subject.OnNext(4);
            subject.OnCompleted();

            // 1, 2, 4, OnCompletedの4つが流れている
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(4, list[2].Value);
            Assert.AreEqual(UniRx.NotificationKind.OnCompleted, list[3].Kind);
        }
    }
}
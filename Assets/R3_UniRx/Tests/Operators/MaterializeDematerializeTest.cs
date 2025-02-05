using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class MaterializeDematerializeTest
    {
        [Test]
        public void R3_Materialize_すべてのイベント種別をOnNextに変換する()
        {
            using var subject = new R3.Subject<int>();

            // LiveListはOnNextのみを記録する
            using var list = subject.Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnErrorResume(new Exception());
            subject.OnCompleted();

            Assert.AreEqual(4, list.Count);

            Assert.AreEqual(R3.NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(1, list[0].Value);

            Assert.AreEqual(R3.NotificationKind.OnNext, list[1].Kind);
            Assert.AreEqual(2, list[1].Value);

            Assert.AreEqual(R3.NotificationKind.OnErrorResume, list[2].Kind);
            Assert.AreEqual(typeof(Exception), list[2].Error.GetType());

            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[3].Kind);
        }

        [Test]
        public void R3_Dematerialize_MaterializeはDematerializeでもとに戻る()
        {
            using var subject = new R3.Subject<int>();

            var expectedList = subject.IgnoreOnErrorResume().ToLiveList();
            using var list = subject.Materialize().Dematerialize().IgnoreOnErrorResume().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnErrorResume(new Exception());
            subject.OnCompleted();

            Assert.AreEqual(expectedList.Count, list.Count);
            CollectionAssert.AreEqual(expectedList, list);
        }

        [Test]
        public void UniRx_MaterializeAndDematerialize()
        {
            // Materialize
            
            using var subject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();

            subject.Materialize().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();

            Assert.AreEqual(3, list.Count);

            Assert.AreEqual(UniRx.NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(1, list[0].Value);

            Assert.AreEqual(UniRx.NotificationKind.OnNext, list[1].Kind);
            Assert.AreEqual(2, list[1].Value);

            Assert.AreEqual(UniRx.NotificationKind.OnCompleted, list[2].Kind);
            
            // Dematerialize

            var list2 = new List<int>();
            
            list.ToObservable().Dematerialize().Subscribe(x=>list2.Add(x));
            
            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual(1, list2[0]);
            Assert.AreEqual(2, list2[1]);
                
        }
    }
}
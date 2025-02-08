using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using R3;
using UniRx;
using NotificationKind = UniRx.NotificationKind;

namespace R3_Samples.Tests.Operators
{
    public sealed class IgnoreElementsTest
    {
        [Test]
        public void R3_IgnoreElements_OnNextを無視する()
        {
            using var subject = new R3.Subject<int>();

            using var list = subject.IgnoreElements().Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnErrorResume(new Exception());
            subject.OnCompleted();

            // OnNextは無視される
            CollectionAssert.AreEqual(new[]
            {
                R3.NotificationKind.OnErrorResume,
                R3.NotificationKind.OnCompleted
            }, list.Select(x => x.Kind));
        }

        [Test]
        public void UniRx_IgnoreElements()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();

            subject.IgnoreElements().Materialize().Subscribe(x => list.Add(x));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnError(new Exception());

            // OnNextは無視される
            CollectionAssert.AreEqual(new[]
            {
                NotificationKind.OnError
            }, list.Select(x => x.Kind));
        }
    }
}
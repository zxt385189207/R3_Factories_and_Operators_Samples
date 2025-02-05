using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using R3;
using UniRx;
using NotificationKind = UniRx.NotificationKind;

namespace R3_UniRx.Tests.Operators
{
    public sealed class IgnoreOnErrorResumeTest
    {
        [Test]
        public void R3_IgnoreOnErrorResume_OnErrorResumeを無視する()
        {
            using var subject = new R3.Subject<int>();

            using var list = subject.IgnoreOnErrorResume().Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnErrorResume(new Exception());
            subject.OnCompleted();

            // OnErrorResumeは無視される
            CollectionAssert.AreEqual(new[]
            {
                R3.NotificationKind.OnNext,
                R3.NotificationKind.OnNext,
                R3.NotificationKind.OnCompleted
            }, list.Select(x => x.Kind));
        }

        [Test]
        public void UniRx_IgnoreOnErrorResumeは存在しないがCatchIgnoreが近い挙動をする()
        {
            using var subject = new UniRx.Subject<string>();

            var list = new List<UniRx.Notification<int>>();

            // UniRxはCatchIgnoreをOnErrorを無視する
            // OnErrorResumeとOnErrorは概念が違うため一概に同じものとは言えないが…
            subject
                .Select(int.Parse) // ここで例外が起きる可能性あり
                .CatchIgnore()
                .Materialize()
                .Subscribe(x => list.Add(x));

            subject.OnNext("1");
            subject.OnNext("2");
            subject.OnNext("three"); // ここで例外発生
            subject.OnCompleted();

            // OnErrorが無視される
            CollectionAssert.AreEqual(new[]
            {
                NotificationKind.OnNext,
                NotificationKind.OnNext,
                NotificationKind.OnCompleted
            }, list.Select(x => x.Kind));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class SkipTest
    {
        [Test]
        public void R3_Skip_購読開始から指定した個数分だけ無視する()
        {
            using var subject = new R3.Subject<int>();

            using var list = subject.Skip(2).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, list);
        }

        [Test]
        public async Task R3_Skip_購読開始から指定した時間だけ無視する()
        {
            // FakeTimeProviderを使いたいが…

            var observable = R3.Observable.Create<int>(async (observer, ct) =>
            {
                // 最初に1を流して、200ms待って2, 3, 4を流す
                observer.OnNext(1);
                await Task.Delay(200, ct);
                observer.OnNext(2);
                observer.OnNext(3);
                observer.OnNext(4);
                observer.OnCompleted();
            });

            // 購読開始から100ms以内の値を無視
            var results = observable.Skip(TimeSpan.FromMilliseconds(100), TimeProvider.System).ToLiveList();

            await Task.Delay(500);

            CollectionAssert.AreEqual(new[]
            {
                2,
                3,
                4
            }, results);
        }

        [Test]
        public void UniRx_Skip()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            subject.Skip(2).Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, list);
        }
    }
}
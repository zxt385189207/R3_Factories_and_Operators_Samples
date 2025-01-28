using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SkipLastTest
    {
        [Test]
        public void R3_SkipLast_最後から指定した個数分だけ無視する()
        {
            using var subject = new R3.Subject<int>();

            var list = subject.SkipLast(2).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            CollectionAssert.AreEqual(new[]
            {
                1,
                2
            }, list);
        }

        [Test]
        public async Task R3_SkipLast_最後から指定した時間だけ無視する()
        {
            // FakeTimeProviderを使いたいが…

            var observable = R3.Observable.Create<int>(async (observer, ct) =>
            {
                // 1, 2, 3を流して500ms待って4を流す
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                await Task.Delay(500, ct);
                observer.OnNext(4);
                observer.OnCompleted();
            });

            // 最後から100ms以内の値を無視
            var result = await observable.SkipLast(TimeSpan.FromMilliseconds(100), TimeProvider.System).ToArrayAsync();

            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                3
            }, result);
        }

        [Test]
        public void UniRx_SkipLastに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ThrottleFirstTest
    {
        [Test]
        public async Task R3_ThrottleFirst_OnNextが到達したら次以降のOnNextを一定時間無視する_時間による解除()
        {
            using var subject = new R3.Subject<int>();

            // 一度OnNextが到達したら次以降のOnNextを300ms間無視する
            var results = subject.ThrottleFirst(TimeSpan.FromMilliseconds(300), TimeProvider.System).ToLiveList();

            subject.OnNext(1); // pass

            await Task.Delay(100);

            subject.OnNext(2); // ignore

            await Task.Delay(100);

            subject.OnNext(3); // ignore

            await Task.Delay(200);

            subject.OnNext(4); // pass

            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, results);
        }

        [Test]
        public void R3_ThrottleFirst_OnNextが到達したら次以降のOnNextを一定時間無視する_別のObservableによる解除()
        {
            using var subject = new R3.Subject<int>();

            using var sampler = new R3.Subject<R3.Unit>();

            var results = subject.ThrottleFirst(sampler).ToLiveList();

            subject.OnNext(1); // pass
            subject.OnNext(2); // ignore
            subject.OnNext(3); // ignore
            sampler.OnNext(R3.Unit.Default); // 解除
            subject.OnNext(4); // pass
            subject.OnNext(5); // ignore

            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, results);
        }

        [Test]
        public void R3_ThrottleFirst_OnNextが到達したら非同期処理を実行しその実行中はOnNextを無視する()
        {
            using var subject = new R3.Subject<int>();

            var tcs = new UniTaskCompletionSource();
            var task = tcs.Task;

            var calledList = new List<int>();

            // 一度OnNextが到達したら非同期処理を実行し、その実行中はOnNextを無視する
            var results = subject.ThrottleFirst(async (x, ct) =>
                {
                    calledList.Add(x);
                    await task;
                })
                .ToLiveList();

            subject.OnNext(1); // pass
            subject.OnNext(2); // ignore
            subject.OnNext(3); // ignore
            tcs.TrySetResult(); // 解除
            subject.OnNext(4); // pass

            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, results);

            // 無視状態が解除されている状態で最初に発行されたOnNextのみが非同期処理に渡される
            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, calledList);
        }
        
        [Test]
        public async Task UniRx_ThrottleFirst()
        {
            using var subject = new UniRx.Subject<int>();

            var results = new List<int>();
            
            // 一度OnNextが到達したら次以降のOnNextを300ms間無視する
            subject.ThrottleFirst(TimeSpan.FromMilliseconds(300)).Subscribe(results.Add);

            subject.OnNext(1); // pass

            await Task.Delay(100);

            subject.OnNext(2); // ignore

            await Task.Delay(100);

            subject.OnNext(3); // ignore

            await Task.Delay(200);

            subject.OnNext(4); // pass

            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, results);
        }
    }
}
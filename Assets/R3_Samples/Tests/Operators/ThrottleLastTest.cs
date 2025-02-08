using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class ThrottleLastTest
    {
        [Test]
        public async Task R3_ThrottleLast_OnNextが到達したら一定時間遮断したのちに最後を発行()
        {
            using var subject = new R3.Subject<int>();

            // 一度OnNextが到達したらOnNextを300ms間無視して、最後に発行されたOnNextを発行する
            var results = subject.ThrottleLast(TimeSpan.FromMilliseconds(300), TimeProvider.System).ToLiveList();

            subject.OnNext(1); // 遮断開始

            await Task.Delay(100);

            subject.OnNext(2); // まだ遮断中

            await Task.Delay(100);

            subject.OnNext(3); // まだ遮断中、だが最後に発行されたOnNextなので後で通過

            await Task.Delay(200); // このあたりで遮断解除

            subject.OnNext(4); // 遮断開始

            CollectionAssert.AreEqual(new[]
            {
                3
            }, results);
        }

        [Test]
        public void R3_ThrottleLast_OnNextが到達したら一定時間遮断したのちに最後を発行_別のObservableによる解除()
        {
            using var subject = new R3.Subject<int>();

            using var sampler = new R3.Subject<R3.Unit>();

            var results = subject.ThrottleLast(sampler).ToLiveList();

            subject.OnNext(1); // ignore
            subject.OnNext(2); // ignore
            subject.OnNext(3); // pass
            sampler.OnNext(R3.Unit.Default); // 解除
            subject.OnNext(4); // ignore
            subject.OnNext(5); // ignore

            CollectionAssert.AreEqual(new[]
            {
                3
            }, results);
        }

        [Test]
        public void R3_ThrottleLast_OnNextが到達したら非同期処理を実行しその実行中はすべてのOnNextを無視して完了時に最後の1つを発行()
        {
            using var subject = new R3.Subject<int>();

            var tcs = new UniTaskCompletionSource();
            var task = tcs.Task;

            var calledList = new List<int>();

            // 一度OnNextが到達したら非同期処理を実行し、その実行中はOnNextをすべて無視、完了時に最後の1つを発行
            var results = subject.ThrottleLast(async (x, ct) =>
                {
                    calledList.Add(x);
                    await task;
                })
                .ToLiveList();

            subject.OnNext(1); // ignore
            subject.OnNext(2); // ignore
            subject.OnNext(3); // 完了時の最後の1つなのでこれが発行される
            tcs.TrySetResult(); // 解除
            
            // 新しいTaskを生成
            tcs = new UniTaskCompletionSource();
            task = tcs.Task;
            
            subject.OnNext(4); // ignore
            
            CollectionAssert.AreEqual(new[]
            {
                3
            }, results);


            // 無視状態が解除されている状態で最初に発行されたOnNextのみが非同期処理に渡される
            CollectionAssert.AreEqual(new[]
            {
                1,
                4
            }, calledList);
        }

        [Test]
        public async Task UniRx_Sample()
        {
            // UniRxではSampleが同じ意味を持つOperator
            
            using var subject = new UniRx.Subject<int>();
            
            var results = new List<int>();
            
            // 一度OnNextが到達したら次以降のOnNextを300ms間無視する
            subject.Sample(TimeSpan.FromMilliseconds(300)).Subscribe(results.Add);

            subject.OnNext(1); // 遮断開始

            await Task.Delay(100);

            subject.OnNext(2); // まだ遮断中

            await Task.Delay(100);

            subject.OnNext(3); // まだ遮断中、だが最後に発行されたOnNextなので後で通過

            await Task.Delay(200); // このあたりで遮断解除

            subject.OnNext(4); // 遮断開始

            CollectionAssert.AreEqual(new[]
            {
                3
            }, results);
        }
    }
}
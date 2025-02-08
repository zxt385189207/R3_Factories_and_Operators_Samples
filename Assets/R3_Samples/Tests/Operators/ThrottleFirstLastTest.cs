using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class ThrottleFirstLastTest
    {
        [Test]
        public async Task R3_ThrottleFirstLast_OnNextが到達したらそれを通し一定時間OnNextを遮断した後に最後に発行されたOnNextを1つ発行する()
        {
            using var subject = new R3.Subject<int>();

            // 一度OnNextが到達したら300ms間の遮断期間
            var results = subject.ThrottleFirstLast(TimeSpan.FromMilliseconds(300), TimeProvider.System).ToLiveList();

            subject.OnNext(1); // 通過

            CollectionAssert.AreEqual(new[]
            {
                1
            }, results);

            await Task.Delay(100);

            //---

            subject.OnNext(2); // 遮断中

            CollectionAssert.AreEqual(new[]
            {
                1
            }, results);

            await Task.Delay(100);

            //---

            subject.OnNext(3); // 遮断中だが、時間切れの際に最後に発行されたOnNextなので後で遅れて通過

            CollectionAssert.AreEqual(new[]
            {
                1
            }, results);


            await Task.Delay(200); // 時間切れで最後に発行されたOnNextが通過

            //---

            CollectionAssert.AreEqual(new[]
            {
                1,
                3
            }, results);

            subject.OnNext(4); // 通過して次の遮断期間に入る

            CollectionAssert.AreEqual(new[]
            {
                1,
                3,
                4
            }, results);

            await Task.Delay(500); // 遮断中

            //---

            subject.OnNext(5); // 通過

            CollectionAssert.AreEqual(new[]
            {
                1,
                3,
                4,
                5
            }, results);
        }

        [Test]
        public void R3_ThrottleFirstLast_OnNextが到達したらそれを通した後にOnNextを遮断_別のObservableにより解除されたら最後のOnNextを１つ発行する()
        {
            using var subject = new R3.Subject<int>();

            using var sampler = new R3.Subject<R3.Unit>();

            var results = subject.ThrottleFirstLast(sampler).ToLiveList();

            subject.OnNext(1); // 通過
            subject.OnNext(2); // 無視
            subject.OnNext(3); // 無視、だが最後に発行されたOnNextなので後で改めて発行される
            sampler.OnNext(R3.Unit.Default); // 解除
            subject.OnNext(4); // pass
            subject.OnNext(5); // ignore

            CollectionAssert.AreEqual(new[]
            {
                1,
                3,
                4
            }, results);
        }
        
        [Test]
        public void R3_ThrottleFirstLast_OnNextが到達したら非同期処理を実行しその実行中はOnNextを無視する_解除されたら最後のOnNextを１つ発行する()
        {
            using var subject = new R3.Subject<int>();

            var tcs = new UniTaskCompletionSource();
            var task = tcs.Task;

            var calledList = new List<int>();

            // 一度OnNextが到達したら非同期処理を実行し、その実行中はOnNextを無視する
            var results = subject.ThrottleFirstLast(async (x, ct) =>
                {
                    calledList.Add(x);
                    await task;
                })
                .ToLiveList();

            subject.OnNext(1); // 通過
            subject.OnNext(2); // 無視
            subject.OnNext(3); // 無視、だが最後に発行されたOnNextなので後で改めて発行される
            tcs.TrySetResult(); // 解除
            subject.OnNext(4); // 通過

            CollectionAssert.AreEqual(new[]
            {
                1,
                3,
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
        public void UniRx_ThrottleFirstLastに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
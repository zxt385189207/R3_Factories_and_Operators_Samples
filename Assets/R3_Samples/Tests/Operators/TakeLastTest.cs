using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class TakeLastTest
    {
        [Test]
        public void R3_TakeLast_OnCompleted発行時に最後から指定した個数だけ通過させる()
        {
            var subject = new R3.Subject<int>();

            // 最後の2つだけ通過
            using var list = subject.TakeLast(2).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            // まだOnCompletedしていないので通過しない
            CollectionAssert.IsEmpty(list);

            subject.OnCompleted();

            // 最後の2つだけ発行されている
            CollectionAssert.AreEqual(new[] { 3, 4 }, list);
        }

        [Test]
        public async Task R3_TakeLast_OnCompleted発行時に最後から指定した時間以内だけ通過させる()
        {
            var subject = new R3.Subject<int>();

            // 完了時に最後から400ms以内だけ通過
            using var list = subject.TakeLast(TimeSpan.FromMilliseconds(400), TimeProvider.System).ToLiveList();

            subject.OnNext(1); // -900 ms
            await Task.Delay(300);

            subject.OnNext(2); // -600 ms
            await Task.Delay(300);

            subject.OnNext(3); // -300 ms
            await Task.Delay(300);

            subject.OnNext(4); // -0 ms

            // まだOnCompletedしていないので通過しない
            CollectionAssert.IsEmpty(list);

            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 3, 4 }, list);
        }

        [Test]
        public void UniRx_TakeLast_OnCompleted発行時に最後から指定した個数だけ通過させる()
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<int>();

            // 最後の2つだけ通過
            subject.TakeLast(2).Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);

            // まだOnCompletedしていないので通過しない
            CollectionAssert.IsEmpty(list);

            subject.OnCompleted();

            // 最後の2つだけ発行されている
            CollectionAssert.AreEqual(new[] { 3, 4 }, list);
        }
    }
}
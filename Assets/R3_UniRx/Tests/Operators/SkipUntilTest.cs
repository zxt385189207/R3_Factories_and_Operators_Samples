using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SkipUntilTest
    {
        [Test]
        public void R3_SkipUntil_Taskが完了するまでの間はOnNextを無視しつづける()
        {
            using var subject = new R3.Subject<int>();

            var taskCompletionSource = new TaskCompletionSource<int>();
            var task = taskCompletionSource.Task;

            var results = subject.SkipUntil(task).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            taskCompletionSource.TrySetResult(0); // 完了
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, results);
        }

        [Test]
        public void R3_SkipUntil_非同期処理が完了するまでの間はOnNextを無視しつづける()
        {
            using var subject = new R3.Subject<int>();

            var taskCompletionSource = new TaskCompletionSource<int>();
            var task = taskCompletionSource.Task;

            // 呼び出された数値を記録
            var calledList = new List<int>();

            // 非同期処理が完了するまでの間はOnNextを無視しつづける
            // 非同期処理は最初に到達したOnNextの値を用いて1つだけ実行される
            var results = subject.SkipUntil(async (x, ct) =>
                {
                    calledList.Add(x);
                    await task;
                })
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            taskCompletionSource.TrySetResult(0); // 完了
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, results);

            CollectionAssert.AreEqual(new[]
            {
                1,
            }, calledList);
        }

        [Test]
        public void R3_SkipUntil_CancellationTokenがキャンセルされるまでOnNextを無視しつづける()
        {
            using var subject = new R3.Subject<int>();

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var results = subject.SkipUntil(cancellationToken).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            cancellationTokenSource.Cancel(); // キャンセル
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, results);
        }

        [Test]
        public void R3_SkipUntil_他のObservableのOnNextが発行されるまではOnNextを無視する()
        {
            using var subject = new R3.Subject<int>();

            using var otherSubject = new R3.Subject<int>();

            var results = subject.SkipUntil(otherSubject).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            otherSubject.OnNext(0); // 発行
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, results);
        }

        [Test]
        public void UniRx_SkipUntil()
        {
            using var subject = new UniRx.Subject<int>();

            using var otherSubject = new UniRx.Subject<int>();

            var list = new List<int>();

           subject.SkipUntil(otherSubject).Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            otherSubject.OnNext(0); // 発行
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, list);
        }
    }
}
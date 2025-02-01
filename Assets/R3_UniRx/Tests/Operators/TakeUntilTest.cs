using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TakeUntilTest
    {
        [Test]
        public void R3_TakeUntil_Taskが完了したらOnCompletedを発行する()
        {
            using var subject = new R3.Subject<int>();

            var taskCompletionSource = new TaskCompletionSource<int>();
            var task = taskCompletionSource.Task;

            var results = subject.TakeUntil(task).Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            taskCompletionSource.TrySetResult(0); // 完了

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);
        }

        [Test]
        public void R3_TakeUntil_非同期処理が完了したらOnCompletedを発行する()
        {
            using var subject = new R3.Subject<int>();

            var taskCompletionSource = new TaskCompletionSource<int>();
            var task = taskCompletionSource.Task;

            // 呼び出された数値を記録
            var calledList = new List<int>();

            // 非同期処理が完了するまでの間はOnNextを通過させる
            // 非同期処理は最初に到達したOnNextの値を用いて1つだけ実行される
            // 非同期処理は実行中であってもOnNextは通過する
            var results = subject.TakeUntil(async (x, ct) =>
                {
                    calledList.Add(x);
                    await task;
                })
                .Materialize()
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            taskCompletionSource.TrySetResult(0); // 完了

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);

            // 実行された非同期処理は[1]のみ
            CollectionAssert.AreEqual(new[]
            {
                1,
            }, calledList);
        }

        [Test]
        public void R3_TakeUntil_CancellationTokenがキャンセルされたらOnCompletedを発行する()
        {
            using var subject = new R3.Subject<int>();

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var results = subject.TakeUntil(cancellationToken).Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            cancellationTokenSource.Cancel(); // キャンセル
            
            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);
        }

        [Test]
        public void R3_TakeUntil_他のObservableのOnNextが発行されたらOnCompletedを発行する()
        {
            using var subject = new R3.Subject<int>();

            using var otherSubject = new R3.Subject<int>();

            var results = subject.TakeUntil(otherSubject).Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            otherSubject.OnNext(0); // 発行

            Assert.AreEqual(1, results[0].Value);
            Assert.AreEqual(2, results[1].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, results[2].Kind);
        }

        [Test]
        public void UniRx_TakeUntil()
        {
            using var subject = new UniRx.Subject<int>();

            using var otherSubject = new UniRx.Subject<int>();

            var list = new List<UniRx.Notification<int>>();

            subject.TakeUntil(otherSubject).Materialize().Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            otherSubject.OnNext(0); // 発行

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            Assert.AreEqual(UniRx.NotificationKind.OnCompleted, list[2].Kind);
        }
    }
}
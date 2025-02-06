using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public class ToObservableTest
    {
        [Test]
        public void ToObservable_Observableに変換する()
        {
            // Task
            var task = CreateTask();
            using var taskResults = task.ToObservable().Materialize().ToLiveList();

            Assert.AreEqual(2, taskResults.Count);
            Assert.AreEqual(NotificationKind.OnNext, taskResults[0].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, taskResults[1].Kind);

            // ValueTask
            var valueTask = CreateValueTask();
            using var valueTaskResults = valueTask.ToObservable().Materialize().ToLiveList();

            Assert.AreEqual(2, valueTaskResults.Count);
            Assert.AreEqual(NotificationKind.OnNext, valueTaskResults[0].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, valueTaskResults[1].Kind);

            // IEnumerable
            var enumerable = CreateEnumerable();
            using var enumerableResults = enumerable.ToObservable().Materialize().ToLiveList();

            Assert.AreEqual(4, enumerableResults.Count);
            Assert.AreEqual(1, enumerableResults[0].Value);
            Assert.AreEqual(2, enumerableResults[1].Value);
            Assert.AreEqual(3, enumerableResults[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, enumerableResults[3].Kind);

            // IAsyncEnumerable
            var asyncEnumerable = CreateAsyncEnumerable();
            using var asyncEnumerableResults = asyncEnumerable.ToObservable().Materialize().ToLiveList();

            Assert.AreEqual(4, asyncEnumerableResults.Count);
            Assert.AreEqual(1, asyncEnumerableResults[0].Value);
            Assert.AreEqual(2, asyncEnumerableResults[1].Value);
            Assert.AreEqual(3, asyncEnumerableResults[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, asyncEnumerableResults[3].Kind);
        }

        [Test]
        public void ToObservable_IObservableからR3のObservableへ変換する()
        {
            // ToObservableでSystem.IObservableからR3.IObservableに変換できる
            using var list =
                UniRx.Observable.Range(0, 3) // System.IObservable
                    .ToObservable() // System.IObservable -> R3.IObservable
                    .Materialize()
                    .ToLiveList();

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(0, list[0].Value);
            Assert.AreEqual(1, list[1].Value);
            Assert.AreEqual(2, list[2].Value);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);

            
            // OnErrorはR3のOnCompleted(Failure)に変換される
            using var list2 =
                UniRx.Observable.Throw<int>(new Exception())
                    .ToObservable()
                    .Materialize()
                    .ToLiveList();

            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list2[0].Kind);
            Assert.AreEqual(typeof(Exception), list2[0].Error.GetType());
        }

        private Task CreateTask()
        {
            return Task.CompletedTask;
        }

        private ValueTask CreateValueTask()
        {
            return new ValueTask(Task.CompletedTask);
        }

        private IEnumerable<int> CreateEnumerable()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        private async IAsyncEnumerable<int> CreateAsyncEnumerable()
        {
            yield return 1;
            await Task.CompletedTask;
            yield return 2;
            await Task.CompletedTask;
            yield return 3;
        }
    }
}
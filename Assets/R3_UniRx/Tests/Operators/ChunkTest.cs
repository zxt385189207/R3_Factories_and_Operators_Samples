using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;


namespace R3_UniRx.Tests.Operators
{
    public class ChunkTest
    {
        [Test]
        public void R3_Chunk_countを指定して値をまとめる()
        {
            // 2つずつまとめる
            var observable = R3.Observable.Range(1, 5).Chunk(count: 2);

            var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 3, 4 },
                new[] { 5 }
            }, list.ToArray());
        }


        [Test]
        public void R3_Chunk_countとskipを指定して値をまとめる()
        {
            // 1個飛ばしで2つずつまとめる
            // ≒ 現在値と一つ前の値をセットにする
            var observable = R3.Observable.Range(1, 5).Chunk(count: 2, skip: 1);

            var list = observable.ToLiveList();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 3, 4 },
                new[] { 4, 5 },
                new[] { 5 }
            }, list.ToArray());
        }

        [Test]
        public async Task R3_Chunk_時間を指定して値をまとめる()
        {
            using var subject = new R3.Subject<int>();

            // 100msごとにまとめる
            var observable = subject.Chunk(timeSpan: TimeSpan.FromMilliseconds(100), timeProvider: TimeProvider.System);
            var list = observable.ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            subject.OnNext(3);
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            subject.OnNext(4);
            subject.OnNext(5);
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            subject.OnNext(6);
            subject.OnCompleted();


            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 3 },
                new[] { 4, 5 },
                new[] { 6 }
            }, list.ToArray());
        }


        [Test]
        public void R3_Chunk_他のObservableのOnNextが発行されるまでまとめる()
        {
            using var baseSubject = new R3.Subject<int>();
            using var triggerSubject = new R3.Subject<R3.Unit>();

            var observable = baseSubject.Chunk(triggerSubject);
            var list = observable.ToLiveList();

            baseSubject.OnNext(1);
            baseSubject.OnNext(2);
            triggerSubject.OnNext(R3.Unit.Default);

            baseSubject.OnNext(3);
            triggerSubject.OnNext(R3.Unit.Default);

            baseSubject.OnNext(4);
            baseSubject.OnNext(5);
            triggerSubject.OnNext(R3.Unit.Default);

            baseSubject.OnNext(6);
            baseSubject.OnCompleted();


            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 3 },
                new[] { 4, 5 },
                new[] { 6 }
            }, list.ToArray());
        }


        [Test]
        public async Task R3_Chunk_値が発行されたら非同期処理を実行し完了するまでまとめる()
        {
            using var subject = new R3.Subject<int>();

            // 発行された値をもとに非同期処理を実行し、それが完了したら途中に発行された値をまとめて発行する
            // 非同期処理が完了後は次に値が発行されるまで待機する
            var observable = subject.Chunk(async (value, ct) =>
            {
                // 今回は100ms固定で待機する(valueは使わない）
                await Task.Delay(TimeSpan.FromMilliseconds(100), ct);
            });
            var list = observable.ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            subject.OnNext(3);
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            subject.OnNext(4);
            subject.OnNext(5);
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            subject.OnNext(6);
            subject.OnCompleted();


            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 3 },
                new[] { 4, 5 },
                new[] { 6 }
            }, list.ToArray());
        }


        [Test]
        public async Task UniRx_Buffer()
        {
            // UniRxではBuffer
            var result =
                await UniRx.Observable.Range(1, 5)
                    .Buffer(count: 2, skip: 1)
                    .ToArray()
                    .ToTask();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 3, 4 },
                new[] { 4, 5 },
                new[] { 5 }
            }, result.ToArray());
        }
    }
}
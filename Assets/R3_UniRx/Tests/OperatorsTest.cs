using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using R3.Collections;
using UniRx;
using UnityEngine.TestTools;
using Observable = UniRx.Observable;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;


public sealed class OperatorsTest
{
    [Test]
    public async Task AggregateAsyncTest()
    {
        // OnCompleted発行時に、過去に発行された値を畳み込み(fold)計算する。

        // R3
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AggregateAsync((p, c) => p + c);

            Assert.AreEqual(15, result);
        }

        // UniRx
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRxObservable.ToObservable(array)
                .Aggregate((p, c) => p + c)
                .ToTask();

            Assert.AreEqual(15, result);
        }
    }

    [Test]
    public async Task AggregateByAsyncTest()
    {
        // OnCompleted発行時に、過去に発行された値をkeySelectorでグループ化し、グループ単位で畳み込み(fold)計算する。

        // R3
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AggregateByAsync(
                    // 偶数グループと奇数グループに分けてそれぞれ合計値を計算
                    keySelector: key => key % 2,
                    seed: 0,
                    func: (total, cur) => total + cur);

            var actual = result.ToArray();

            Assert.AreEqual(9, actual[0].Value); // 偶数グループの合計値
            Assert.AreEqual(6, actual[1].Value); // 奇数グループの合計値
        }

        // UniRx
        {
            // Nothing
        }
    }

    [Test]
    public async Task AllAsyncTest()
    {
        // OnCompleted発行時に、過去に発行されたすべての値が条件を満たしているかを調べる。

        // R3
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            // ぜんぶ正の値か? => true
            var result1 = await R3Observable.ToObservable(array)
                .AllAsync(x => x > 0);
            Assert.IsTrue(result1);

            // 3を含んでいないか? => false
            var result2 = await R3Observable.ToObservable(array)
                .AllAsync(x => x != 3);
            Assert.IsFalse(result2);
        }

        // UniRx
        // Allは存在しないのでToArray()してLINQのAllで代用
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            // ぜんぶ正の値か? => true
            var result1 = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();
            Assert.IsTrue(result1.All(x => x > 0));

            // 3を含んでいないか? => false
            var result2 = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();
            Assert.IsFalse(result2.All(x => x != 3));
        }
    }

    [Test]
    public async Task AmbTest()
    {
        // 2つのObservableを同時に購読し、値が先着した方のObservableのみを採択する。

        // R3
        {
            var o1 = R3Observable.Create<int>(async (observer, ct) =>
            {
                // 100ms待ってから値を発行
                await Task.Delay(100, ct);

                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                observer.OnCompleted();
            });

            var o2 = R3Observable.Create<int>(async (observer, _) =>
            {
                // 即座に発行
                observer.OnNext(4);
                observer.OnNext(5);
                observer.OnNext(6);
                await Task.Yield();
                observer.OnCompleted();
            });

            var result = await R3Observable.Amb(o1, o2).ToArrayAsync();

            // o2が先着しているのでo2の値が採択される
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, result);
        }

        // UniRx
        {
            // 1,2,3  1フレームごとに発行
            var o1 = UniRx.Observable.Range(1, 3, Scheduler.MainThread);
            // 4,5,6  即時
            var o2 = UniRx.Observable.Range(4, 3, Scheduler.Immediate);

            var result = await o1.Amb(o2).ToArray().ToTask();

            // o2が先着しているのでo2の値が採択される
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, result);
        }
    }

    [Test]
    public async Task AnyAsyncTest()
    {
        // 条件を満たしたOnNextが発行されたら即座に完了するTask<bool>に変換する。
        // ContainsAsyncとの違いはFunc<T, bool>を引数に取る。

        // R3
        {
            var subject = new R3.Subject<int>();

            // 3が発行されたか？
            var task = subject.AnyAsync(x => x == 3);

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }

        // UniRx
        // Anyは存在しないのでFirstOrDefaultで代用
        {
            var subject = new UniRx.Subject<int>();

            // 3が発行されたか？
            var task = subject.FirstOrDefault(x => x == 3).Select(_ => true).DefaultIfEmpty(false).ToTask();

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }
    }

    [Test]
    public void AppendTest()
    {
        // OnCompleted発行時に、Observableの最後に指定された値を挿入する。

        // R3
        {
            var subject = new R3.Subject<int>();

            // Appendで最後に100を追加する
            var liveList = subject.Append(100).ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // Observableはまだ完了していないので100は追加されていない
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, liveList);

            // Observableが完了してAppendが発火する
            subject.OnCompleted();

            // 100が追加されている
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100 }, liveList);
        }
    }

    [Test]
    public void AsObservableTest()
    {
        // Observable型に変換する。ソースとなっている型（SubjectやReactivePropertyなど）へのダウンャストを防止できる。

        // R3
        {
            var subject = new R3.Subject<int>();
            var castObservable = (Observable<int>)subject;
            var convertedObservable = castObservable.AsObservable();

            // ただのキャストではSubjectとして扱えてしまう
            Assert.IsInstanceOf<R3.Subject<int>>(castObservable);

            // AsObservableするとSubjectではなくObservable型になる
            Assert.IsNotInstanceOf<R3.Subject<int>>(convertedObservable);
            Assert.IsInstanceOf<R3.Observable<int>>(convertedObservable);
        }

        // UniRx
        {
            var subject = new UniRx.Subject<int>();
            var castObservable = (IObservable<int>)subject;
            var convertedObservable = castObservable.AsObservable();

            // ただのキャストではSubjectとして扱えてしまう
            Assert.IsInstanceOf<UniRx.Subject<int>>(castObservable);

            // AsObservableするとSubjectではなくObservable型になる
            Assert.IsNotInstanceOf<UniRx.Subject<int>>(convertedObservable);
            Assert.IsInstanceOf<IObservable<int>>(convertedObservable);
        }
    }

    [Test]
    public void AsSystemObservableTest()
    {
        // R3のObservable<T>からSystem.IObservable<T>へと変換する。

        // R3
        var observable = R3Observable.Range(1, 3);

        // R3 -> System.IObservable(UniRx)
        var systemObservable = observable.AsSystemObservable();
        Assert.IsInstanceOf<IObservable<int>>(systemObservable);
    }

    [Test]
    public async Task AsUnitObservableTest()
    {
        // Observable<Unit>に変換する。
        {
            // R3
            {
                var observable = R3Observable.Range(1, 3);

                // Unitに変換
                var result = await observable.AsUnitObservable().ToArrayAsync();

                CollectionAssert.AreEqual(new[] { R3.Unit.Default, R3.Unit.Default, R3.Unit.Default }, result);
            }

            // UniRx
            {
                var observable = UniRxObservable.Range(1, 3);

                // Unitに変換
                var result = await observable.AsUnitObservable().ToArray().ToTask();

                CollectionAssert.AreEqual(new[] { UniRx.Unit.Default, UniRx.Unit.Default, UniRx.Unit.Default }, result);
            }
        }
    }

    [Test]
    public async Task AverageAsyncTest()
    {
        // OnCompleted発行時に、購読中に発行された値の平均値を求める。

        // R3
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .AverageAsync();

            Assert.AreEqual(3.0, result);
        }

        // UniRx
        // Averageは存在しないのでToArray()してLINQのAverageで代用

        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();

            Assert.AreEqual(3.0, result.Average());
        }
    }

    [Test]
    public async Task CastTest()
    {
        // OnNextの型を指定した型にキャストする。変換できない場合はOnErrorResumeが発行される。

        // R3{
        {
            // object型が流れるSubject
            var subject = new R3.Subject<object>();

            // int型にキャストする
            var castObservable = subject.Cast<object, int>();

            // 発行されたメッセージをすべて格納する
            LiveList<R3.Notification<int>> list = castObservable.Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("3");
            subject.OnNext(4);
            subject.OnCompleted();

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            // キャストできない"3"はOnErrorResumeが発行されている
            Assert.AreEqual(R3.NotificationKind.OnErrorResume, list[2].Kind);
            Assert.AreEqual(4, list[3].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[4].Kind);
        }

        // UniRx
        // UniRxはCast失敗時にOnErrorとなりそこでObservableは停止する
        {
            var result = await Observable.Create<object>(observer =>
                {
                    observer.OnNext(1);
                    observer.OnNext(2);
                    observer.OnNext("3");
                    observer.OnNext(4);
                    observer.OnCompleted();
                    return UniRx.Disposable.Empty;
                })
                .Cast<object, int>()
                .Materialize()
                .ToArray()
                .ToTask();

            Assert.AreEqual(1, result[0].Value);
            Assert.AreEqual(2, result[1].Value);
            Assert.AreEqual(UniRx.NotificationKind.OnError, result[2].Kind);
            Assert.AreEqual(3, result.Length); // 3以降は流れていない
        }
    }

    [Test]
    public void CatchTest()
    {
        // 購読中のObservableからOnCompleted(Exception)が発行された場合、指定したObservableに購読先を切り替える。

        // R3
        {
            var subject = new R3.Subject<int>();
            var fallbackObservable = R3Observable.Return(100);

            var catchObservable = subject.Catch(fallbackObservable);

            var list = catchObservable.ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            // OnCompleted(Exception)でfallbackObservableに切り替わる
            subject.OnCompleted(new Exception("Failed"));

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100 }, list);
        }

        // UniRx
        // UniRxはOnErrorでfallbackObservableに切り替わる
        {
            var subject = new UniRx.Subject<int>();
            var fallbackObservable = UniRxObservable.Return(100);

            var catchObservable = subject.Catch<int, Exception>(_ => fallbackObservable);

            var list = new System.Collections.Generic.List<int>();
            catchObservable.Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            // OnErrorでfallbackObservableに切り替わる
            subject.OnError(new Exception("OnErrorResume"));

            CollectionAssert.AreEqual(new[] { 1, 2, 100 }, list);
        }
    }

    [Test]
    public async Task ChunkTest()
    {
        // 発行されたOnNextの中身を指定した条件（個数や時間）でまとめて1つのOnNextとして発行する。

        // R3
        {
            // 1個飛ばしで2つずつまとめる
            // ≒ 現在値と一つ前の値をセットにする
            var observable = R3Observable.Range(1, 5).Chunk(count: 2, skip: 1);

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

        // UniRx
        // UniRxではBuffer
        {
            var result =
                await UniRxObservable.Range(1, 5)
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

    [UnityTest]
    public IEnumerator ChunkFrameTest() => UniTask.ToCoroutine(async () =>
    {
        // 指定したUnityのフレーム区間内に発行されたOnNextをまとめて1つのOnNextして発行する。

        // R3
        {
            // 時間管理をFakeFrameProviderで行う
            var fakeFrameProvider = new FakeFrameProvider();

            var subject = new R3.Subject<int>();

            // 1フレームごとにまとめる
            var list = subject.ChunkFrame(1, fakeFrameProvider).ToLiveList();

            // 1F目
            subject.OnNext(1);
            fakeFrameProvider.Advance();

            // 2F目
            subject.OnNext(2);
            subject.OnNext(3);
            fakeFrameProvider.Advance();

            // 3F目
            subject.OnNext(4);
            fakeFrameProvider.Advance();

            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1 },
                new[] { 2, 3 },
                new[] { 4 }
            }, list.ToArray());
        }

        // UniRx
        // UniRxではBatchFrame
        {
            var subject = new UniRx.Subject<int>();

            var list = new List<IList<int>>();
            subject
                .BatchFrame(1, FrameCountType.Update)
                .Subscribe(x => list.Add(x));

            // 1F目
            subject.OnNext(1);
            await UniTask.NextFrame();
            // 2F目
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.NextFrame();
            // 3F目
            subject.OnNext(4);
            subject.OnCompleted();
            await UniTask.NextFrame();

            CollectionAssert.AreEqual(new[]
            {
                new[] { 1 },
                new[] { 2, 3 },
                new[] { 4 }
            }, list.ToArray());
        }
    });

    [Test]
    public void ConcatTest()
    {
        // OnCompleted発行時に次のObservableに購読先を切り替える。

        // R3
        {
            var firstObservable = R3Observable.Range(1, 3);
            var secondObservable = R3Observable.Range(4, 3);

            var list = firstObservable.Concat(secondObservable).ToLiveList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, list);
        }

        // UniRx
        {
            var firstObservable = UniRxObservable.Range(1, 3);
            var secondObservable = UniRxObservable.Range(4, 3);

            var list = new List<int>();
            firstObservable.Concat(secondObservable).Subscribe(list.Add);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, list);
        }
    }

    [Test]
    public async Task ContainsAsyncTest()
    {
        // 指定した値を含んだOnNextが発行されたら即座に完了するTask<bool>に変換する。
        // AnyAsyncとの違いはこちらは値そのものを引数に取る。

        // R3
        {
            var subject = new R3.Subject<int>();

            // 3が発行されたか？
            var task = subject.ContainsAsync(3);

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }

        // UniRx
        // Containsは存在しないのでFirstOrDefaultで代用
        {
            var subject = new UniRx.Subject<int>();

            // 3が発行されたか？
            var task = subject.FirstOrDefault(x => x == 3).Select(_ => true).DefaultIfEmpty(false).ToTask();

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }
    }

    [Test]
    public async Task CountAsyncTest()
    {
        // OnCompleted発行時に、購読中に発行された条件を満たす値の個数を返す。

        // R3
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await R3Observable.ToObservable(array)
                .CountAsync(x => x % 2 == 0); // 偶数の個数を数える

            Assert.AreEqual(2, result);
        }

        // UniRx
        // Countは存在しないのでAggregateで代用
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var result = await UniRxObservable.ToObservable(array)
                .Aggregate(0, (prev, curr) => curr % 2 == 0 ? prev + 1 : prev)
                .ToTask();

            Assert.AreEqual(2, result);
        }
    }

    [Test]
    public async Task DebounceTest()
    {
        // 本来はFakeTimeProviderを使ってテストしたいがUnityでうまく動作しないため、実際の時間を使う

        // メッセージの流量を減らす。OnNextがまとめて発行されたときに、最後に値が発行されてから一定時間経過後に最後のOnNextを1つだけ発行する。

        // R3
        {
            var subject = new R3.Subject<int>();

            // 連続して値が発行された場合は値が落ち着いてから100ms後に最後の値を発行する
            var list = subject.Debounce(TimeSpan.FromMilliseconds(100), UnityTimeProvider.Update).ToLiveList();

            subject.OnNext(1);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(4);
            subject.OnNext(5); // ここから100ms経過すると5が発行される
            await UniTask.Delay(TimeSpan.FromMilliseconds(150), DelayType.Realtime);
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 5 }, list);
        }

        // UniRx
        // Throttle
        {
            var subject = new UniRx.Subject<int>();
            
            var list = new List<int>();
            subject
                .Throttle(TimeSpan.FromMilliseconds(100), Scheduler.MainThread)
                .Subscribe(list.Add);
            
            subject.OnNext(1);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(2);
            subject.OnNext(3);
            await UniTask.Delay(TimeSpan.FromMilliseconds(16), DelayType.Realtime);
            subject.OnNext(4);
            subject.OnNext(5); // ここから100ms経過すると5が発行される
            await UniTask.Delay(TimeSpan.FromMilliseconds(150), DelayType.Realtime);
            subject.OnCompleted();
            
            CollectionAssert.AreEqual(new[] { 5 }, list);
        }
    }

    [Test]
    public async Task DebounceFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DefaultIfEmptyTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DelayTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DelayFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DelaySubscriptionTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DelaySubscriptionFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DematerializeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DistinctTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DistinctByTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DistinctUntilChangedTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DistinctUntilChangedByTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DoTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DoCancelOnCompletedTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ElementAtAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ElementAtOrDefaultAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task FirstAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task FirstOrDefaultAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ForEachAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task FrameCountTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task FrameIntervalTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task IgnoreElementsTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task IgnoreOnErrorResumeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task IndexTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task IsEmptyAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task LastAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task LastOrDefaultAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task LongCountAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MaterializeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MaxAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MaxByAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MergeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MinAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MinByAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MinMaxAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task MulticastTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ObserveOnTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ObserveOnCurrentSynchronizationContextTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ObserveOnThreadPoolTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task OfTypeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task OnErrorResumeAsFailureTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task PairwiseTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task PrependTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task PublishTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task RefCountTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ReplayTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ReplayFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ScanTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SelectTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SelectAwaitTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SelectManyTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SequenceEqualAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ShareTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SingleAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SingleOrDefaultAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SkipTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SkipFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SkipLastTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SkipLastFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SkipUntilTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SkipWhileTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SubscribeAwaitTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SubscribeOnTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SubscribeOnCurrentSynchronizationContextTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SubscribeOnSynchronizeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SubscribeOnThreadPoolTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SumAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SwitchTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SynchronizeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TakeTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TakeFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TakeLastTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TakeLastFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TakeUntilTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TakeWhileTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ThrottleFirstTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ThrottleFirstFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ThrottleFirstLastTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ThrottleFirstLastFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ThrottleLastTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ThrottleLastFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TimeIntervalTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TimeoutTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TimeoutFrameTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TimestampTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToArrayAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToAsyncEnumerableTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToDictionaryAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToHashSetAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToListAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToLiveListTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task ToLookupAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task TrampolineTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task WaitAsyncTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task WhereTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task WhereAwaitTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task WhereNotNullTest()
    {
        Assert.Fail();
    }

    [Test]
    public async Task WithLatestFromTest()
    {
        Assert.Fail();
    }
}
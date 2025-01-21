using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using R3.Collections;
using UniRx;
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
        // OnCompleted発行時に、過去に発行された値のうち少なくとも1つが条件を満たしているかを調べる。

        // R3
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            // 1を含む? => true
            var result1 = await R3Observable.ToObservable(array)
                .AnyAsync(x => x == 1);
            Assert.IsTrue(result1);

            // 0を含む? => false
            var result2 = await R3Observable.ToObservable(array)
                .AnyAsync(x => x == 0);
            Assert.IsFalse(result2);
        }

        // UniRx
        // Allは存在しないのでToArray()してLINQのAnyで代用
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            // 1を含む? => true
            var result1 = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();
            Assert.IsTrue(result1.Any(x => x == 1));

            // 0を含む? => false
            var result2 = await UniRxObservable.ToObservable(array)
                .ToArray()
                .ToTask();
            Assert.IsFalse(result2.All(x => x == 0));
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
    public async Task CatchTest()
    {
    }

    [Test]
    public async Task ChunkTest()
    {
    }

    [Test]
    public async Task ChunkFrameTest()
    {
    }

    [Test]
    public async Task ConcatTest()
    {
    }

    [Test]
    public async Task ContainsAsyncTest()
    {
    }

    [Test]
    public async Task CountAsyncTest()
    {
    }

    [Test]
    public async Task DebounceTest()
    {
    }

    [Test]
    public async Task DebounceFrameTest()
    {
    }

    [Test]
    public async Task DefaultIfEmptyTest()
    {
    }

    [Test]
    public async Task DelayTest()
    {
    }

    [Test]
    public async Task DelayFrameTest()
    {
    }

    [Test]
    public async Task DelaySubscriptionTest()
    {
    }

    [Test]
    public async Task DelaySubscriptionFrameTest()
    {
    }

    [Test]
    public async Task DematerializeTest()
    {
    }

    [Test]
    public async Task DistinctTest()
    {
    }

    [Test]
    public async Task DistinctByTest()
    {
    }

    [Test]
    public async Task DistinctUntilChangedTest()
    {
    }

    [Test]
    public async Task DistinctUntilChangedByTest()
    {
    }

    [Test]
    public async Task DoTest()
    {
    }

    [Test]
    public async Task DoCancelOnCompletedTest()
    {
    }

    [Test]
    public async Task ElementAtAsyncTest()
    {
    }

    [Test]
    public async Task ElementAtOrDefaultAsyncTest()
    {
    }

    [Test]
    public async Task FirstAsyncTest()
    {
    }

    [Test]
    public async Task FirstOrDefaultAsyncTest()
    {
    }

    [Test]
    public async Task ForEachAsyncTest()
    {
    }

    [Test]
    public async Task FrameCountTest()
    {
    }

    [Test]
    public async Task FrameIntervalTest()
    {
    }

    [Test]
    public async Task IgnoreElementsTest()
    {
    }

    [Test]
    public async Task IgnoreOnErrorResumeTest()
    {
    }

    [Test]
    public async Task IndexTest()
    {
    }

    [Test]
    public async Task IsEmptyAsyncTest()
    {
    }

    [Test]
    public async Task LastAsyncTest()
    {
    }

    [Test]
    public async Task LastOrDefaultAsyncTest()
    {
    }

    [Test]
    public async Task LongCountAsyncTest()
    {
    }

    [Test]
    public async Task MaterializeTest()
    {
    }

    [Test]
    public async Task MaxAsyncTest()
    {
    }

    [Test]
    public async Task MaxByAsyncTest()
    {
    }

    [Test]
    public async Task MergeTest()
    {
    }

    [Test]
    public async Task MinAsyncTest()
    {
    }

    [Test]
    public async Task MinByAsyncTest()
    {
    }

    [Test]
    public async Task MinMaxAsyncTest()
    {
    }

    [Test]
    public async Task MulticastTest()
    {
    }

    [Test]
    public async Task ObserveOnTest()
    {
    }

    [Test]
    public async Task ObserveOnCurrentSynchronizationContextTest()
    {
    }

    [Test]
    public async Task ObserveOnThreadPoolTest()
    {
    }

    [Test]
    public async Task OfTypeTest()
    {
    }

    [Test]
    public async Task OnErrorResumeAsFailureTest()
    {
    }

    [Test]
    public async Task PairwiseTest()
    {
    }

    [Test]
    public async Task PrependTest()
    {
    }

    [Test]
    public async Task PublishTest()
    {
    }

    [Test]
    public async Task RefCountTest()
    {
    }

    [Test]
    public async Task ReplayTest()
    {
    }

    [Test]
    public async Task ReplayFrameTest()
    {
    }

    [Test]
    public async Task ScanTest()
    {
    }

    [Test]
    public async Task SelectTest()
    {
    }

    [Test]
    public async Task SelectAwaitTest()
    {
    }

    [Test]
    public async Task SelectManyTest()
    {
    }

    [Test]
    public async Task SequenceEqualAsyncTest()
    {
    }

    [Test]
    public async Task ShareTest()
    {
    }

    [Test]
    public async Task SingleAsyncTest()
    {
    }

    [Test]
    public async Task SingleOrDefaultAsyncTest()
    {
    }

    [Test]
    public async Task SkipTest()
    {
    }

    [Test]
    public async Task SkipFrameTest()
    {
    }

    [Test]
    public async Task SkipLastTest()
    {
    }

    [Test]
    public async Task SkipLastFrameTest()
    {
    }

    [Test]
    public async Task SkipUntilTest()
    {
    }

    [Test]
    public async Task SkipWhileTest()
    {
    }

    [Test]
    public async Task SubscribeAwaitTest()
    {
    }

    [Test]
    public async Task SubscribeOnTest()
    {
    }

    [Test]
    public async Task SubscribeOnCurrentSynchronizationContextTest()
    {
    }

    [Test]
    public async Task SubscribeOnSynchronizeTest()
    {
    }

    [Test]
    public async Task SubscribeOnThreadPoolTest()
    {
    }

    [Test]
    public async Task SumAsyncTest()
    {
    }

    [Test]
    public async Task SwitchTest()
    {
    }

    [Test]
    public async Task SynchronizeTest()
    {
    }

    [Test]
    public async Task TakeTest()
    {
    }

    [Test]
    public async Task TakeFrameTest()
    {
    }

    [Test]
    public async Task TakeLastTest()
    {
    }

    [Test]
    public async Task TakeLastFrameTest()
    {
    }

    [Test]
    public async Task TakeUntilTest()
    {
    }

    [Test]
    public async Task TakeWhileTest()
    {
    }

    [Test]
    public async Task ThrottleFirstTest()
    {
    }

    [Test]
    public async Task ThrottleFirstFrameTest()
    {
    }

    [Test]
    public async Task ThrottleFirstLastTest()
    {
    }

    [Test]
    public async Task ThrottleFirstLastFrameTest()
    {
    }

    [Test]
    public async Task ThrottleLastTest()
    {
    }

    [Test]
    public async Task ThrottleLastFrameTest()
    {
    }

    [Test]
    public async Task TimeIntervalTest()
    {
    }

    [Test]
    public async Task TimeoutTest()
    {
    }

    [Test]
    public async Task TimeoutFrameTest()
    {
    }

    [Test]
    public async Task TimestampTest()
    {
    }

    [Test]
    public async Task ToArrayAsyncTest()
    {
    }

    [Test]
    public async Task ToAsyncEnumerableTest()
    {
    }

    [Test]
    public async Task ToDictionaryAsyncTest()
    {
    }

    [Test]
    public async Task ToHashSetAsyncTest()
    {
    }

    [Test]
    public async Task ToListAsyncTest()
    {
    }

    [Test]
    public async Task ToLiveListTest()
    {
    }

    [Test]
    public async Task ToLookupAsyncTest()
    {
    }

    [Test]
    public async Task TrampolineTest()
    {
    }

    [Test]
    public async Task WaitAsyncTest()
    {
    }

    [Test]
    public async Task WhereTest()
    {
    }

    [Test]
    public async Task WhereAwaitTest()
    {
    }

    [Test]
    public async Task WhereNotNullTest()
    {
    }

    [Test]
    public async Task WithLatestFromTest()
    {
    }
}
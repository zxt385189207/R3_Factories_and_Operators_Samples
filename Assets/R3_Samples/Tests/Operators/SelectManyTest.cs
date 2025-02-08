using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class SelectManyTest
    {
        [Test]
        public void R3_SelectMany_新しいObservableを生成して並列に合成する()
        {
            using var subject = new R3.Subject<int>();
            var fakeFrameProvider = new FakeFrameProvider();

            using var list = subject.SelectMany(CreateObservable).ToLiveList();
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            // 3F進む
            fakeFrameProvider.Advance(3);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 2, 3, 3 }, list);
            return;


            // -- 

            // 入力された整数回数だけ、1Fごとにその整数値を発行するObservableを生成する
            Observable<int> CreateObservable(int x)
            {
                return R3.Observable.Create<int>(async (observer, ct) =>
                {
                    for (int i = 0; i < x; i++)
                    {
                        await fakeFrameProvider.WaitAsync(ct: ct);
                        observer.OnNext(x);
                    }

                    observer.OnCompleted();
                });
            }
        }
        
        [Test]
        public async Task UniRx_SelectMany()
        {
            using var subject = new UniRx.Subject<int>();

            var list = new List<int>();
            
            // Observable.Repeat()は指定回数だけ指定の値を発行するObservableを生成する
            // Scheduler.MainThreadを指定すると1Fごとに1つずつ発行される
            subject
                .SelectMany(x => UniRx.Observable.Repeat(x, x, Scheduler.MainThread))
                .Subscribe(list.Add);
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            await UniTask.DelayFrame(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 2, 3, 3 }, list);

            
        }
    }
}
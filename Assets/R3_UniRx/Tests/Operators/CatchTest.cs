using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class CatchTest
    {
        [Test]
        public void R3_Catch_購読中のObservableが異常終了したとき指定したObservableに購読先を切り替える()
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


        [Test]
        public void UniRx_Catch()
        {
            // UniRxはOnErrorでfallbackObservableに切り替わる
            var subject = new UniRx.Subject<int>();
            var fallbackObservable = UniRxObservable.Return(100);

            var catchObservable = subject.Catch<int, Exception>(_ => fallbackObservable);

            var list = new List<int>();
            catchObservable.Subscribe(list.Add);

            subject.OnNext(1);
            subject.OnNext(2);
            // OnErrorでfallbackObservableに切り替わる
            subject.OnError(new Exception("OnErrorResume"));

            CollectionAssert.AreEqual(new[] { 1, 2, 100 }, list);
        }
    }
}
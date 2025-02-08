using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class CatchTest
    {
        [Test]
        public void R3_Catch_購読中のObservableが異常終了したとき指定したObservableに購読先を切り替える()
        {
        using var subject = new R3.Subject<int>();
            var fallbackObservable = R3.Observable.Return(100);

            var catchObservable = subject.Catch(fallbackObservable);

            using var list = catchObservable.ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            // OnCompleted(Exception)でfallbackObservableに切り替わる
            subject.OnCompleted(new Exception("Failed"));

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 100 }, list);
        }

        [Test]
        public void R3_Catch_購読中のObservableが異常終了したときExceptionごとに指定したObservableに購読先を切り替える()
        {
            using var subject = new R3.Subject<int>();
            var fallbackObservable = R3.Observable.Return(100);

            var catchObservable = subject.Catch((Exception ex) =>
            {
                // Exceptionの型や内容に応じてfallback先を切り替える
                return ex switch
                {
                    Exception e when e.Message == "Failed" => fallbackObservable,
                    _ => R3.Observable.Empty<int>()
                };
            });

            using var list = catchObservable.ToLiveList();

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
        using var subject = new UniRx.Subject<int>();
            var fallbackObservable = UniRx.Observable.Return(100);

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
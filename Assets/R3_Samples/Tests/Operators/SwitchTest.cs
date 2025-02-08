using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class SwitchTest
    {
        [Test]
        public void R3_Switch_次のObservableへ順次購読対象を移す()
        {
            var subject1 = new R3.Subject<int>();
            var subject2 = new R3.Subject<int>();
            var subject3 = new R3.Subject<int>();

            // Observable<int>を発行するSubject
            var observableSubject = new R3.Subject<Observable<int>>();

            // Observable<Observable<int>>を順次購読する
            using var list = observableSubject.Switch().ToLiveList();

            // 1つ目のObservableを発行
            observableSubject.OnNext(subject1);

            subject1.OnNext(1);

            // 2つ目のObservableを発行
            observableSubject.OnNext(subject2);

            subject1.OnNext(1); // 1つめのObservableも発行している
            subject2.OnNext(2);
            subject2.OnCompleted(); // 現在のObservableが完了しても大本が生きてるなら続行

            // 3つ目のObservableを発行
            observableSubject.OnNext(subject3);

            subject1.OnNext(1); // 1つめのObservableがまだ発行している
            subject3.OnNext(3);

            observableSubject.OnCompleted(); // 大本が完了したが、subject3が生きているので続行

            subject3.OnNext(3); // 3つめのObservableがまだ発行している

            subject3.OnCompleted(); // 3つめのObservableが完了

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 3 }, list);
        }
        
        
        [Test]
        public void UniRx_Switch_次のObservableへ順次購読対象を移す()
        {
            var subject1 = new UniRx.Subject<int>();
            var subject2 = new UniRx.Subject<int>();
            var subject3 = new UniRx.Subject<int>();

            // Observable<int>を発行するSubject
            var observableSubject = new UniRx.Subject<IObservable<int>>();

            var list = new List<int>();
            
            // Observable<Observable<int>>を順次購読する
            observableSubject.Switch().Subscribe(list.Add);

            // 1つ目のObservableを発行
            observableSubject.OnNext(subject1);

            subject1.OnNext(1);

            // 2つ目のObservableを発行
            observableSubject.OnNext(subject2);

            subject1.OnNext(1); // 1つめのObservableも発行している
            subject2.OnNext(2);
            subject2.OnCompleted(); // 現在のObservableが完了しても大本が生きてるなら続行

            // 3つ目のObservableを発行
            observableSubject.OnNext(subject3);

            subject1.OnNext(1); // 1つめのObservableがまだ発行している
            subject3.OnNext(3);

            observableSubject.OnCompleted(); // 大本が完了したが、subject3が生きているので続行

            subject3.OnNext(3); // 3つめのObservableがまだ発行している

            subject3.OnCompleted(); // 3つめのObservableが完了

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 3 }, list);
        }
    }
}
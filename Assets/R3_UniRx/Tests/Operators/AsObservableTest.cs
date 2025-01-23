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

namespace R3_UniRx.Tests.Operators
{
    public class AsObservableTest
    {
        [Test]
        public void R3_AsObservable_Observable型に変換する()
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


        [Test]
        public void UniRx_AsObservable()
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
}
using System;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public class AsObservableTest
    {
        [Test]
        public void R3_AsObservable_Observable型に変換する()
        {
        using var subject = new R3.Subject<int>();
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
        using var subject = new UniRx.Subject<int>();
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
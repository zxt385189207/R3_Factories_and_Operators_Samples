using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class ConcatTest
    {
        [Test]
        public void R3_Concat_OnCompleted発行時に次のObservableに購読先を切り替える()
        {
            var firstObservable = R3.Observable.Range(1, 3);
            var secondObservable = R3.Observable.Range(4, 3);

            using var list = firstObservable.Concat(secondObservable).ToLiveList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, list);
        }

        [Test]
        public void UniRx_Concat()
        {
            var firstObservable = UniRx.Observable.Range(1, 3);
            var secondObservable = UniRx.Observable.Range(4, 3);

            var list = new List<int>();
            firstObservable.Concat(secondObservable).Subscribe(list.Add);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, list);
        }
    }
}
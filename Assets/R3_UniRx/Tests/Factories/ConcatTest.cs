using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public class ConcatTest
    {
        [Test]
        public void Concat_OnCompleted発行時に次のObservableに購読先を切り替える()
        {
            var firstObservable = Observable.Range(1, 3);
            var secondObservable = Observable.Range(4, 3);

            var list = Observable.Concat(firstObservable, secondObservable).ToLiveList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, list);
        }
    }
}
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public class AmbTest
    {
        [Test]
        public async Task Amb_2つのObservableを同時に購読し値が先着した方のObservableのみを採択する()
        {
            // 2つのObservableを同時に購読し、値が先着した方のObservableのみを採択する。

            var o1 = R3.Observable.Create<int>(async (observer, ct) =>
            {
                // 100ms待ってから値を発行
                await Task.Delay(100, ct);

                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                observer.OnCompleted();
            });

            var o2 = R3.Observable.Create<int>(async (observer, _) =>
            {
                // 即座に発行
                observer.OnNext(4);
                observer.OnNext(5);
                observer.OnNext(6);
                await Task.Yield();
                observer.OnCompleted();
            });

            var result = await R3.Observable.Amb(o1, o2).ToArrayAsync();

            // o2が先着しているのでo2の値が採択される
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, result);
        }
    }
}
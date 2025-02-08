using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class AsUnitObservableTest
    {
        [Test]
        public async Task R3_AsUnitObservableTest_UnitなObservableに変換する()
        {
            var observable = R3.Observable.Range(1, 3);

            // Unitに変換
            var result = await observable.AsUnitObservable().ToArrayAsync();

            CollectionAssert.AreEqual(new[] { R3.Unit.Default, R3.Unit.Default, R3.Unit.Default }, result);
        }


        [Test]
        public async Task UniRx_AsUnitObservable()
        {
            var observable = UniRx.Observable.Range(1, 3);

            // Unitに変換
            var result = await observable.AsUnitObservable().ToArray().ToTask();

            CollectionAssert.AreEqual(new[] { UniRx.Unit.Default, UniRx.Unit.Default, UniRx.Unit.Default }, result);
        }
    }
}
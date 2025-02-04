using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class DeferTest
    {
        [Test]
        public async Task Defer_Observableの評価を遅延させる()
        {
            var value = 0;
            
            // 即時評価される
            var observableNoDefer = Observable.Return(value);
            
            // Deferを使うと評価が遅延される
            var observableDefer = Observable.Defer(() => Observable.Return(value));


            // ここでvalueを変更してもobservableNoDeferは定義時の値が使用されている
            value = 1;
            
            // observableNoDeferは0を返す
            Assert.AreEqual(0, await observableNoDefer.FirstAsync());
            
            // observableDeferはSubscribeしたタイミングで評価される
            // そのためこの時点でのvalueが使用されてObservableが構築されるため、1を返す
            Assert.AreEqual(1, await observableDefer.FirstAsync());
        }
    }
}
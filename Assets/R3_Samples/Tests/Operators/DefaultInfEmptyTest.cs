using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class DefaultIfEmpty
    {
        [Test]
        public async Task R3_DefaultIfEmpty_Observableが空のときに指定した値を発行する()
        {
            // OnNextがある
            var result = await R3.Observable.Return(1).DefaultIfEmpty(100).LastAsync();
            // DefaultIfEmptyは発火しない
            Assert.AreEqual(1, result);
            
            // OnNextがない
            result = await R3.Observable.Empty<int>().DefaultIfEmpty(100).LastAsync();
            // DefaultIfEmptyが発火する
            Assert.AreEqual(100, result);
        }
      
        [Test]
        public async Task UniRx_DefaultIfEmpty()
        {
            // OnNextがある
            var result = await UniRx.Observable.Return(1).DefaultIfEmpty(100).Last();
            // DefaultIfEmptyは発火しない
            Assert.AreEqual(1, result);
            
            // OnNextがない
            result = await UniRx.Observable.Empty<int>().DefaultIfEmpty(100).Last();
            // DefaultIfEmptyが発火する
            Assert.AreEqual(100, result);

        }
    }
}
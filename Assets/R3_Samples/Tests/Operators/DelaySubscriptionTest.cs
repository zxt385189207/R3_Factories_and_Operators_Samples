using System;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class DelaySubscriptionTest
    {
        [Test]
        public async Task R3_DelaySubscription_遅れてSubscribeする()
        {
            var isSubscribed = false;

            using var subject = new R3.Subject<int>();

            // Subscribeされたらフラグを立てる
            var observable = subject.Do(onSubscribe: () => isSubscribed = true);

            // 100ms遅らせてSubscribeする
            observable.DelaySubscription(TimeSpan.FromMilliseconds(100), TimeProvider.System).Subscribe();
            
            // まだSubscribeされていない
            Assert.IsFalse(isSubscribed); 
            
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            
            // Subscribeされている
            Assert.IsTrue(isSubscribed);
        }

        [Test]
        public async Task UniRx_DelaySubscription()
        {
            var isSubscribed = false;
            
            UniRx.Observable.Return(1)
                .DoOnSubscribe(() => isSubscribed = true)
                .DelaySubscription(TimeSpan.FromMilliseconds(100))
                .Subscribe();
            
            Assert.IsFalse(isSubscribed);
            
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            
            Assert.IsTrue(isSubscribed);
        }
    }
}
using System;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class DoTest
    {
        [Test]
        public void R3_Do_各種イベント時にデリゲートを呼び出す()
        {
            using var subject = new R3.Subject<int>();

            var isOnNextCalled = false;
            var isOnErrorResumeCalled = false;
            var isOnCompletedCalled = false;
            var isOnSubscribeCalled = false;

            var observable = subject.Do(
                onNext: _ => { isOnNextCalled = true; },
                onErrorResume: _ => { isOnErrorResumeCalled = true; },
                onCompleted: _ => { isOnCompletedCalled = true; },
                onSubscribe: () => { isOnSubscribeCalled = true; }
            );

            // すべて呼び出されていない
            Assert.IsFalse(isOnNextCalled);
            Assert.IsFalse(isOnErrorResumeCalled);
            Assert.IsFalse(isOnCompletedCalled);
            Assert.IsFalse(isOnSubscribeCalled);

            observable.Subscribe(_ => { }, _ => { }, _ => { });

            // OnSubscribeだけ呼び出されている
            Assert.IsTrue(isOnSubscribeCalled);
            Assert.IsFalse(isOnNextCalled);
            Assert.IsFalse(isOnErrorResumeCalled);
            Assert.IsFalse(isOnCompletedCalled);

            subject.OnNext(1);

            // OnNextだけ呼び出されている
            Assert.IsTrue(isOnNextCalled);
            Assert.IsFalse(isOnErrorResumeCalled);
            Assert.IsFalse(isOnCompletedCalled);

            subject.OnErrorResume(new Exception());

            // OnErrorResumeだけ呼び出されている
            Assert.IsTrue(isOnErrorResumeCalled);
            Assert.IsFalse(isOnCompletedCalled);

            subject.OnCompleted();

            // OnCompletedが呼び出されている
            Assert.IsTrue(isOnCompletedCalled);
        }

        [Test]
        public void UniRx_Do()
        {
            var subject = new UniRx.Subject<int>();

            var isOnNextCalled = false;
            var isOnCompletedCalled = false;
            var isOnSubscribeCalled = false;

            var observable = subject.Do(
                    _ => { isOnNextCalled = true; },
                    _ => { },
                    () => { isOnCompletedCalled = true; }
                )
                .DoOnSubscribe(() => isOnSubscribeCalled = true);

            // すべて呼び出されていない
            Assert.IsFalse(isOnNextCalled);
            Assert.IsFalse(isOnCompletedCalled);
            Assert.IsFalse(isOnSubscribeCalled);

            observable.Subscribe();

            // OnSubscribeだけ呼び出されている
            Assert.IsTrue(isOnSubscribeCalled);
            Assert.IsFalse(isOnNextCalled);
            Assert.IsFalse(isOnCompletedCalled);

            subject.OnNext(1);

            // OnNextだけ呼び出されている
            Assert.IsTrue(isOnNextCalled);
            Assert.IsFalse(isOnCompletedCalled);

            subject.OnCompleted();

            // OnCompletedが呼び出されている
            Assert.IsTrue(isOnCompletedCalled);
        }
    }
}
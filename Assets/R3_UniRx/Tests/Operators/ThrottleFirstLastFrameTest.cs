using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ThrottleFirstLastFrameTest
    {
        [Test]
        public void R3_ThrottleFirstLastFrame_OnNextが到達したら次以降のOnNextを一定フレーム遮断する_遮断終了後に最後のOnNextを発行する()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 一度OnNextが到達したら次以降のOnNextを3F間無視し、解除時に最後のOnNextを発行する
            var results = subject.ThrottleFirstLastFrame(3, fakeFrameProvider).ToLiveList();

            subject.OnNext(1); // 1F 通過

            fakeFrameProvider.Advance();

            subject.OnNext(2); // 2F 無視

            fakeFrameProvider.Advance();

            subject.OnNext(3); // 3F 無視、だが最後のOnNextなので後で通過

            fakeFrameProvider.Advance(); // reset

            subject.OnNext(4); // 1F 通過

            fakeFrameProvider.Advance();

            subject.OnNext(5); // 2F 無視

            CollectionAssert.AreEqual(new[]
            {
                1,
                3,
                4
            }, results);
        }


        [Test]
        public void UniRx_ThrottleFirstLastFrameに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
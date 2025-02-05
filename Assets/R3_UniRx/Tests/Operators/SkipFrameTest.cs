using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SkipFrameTest
    {
        [Test]
        public void R3_SkipFrame_購読開始から指定したフレーム間のOnNextを無視する()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();

            // 購読開始から2フレーム目までのOnNextを無視
            using var list = subject.SkipFrame(2, fakeFrameProvider).ToLiveList();
            
            
            subject.OnNext(1); // 1F目、無視
            fakeFrameProvider.Advance();
            
            subject.OnNext(2); // 2F目、無視
            fakeFrameProvider.Advance();
            
            subject.OnNext(3); // 3F目、ここから流れる
            fakeFrameProvider.Advance();
            
            subject.OnNext(4);
            fakeFrameProvider.Advance();
            
            CollectionAssert.AreEqual(new[]
            {
                3,
                4
            }, list);
        }

        [Test]
        public void UniRx_SkipFrameに相当するものは存在しない()
        {
            Assert.Ignore();
        }
    }
}
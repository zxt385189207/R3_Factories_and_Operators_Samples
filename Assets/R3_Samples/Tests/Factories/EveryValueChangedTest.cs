using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class EveryValueChangedTest
    {
        private class Data
        {
            public int Value { get; set; }

            public Data(int value)
            {
                Value = value;
            }
        }

        [Test]
        public void EveryValueChanged_オブジェクトを毎フレーム監視して差分があればメッセージを発行する()
        {
            using var cts = new CancellationTokenSource();
            var fakeFrameProvider = new FakeFrameProvider(); // テスト用のフレームカウントを提供するクラス

            var data = new Data(0);

            // [data]のもつ[Value]プロパティを毎フレーム監視して、値が変更されたらその値を発行する
            using var list = Observable.EveryValueChanged(data, d => d.Value, fakeFrameProvider, cts.Token)
                .Materialize()
                .ToLiveList();

            // 購読直後は現在の値がまず発行される
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[0].Kind);
            Assert.AreEqual(0, list[0].Value);

            // フレームを進める
            fakeFrameProvider.Advance();

            // フレームは進んだが差分がないのでOnNextは発行されない
            Assert.AreEqual(1, list.Count); // 増えてない

            // data.Valueを変更する
            data.Value = 1;
            
            // フレームを進める
            fakeFrameProvider.Advance();
            
            // data.Valueが変更されたのでOnNextが発行される
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[1].Kind);
            Assert.AreEqual(1, list[1].Value);
            
            // data.Valueを複数回変更
            data.Value = 2;
            data.Value = 3;
            
            // フレームを進める
            fakeFrameProvider.Advance();
            
            // data.Valueが変更されたのでOnNextが発行される
            // フレームが進んだタイミングで最新の値のみが発行される
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(NotificationKind.OnNext, list[2].Kind);
            Assert.AreEqual(3, list[2].Value);
            
            // data.Valueを複数回変更
            data.Value = 3;
            data.Value = 0;
            data.Value = 3;
            
            // フレームを進める
            fakeFrameProvider.Advance();
            
            // data.Valueは変更されが、前フレームとの差分がないのでOnNextは発行されない
            Assert.AreEqual(3, list.Count);　// 増えてない

            // CancellationTokenがキャンセルされるとOnCompletedが発行される
            cts.Cancel();
            
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, list[3].Kind);
        }
    }
}
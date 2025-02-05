using System;
using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class FromEventTest
    {
        // ボタンがクリックされたときに発行されるイベントがあったとする
        private Action OnButtonClicked;

        // スライダーの値が変更されたときに発行されるイベントがあったとする
        private Action<int> OnSliderChanged;
        
        private delegate void PushHandler();

        private event PushHandler PushEvent;

        [Test]
        public void FromEvent_デリゲートやeventをObservableに変換する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var buttonList = Observable.FromEvent(
                    h => OnButtonClicked += h,
                    h => OnButtonClicked -= h,
                    ct)
                .Materialize()
                .ToLiveList();

            var sliderList = Observable.FromEvent<int>(
                    h => OnSliderChanged += h,
                    h => OnSliderChanged -= h,
                    ct)
                .Materialize()
                .ToLiveList();
            
            // delegate & event
            var pushList = Observable.FromEvent(
                    h=> new PushHandler(h),
                    e=> PushEvent += e,
                    e=> PushEvent -= e,
                    ct)
                .Materialize()
                .ToLiveList();

            // ボタンがクリックされる
            OnButtonClicked?.Invoke();

            Assert.AreEqual(1, buttonList.Count);
            Assert.AreEqual(NotificationKind.OnNext, buttonList[0].Kind);
            Assert.AreEqual(Unit.Default, buttonList[0].Value);

            // スライダーの値が変更される
            OnSliderChanged?.Invoke(100);
            OnSliderChanged?.Invoke(200);

            Assert.AreEqual(2, sliderList.Count);
            Assert.AreEqual(100, sliderList[0].Value);
            Assert.AreEqual(200, sliderList[1].Value);
            
            // PushEvent
            PushEvent?.Invoke();
            
            Assert.AreEqual(1, pushList.Count);
            Assert.AreEqual(NotificationKind.OnNext, pushList[0].Kind);
            
            // キャンセルするとOnCompletedが発行される
            cts.Cancel();
            
            Assert.AreEqual(NotificationKind.OnCompleted, buttonList[1].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, sliderList[2].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, pushList[1].Kind);
        }
    }
}
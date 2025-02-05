using System;
using System.Threading;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class FromEventHandlerTest
    {
        event EventHandler OnButtonClicked;
        event EventHandler<int> OnSliderChanged;

        [Test]
        public void FromEventHandler_EventHandlerを用いたeventをObservableに変換する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var buttonList = Observable.FromEventHandler(
                    h => OnButtonClicked += h,
                    h => OnButtonClicked -= h,
                    ct)
                .Materialize()
                .ToLiveList();

            var sliderList = Observable.FromEventHandler<int>(
                    h => OnSliderChanged += h,
                    h => OnSliderChanged -= h,
                    ct)
                .Materialize()
                .ToLiveList();

            // ボタンがクリックされる
            OnButtonClicked?.Invoke(this, EventArgs.Empty);

            Assert.AreEqual(1, buttonList.Count);
            Assert.AreEqual(NotificationKind.OnNext, buttonList[0].Kind);
            Assert.AreSame(this, buttonList[0].Value.sender);
            Assert.AreEqual(EventArgs.Empty, buttonList[0].Value.e);

            // スライダーの値が変更される
            OnSliderChanged?.Invoke(this, 100);
            OnSliderChanged?.Invoke(this, 200);

            Assert.AreEqual(2, sliderList.Count);
            Assert.AreEqual(NotificationKind.OnNext, sliderList[0].Kind);
            Assert.AreSame(this, sliderList[0].Value.sender);
            Assert.AreEqual(100, sliderList[0].Value.e);
            
            Assert.AreEqual(NotificationKind.OnNext, sliderList[1].Kind);
            Assert.AreSame(this, sliderList[1].Value.sender);
            Assert.AreEqual(200, sliderList[1].Value.e);

            // キャンセルするとOnCompletedが発行される
            cts.Cancel();

            Assert.AreEqual(NotificationKind.OnCompleted, buttonList[1].Kind);
            Assert.AreEqual(NotificationKind.OnCompleted, sliderList[2].Kind);
        }
    }
}
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class ObservePropertyChangedTest
    {
        private class Target : INotifyPropertyChanged
        {
            private float _value;

            public float Value
            {
                get => _value;
                set
                {
                    if (value.Equals(_value)) return;
                    _value = value;
                    NotifyPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Test]
        public void ObservePropertyChanged_プロパティの変更を監視する()
        {
            var target = new Target();

            using var list =
                target
                    .ObservePropertyChanged(x => x.Value)
                    .ToLiveList();

            CollectionAssert.AreEqual(new[] { 0f }, list);

            target.Value = 1.0f;
            target.Value = 2.0f;
            target.Value = 3.0f;

            CollectionAssert.AreEqual(new[] { 0f, 1.0f, 2.0f, 3.0f }, list);
        }
    }
}
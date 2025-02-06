using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class ObservePropertyChangingTest
    {
        public class Target : INotifyPropertyChanging
        {
            private float _value;

            public event PropertyChangingEventHandler PropertyChanging;

            public Target(float value)
            {
                _value = value;
            }

            public float Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        OnPropertyChanging(nameof(Value));
                        _value = value;
                    }
                }
            }

            protected virtual void OnPropertyChanging(string propertyName)
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }
            
        }

        [Test]
        public void ObservePropertyChanging_プロパティの変更を監視する()
        {
            var target = new Target(0);

            // INotifyPropertyChangingは変更時に「変更前の値」を通知する
            using var list =
                target
                    .ObservePropertyChanging(x => x.Value)
                    .ToLiveList();

            CollectionAssert.AreEqual(new[] { 0f }, list);

            target.Value = 1.0f;
            target.Value = 2.0f;
            target.Value = 3.0f;

            CollectionAssert.AreEqual(new[] { 0f, 0.0f, 1.0f, 2.0f }, list);
        }
    }
}
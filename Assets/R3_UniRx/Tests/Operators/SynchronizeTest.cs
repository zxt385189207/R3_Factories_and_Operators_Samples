using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed　class SynchronizeTest
    {
        [Test]
        public async Task R3_Synchronize_それ以降のメッセージ処理に排他ロックをかける()
        {
            var counter = 0;

            var taskList = new List<UniTask>();
            var gate = new object();

            // 5000個のTaskが並行に実行される
            for (var i = 0; i < 5000; i++)
            {
                // Synchronizeを挟まない場合、
                // counterは壊れてしまい安定して5000になることはなくなる
                var task = UniTask.RunOnThreadPool(() =>
                {
                    R3.Observable.Return(R3.Unit.Default)
                        .Synchronize(gate)
                        .Subscribe(_ => ++counter);
                });
                taskList.Add(task);
            }

            await UniTask.WhenAll(taskList);

            // Synchronizeによりそれ以降のOnNext処理は排他ロックがかかるため
            // counterは確実に5000になる
            Assert.AreEqual(5000, counter);
        }

        [Test]
        public async Task UniRx_Synchronize()
        {
            var counter = 0;

            var taskList = new List<UniTask>();
            var gate = new object();

            // 5000個のTaskが並行に実行される
            for (var i = 0; i < 5000; i++)
            {
                // Synchronizeを挟まない場合、
                // counterは壊れてしまい安定して5000になることはなくなる
                var task = UniTask.RunOnThreadPool(() =>
                {
                    UniRx.Observable.Return(R3.Unit.Default)
                        .Synchronize(gate)
                        .Subscribe(_ => ++counter);
                });
                taskList.Add(task);
            }

            await UniTask.WhenAll(taskList);

            // Synchronizeによりそれ以降のOnNext処理は排他ロックがかかるため
            // counterは確実に5000になる
            Assert.AreEqual(5000, counter);
        }
    }
}
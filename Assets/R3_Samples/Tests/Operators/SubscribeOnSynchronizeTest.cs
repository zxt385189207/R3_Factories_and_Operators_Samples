using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Operators
{
    public sealed class SubscribeOnSynchronizeTest
    {
        [Test]
        public async Task R3_SubscribeOnSynchronize_Subscribe処理に排他ロックをかける()
        {
            var counter = 0;

            var taskList = new List<UniTask>();
            var gate = new object();

            // 5000個のTaskが並行に実行される
            for (var i = 0; i < 5000; i++)
            {
                // SubscribeOnSynchronizeを挟まない場合、
                // counterは壊れてしまい安定して5000になることはなくなる
                var task = UniTask.RunOnThreadPool(() =>
                {
                    R3.Observable.Return(R3.Unit.Default)
                        .SubscribeOnSynchronize(gate)
                        .Subscribe(_ => ++counter);
                });
                taskList.Add(task);
            }

            await UniTask.WhenAll(taskList);

            // SubscribeOnSynchronizeにより、Subscribe処理に排他ロックがかかるため
            // counterは確実に5000になる
            Assert.AreEqual(5000, counter);
        }
        
        [Test]
        public void UniRx_SubscribeOnSynchronizeは存在しない()
        {
            Assert.Ignore();
        }
    }
}
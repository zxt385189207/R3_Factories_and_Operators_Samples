using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using R3.Collections;
using UniRx;
using UnityEngine.TestTools;
using Observable = UniRx.Observable;
using UniRxObservable = UniRx.Observable;
using R3Observable = R3.Observable;

namespace R3_UniRx.Tests.Operators
{
    public class AnyAsyncTest
    {
        
        [Test]
        public async Task R3_AnyAsync_条件を満たしたOnNextが発行されたら即座に完了するTasに変換する()
        {
            var subject = new R3.Subject<int>();

            // 3が発行されたか？
            // ContainsAsyncとの違いはこちらはFuncを引数に取る。
            var task = subject.AnyAsync(x => x == 3);

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);

        }
        
        
        [Test]
        public async Task UniRx_AnyAsyncをFirstOrDefaultで再現する()
        {
            // Anyは存在しないのでFirstOrDefaultで代用
            var subject = new UniRx.Subject<int>();

            // 3が発行されたか？
            var task = subject.FirstOrDefault(x => x == 3).Select(_ => true).DefaultIfEmpty(false).ToTask();

            subject.OnNext(1);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(2);
            Assert.IsFalse(task.IsCompleted);

            subject.OnNext(3);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(await task);
        }
        
        
    }
}
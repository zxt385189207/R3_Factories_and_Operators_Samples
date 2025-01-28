using System;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SingleAsyncTest
    {
        [Test]
        public async Task R3_SingleAsync_条件を満たす値が1つである場合にその値を返す()
        {
            using var subject = new R3.Subject<int>();
            
            // 3は1つだけである
            var task = subject.SingleAsync(x => x == 3);
            
            subject.OnNext(1);
            subject.OnNext(2);
            
            // まだ条件を満たす値が出ていないので未完了
            Assert.IsFalse(task.IsCompleted);
            
            // 条件を満たす値
            subject.OnNext(3);
            
            // まだ未完了
            Assert.IsFalse(task.IsCompleted);
            
            subject.OnCompleted();
            
            // OnCompleted()が呼ばれることで確定する
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(3, await task);
            
        }
        
        [Test]
        public async Task R3_SingleAsync_条件を満たす値が2つ以上発行された場合はInvalidOperationException()
        {
            using var subject = new R3.Subject<int>();
            
            // 3は1つだけである
            var task = subject.SingleAsync(x => x == 3);
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3); // 1回ここで発行
            subject.OnNext(1);
                        
            // まだTaskは未完了
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsFaulted);

            // 2回目の発行
            subject.OnNext(3);
            
            // 2回目の発行で例外が発生
            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            
        }
        
        [Test]
        public async Task R3_SingleAsync_条件を満たす値が0個の場合はInvalidOperationException()
        {
            using var subject = new R3.Subject<int>();
            
            // 3は1つだけである
            var task = subject.SingleAsync(x => x == 3);
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(1);
                        
            // まだTaskは未完了
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsFaulted);
            
            subject.OnCompleted();
            
            // 一度も条件を満たす値が発行されなかったため例外
            Assert.IsTrue(task.IsFaulted);
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
        }
        
        [Test]
        public void UniRx_Single()
        {
            var subject = new UniRx.Subject<int>();
            
            // 3は1つだけである
            var task = subject.Single(x => x == 3).ToTask();
            
            subject.OnNext(1);
            subject.OnNext(2);
            
            // まだ条件を満たす値が出ていないので未完了
            Assert.IsFalse(task.IsCompleted);
            
            // 条件を満たす値
            subject.OnNext(3);
            
            // まだ未完了
            Assert.IsFalse(task.IsCompleted);
            
            subject.OnCompleted();
            
            // OnCompleted()が呼ばれることで確定する
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(3, task.Result);
        }
    }
}
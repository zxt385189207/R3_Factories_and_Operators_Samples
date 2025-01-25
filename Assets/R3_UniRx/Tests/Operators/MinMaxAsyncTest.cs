using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class MinMaxAsyncTest
    {
        
        private record Data(int Value);
        
        [Test]
        public void R3_MinMaxAsync_最大値と最小値を得る()
        {
            using var subject = new R3.Subject<Data>();
            
            // Task<(int Min, int Max)>である
            Task<(int Min, int Max)> task = subject.MinMaxAsync(x => x.Value);
            
            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);
            
            subject.OnNext(new Data(1));
            subject.OnNext(new Data(100));
            subject.OnNext(new Data(3));
            subject.OnNext(new Data(400));
            
            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);
            
            subject.OnCompleted();
            
            // Observableが完了することでTaskも完了する
            Assert.IsTrue(task.IsCompleted);
            
            var (min,max) = task.Result;
            Assert.AreEqual(1, min);
            Assert.AreEqual(400, max);
        }
        
        [Test]
        public void UniRx_MinMaxは存在しない()
        {
            Assert.Ignore();
        }
    }
}
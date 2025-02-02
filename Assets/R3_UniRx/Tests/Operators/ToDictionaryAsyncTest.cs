using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ToDictionaryAsyncTest
    {
        [Test]
        public void R3_ToDictionaryAsync_Dictionaryに変換する()
        {
            var subject = new R3.Subject<(int Key, string Value)>();
            
            var task = subject.ToDictionaryAsync(x => x.Key, x => x.Value);
            
            subject.OnNext((1, "a"));
            subject.OnNext((2, "b"));
            subject.OnNext((3, "c"));
            subject.OnNext((4, "A"));
            subject.OnNext((5, "B"));
            subject.OnNext((6, "C"));
            
            // まだ完了していない
            Assert.IsFalse(task.IsCompleted);
            
            subject.OnCompleted();
            
            // 完了
            Assert.IsTrue(task.IsCompleted);
            
            var result = task.Result;
            CollectionAssert.AreEqual(new Dictionary<int, string>
            {
                { 1, "a" },
                { 2, "b" },
                { 3, "c" },
                { 4, "A" },
                { 5, "B" },
                { 6, "C" },
            }, result);
        }
        
        [Test]
        public void UniRx_ToDictionaryは存在しないのでToArrayなどでがんばる()
        {
            var subject = new UniRx.Subject<(int Key, string Value)>();
            
            var resultArray = default((int Key, string Value)[]);
            
            subject.ToArray().Subscribe(x => resultArray = x);
            
            subject.OnNext((1, "a"));
            subject.OnNext((2, "b"));
            subject.OnNext((3, "c"));
            subject.OnNext((4, "A"));
            subject.OnNext((5, "B"));
            subject.OnNext((6, "C"));
            subject.OnCompleted();
            
            var dictionary = resultArray.ToDictionary(x => x.Key, x => x.Value);
            
            CollectionAssert.AreEqual(new Dictionary<int, string>
            {
                { 1, "a" },
                { 2, "b" },
                { 3, "c" },
                { 4, "A" },
                { 5, "B" },
                { 6, "C" },
            }, dictionary);
        }
    }
}
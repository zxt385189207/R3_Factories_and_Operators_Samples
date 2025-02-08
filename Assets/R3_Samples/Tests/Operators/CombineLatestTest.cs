using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class CombineLatestTest
    {
        [Test]
        public void R3_CombineLatest_複数のObservableの最新値をまとめる()
        {
            using var first = new R3.Subject<int>();
            using var second = new R3.Subject<string>();

            using var list = first.CombineLatest(second, (x, y) => x + y).ToLiveList();

            // first側入力
            first.OnNext(1);

            // second側が入力されていないのでまだ何も出力されない
            CollectionAssert.IsEmpty(list);

            // second側入力
            second.OnNext("a");

            // [1] + [a]
            CollectionAssert.AreEqual(new[] { "1a" }, list);

            // first側入力
            first.OnNext(2);

            // [2] + [a] が出力される
            CollectionAssert.AreEqual(new[] { "1a", "2a" }, list);

            // second側入力
            second.OnNext("b");

            // [2] + [b] が出力される
            CollectionAssert.AreEqual(new[] { "1a", "2a", "2b" }, list);
        }

        [Test]
        public void UniRx_CombineLatest()
        {
            using var first = new UniRx.Subject<int>();
            using var second = new UniRx.Subject<string>();

            var list = new List<string>();
            first.CombineLatest(second, (x, y) => x + y).Subscribe(list.Add);

            // first側入力
            first.OnNext(1);

            // second側が入力されていないのでまだ何も出力されない
            CollectionAssert.IsEmpty(list);

            // second側入力
            second.OnNext("a");

            // [1] + [a]
            CollectionAssert.AreEqual(new[] { "1a" }, list);

            // first側入力
            first.OnNext(2);

            // [2] + [a] が出力される
            CollectionAssert.AreEqual(new[] { "1a", "2a" }, list);

            // second側入力
            second.OnNext("b");

            // [2] + [b] が出力される
            CollectionAssert.AreEqual(new[] { "1a", "2a", "2b" }, list);
        }
    }
}
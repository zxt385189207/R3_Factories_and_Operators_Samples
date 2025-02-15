using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class CombineLatestTest
    {
        [Test]
        public void CombineLatest_複数のObservableの最新値をまとめる()
        {
            using var first = new R3.Subject<int>();
            using var second = new R3.Subject<string>();

            using var list = Observable.CombineLatest(first, second, (x, y) => x + y).ToLiveList();

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
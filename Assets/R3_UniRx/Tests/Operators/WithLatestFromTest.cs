using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class WithLatestFromTest
    {
        [Test]
        public void R3_WithLatestFrom_別のObservableを入力として与えその最新値とメインObservableの値をセットにして出力する()
        {
            using var first = new R3.Subject<int>();
            using var second = new R3.Subject<string>();

            var list = first.WithLatestFrom(second, (x, y) => x + y).ToLiveList();

            // first側入力
            first.OnNext(1);

            // second側が入力されていないのでまだ何も出力されない
            CollectionAssert.IsEmpty(list);

            // second側入力
            second.OnNext("a");

            // second側が入力されているが、first側のペアが成立していなかったのでまだ何も出力されない
            CollectionAssert.IsEmpty(list);

            // first側入力、secondsの最新値と組み合わり出力される
            first.OnNext(2);

            CollectionAssert.AreEqual(new[] { "2a" }, list);

            second.OnNext("b");
            second.OnNext("c");

            // seconds側は更新されてもfirst側のペアが成立していないので何も出力されない
            CollectionAssert.AreEqual(new[] { "2a" }, list);

            // first側入力、secondsの最新値と組み合わり出力される
            first.OnNext(3);
            first.OnNext(4);

            CollectionAssert.AreEqual(new[] { "2a", "3c", "4c" }, list);
        }

        [Test]
        public void UniRx_WithLatestFrom()
        {
            using var first = new UniRx.Subject<int>();
            using var second = new UniRx.Subject<string>();

            var list = new List<string>();
            first.WithLatestFrom(second, (x, y) => x + y).Subscribe(list.Add);

            // first側入力
            first.OnNext(1);

            // second側が入力されていないのでまだ何も出力されない
            CollectionAssert.IsEmpty(list);

            // second側入力
            second.OnNext("a");

            // second側が入力されているが、first側のペアが成立していなかったのでまだ何も出力されない
            CollectionAssert.IsEmpty(list);

            // first側入力、secondsの最新値と組み合わり出力される
            first.OnNext(2);

            CollectionAssert.AreEqual(new[] { "2a" }, list);

            second.OnNext("b");
            second.OnNext("c");

            // seconds側は更新されてもfirst側のペアが成立していないので何も出力されない
            CollectionAssert.AreEqual(new[] { "2a" }, list);

            // first側入力、secondsの最新値と組み合わり出力される
            first.OnNext(3);
            first.OnNext(4);

            CollectionAssert.AreEqual(new[] { "2a", "3c", "4c" }, list);
        }
    }
}
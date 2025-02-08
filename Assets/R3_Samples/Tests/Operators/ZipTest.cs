using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class ZipTest
    {
        [Test]
        public void R3_Zip_複数のObservableの値をキューにつめて順次まとめる()
        {
            using var first = new R3.Subject<int>();
            using var second = new R3.Subject<string>();

            using var list = first.Zip(second, (x, y) => x + y).ToLiveList();

            // first側入力
            first.OnNext(1);
            first.OnNext(2);
            first.OnNext(3);
            
            // secondが入力されていないので何も出力されない
            CollectionAssert.IsEmpty(list);
            
            // この時点でfirst側には[1][2][3]がキューにたまっている
            
            // second側入力
            // 順次first側のキューから取り出して反映されていく
            second.OnNext("a");
            
            CollectionAssert.AreEqual(new[] { "1a" }, list);
            
            second.OnNext("b");
            
            CollectionAssert.AreEqual(new[] { "1a", "2b" }, list);
            
            second.OnNext("c");
            
            CollectionAssert.AreEqual(new[] { "1a", "2b", "3c" }, list);
        }

        [Test]
        public void UniRx_Zip()
        {
            using var first = new UniRx.Subject<int>();
            using var second = new UniRx.Subject<string>();

            var list = new List<string>();
            first.Zip(second, (x, y) => x + y).Subscribe(list.Add);

            // first側入力
            first.OnNext(1);
            first.OnNext(2);
            first.OnNext(3);
            
            // secondが入力されていないので何も出力されない
            CollectionAssert.IsEmpty(list);
            
            // この時点でfirst側には[1][2][3]がキューにたまっている
            
            // second側入力
            // 順次first側のキューから取り出して反映されていく
            second.OnNext("a");
            
            CollectionAssert.AreEqual(new[] { "1a" }, list);
            
            second.OnNext("b");
            
            CollectionAssert.AreEqual(new[] { "1a", "2b" }, list);
            
            second.OnNext("c");
            
            CollectionAssert.AreEqual(new[] { "1a", "2b", "3c" }, list);
        }
    }
}
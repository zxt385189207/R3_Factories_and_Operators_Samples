#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public sealed class WhereNotNullTest
    {
        [Test]
        public void R3_WhereNotNull_値がnullでない場合のみ通過()
        {
            using var subject = new R3.Subject<string?>();

            using var list = subject.WhereNotNull().ToLiveList();

            subject.OnNext("a");
            subject.OnNext(null);
            subject.OnNext("b");
            subject.OnCompleted();

            // nullを除外して出力される
            CollectionAssert.AreEqual(new[] { "a", "b" }, list);
        }

        [Test]
        public void UniRx_WhereNotNullをWhereで代用()
        {
            using var subject = new UniRx.Subject<string?>();

            var list = new List<string>();
            subject.Where(x => x != null).Subscribe(list.Add!);

            subject.OnNext("a");
            subject.OnNext(null);
            subject.OnNext("b");
            subject.OnCompleted();

            // nullを除外して出力される
            CollectionAssert.AreEqual(new[] { "a", "b" }, list);
        }
    }
}
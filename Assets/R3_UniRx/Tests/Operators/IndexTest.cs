using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class IndexTest
    {
        [Test]
        public void R3_Index_OnNextにIndexをふる()
        {
            using var subject = new R3.Subject<string>();

            // Subscribe前のOnNextは関係ない
            subject.OnNext("-");

            var list = subject.Index().ToLiveList();

            subject.OnNext("A");
            subject.OnNext("B");
            subject.OnNext("C");

            Assert.AreEqual(new[]
            {
                (0, "A"),
                (1, "B"),
                (2, "C"),
            }, list);
        }

        [Test]
        public void UniRx_Indexは存在しないがSelectで代替可能()
        {
            using var subject = new UniRx.Subject<string>();

            var list = new List<(int, string)>();

            subject.Select((x, i) => (i, x)).Subscribe(x => list.Add(x));

            subject.OnNext("A");
            subject.OnNext("B");
            subject.OnNext("C");

            Assert.AreEqual(new[]
            {
                (0, "A"),
                (1, "B"),
                (2, "C"),
            }, list);
        }
    }
}
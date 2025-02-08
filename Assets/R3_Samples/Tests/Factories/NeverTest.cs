using NUnit.Framework;
using R3;

namespace R3_Samples.Tests.Factories
{
    public sealed class NeverTest
    {
        [Test]
        public void Never_何も発行しない()
        {
            var observable = Observable.Never<int>();

            using var list = observable.Materialize().ToLiveList();

            Assert.IsEmpty(list);
        }
    }
}
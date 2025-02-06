using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Factories
{
    public sealed class YieldFrameTest
    {
        [Test]
        public async Task YieldFrame_指定したFrameProviderの次の実行タイミングでメッセージを発行する()
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // 次のFixedUpdateの実行タイミングでメッセージを発行する
            // ConfigureAwait(false)なのでこのawait以降はFixedUpdateが継続する
            await Observable.YieldFrame(UnityFrameProvider.FixedUpdate, ct)
                .FirstAsync(cancellationToken: ct)
                .ConfigureAwait(false);

            Assert.Pass();
        }
    }
}
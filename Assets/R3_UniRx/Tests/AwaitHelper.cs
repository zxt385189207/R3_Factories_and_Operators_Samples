using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace R3_UniRx
{
    internal class FrameAwaiter : IFrameRunnerWorkItem
    {
        private readonly AutoResetUniTaskCompletionSource _utc = AutoResetUniTaskCompletionSource.Create();
        public UniTask Task => _utc.Task;

        private int _count;
        private readonly CancellationToken _ct;

        public FrameAwaiter(int count = 1, CancellationToken ct = default)
        {
            _count = count;
            _ct = ct;
            if (ct.CanBeCanceled)
            {
                ct.Register(() =>
                {
                    _utc.TrySetCanceled();
                });
            }
        }

        public bool MoveNext(long frameCount)
        {
            if(_ct.IsCancellationRequested)
            {
                return false;
            }
            
            if (--_count <= 0)
            {
                _utc.TrySetResult();
                return false;
            }

            return true;
        }
    }

    internal static class FakeFrameProviderExt
    {
        internal static UniTask WaitAsync(
            this FakeFrameProvider provider,
            int count = 1,
            CancellationToken ct = default)
        {
            var item = new FrameAwaiter(count, ct);
            provider.Register(item);
            return item.Task;
        }
    }
}
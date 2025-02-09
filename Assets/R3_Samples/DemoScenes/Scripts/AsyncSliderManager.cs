using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R3_Samples.DemoScenes
{
    public class AsyncSliderManager : MonoBehaviour
    {
        [SerializeField] private AsyncSlider _asyncSliderPrefab;

        public async UniTask AddNewAsyncSliderAsync(float seconds, string view, CancellationToken ct)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            var token = cts.Token;

            var asyncSlider = Instantiate(_asyncSliderPrefab, transform);
            await asyncSlider.WaitSecondsAsync(seconds,view, token);
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace R3_Samples.DemoScenes
{
    public class AsyncSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _text;

        public async UniTask WaitSecondsAsync(float seconds, string view, CancellationToken ct)
        {
            _text.text = view;
            try
            {
                _slider.value = 0;

                var startTime = Time.time;

                while (Time.time - startTime < seconds)
                {
                    _slider.value = (Time.time - startTime) / seconds;
                    await UniTask.Yield(ct);
                }
            }
            finally
            {
                Destroy(gameObject);
            }
        }
    }
}
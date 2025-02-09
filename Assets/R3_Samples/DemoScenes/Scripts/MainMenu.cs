using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace R3_Samples.SceneScenes
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _debounceButton;
        [SerializeField] private Button _throttleFirstButton;
        [SerializeField] private Button _throttleLastButton;
        [SerializeField] private Button _throttleFirstLastButton;

        [SerializeField] private Button _awaitSequentialButton;
        [SerializeField] private Button _awaitParallelButton;
        [SerializeField] private Button _awaitSequentialParallelButton;
        [SerializeField] private Button _awaitDropButton;
        [SerializeField] private Button _awaitSwitchButton;
        [SerializeField] private Button _awaitThrottleFirstLastButton;

        [SerializeField] private Button _zipButton;
        [SerializeField] private Button _zipLatestButton;
        [SerializeField] private Button _combineLatestButton;
        [SerializeField] private Button _withLatestFromButton;

        private string _currentSceneName;

        private void Start()
        {
            OpenSceneAsync("DebounceScene").Forget();

            _debounceButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("DebounceScene").Forget())
                .AddTo(this);
            _throttleFirstButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("ThrottleFirstScene").Forget())
                .AddTo(this);

            _throttleLastButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("ThrottleLastScene").Forget())
                .AddTo(this);

            _throttleFirstLastButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("ThrottleFirstLastScene").Forget())
                .AddTo(this);


            _awaitSequentialButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("AwaitSequentialScene").Forget())
                .AddTo(this);

            _awaitParallelButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("AwaitParallelScene").Forget())
                .AddTo(this);

            _awaitSequentialParallelButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("AwaitSequentialParallelScene").Forget())
                .AddTo(this);

            _awaitDropButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("AwaitDropScene").Forget())
                .AddTo(this);

            _awaitSwitchButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("AwaitSwitchScene").Forget())
                .AddTo(this);

            _awaitThrottleFirstLastButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("AwaitThrottleFirstLastScene").Forget())
                .AddTo(this);


            _zipButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("ZipScene").Forget())
                .AddTo(this);

            _zipLatestButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("ZipLatestScene").Forget())
                .AddTo(this);

            _combineLatestButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("CombineLatestScene").Forget())
                .AddTo(this);

            _withLatestFromButton.OnClickAsObservable()
                .Subscribe(_ => OpenSceneAsync("WithLatestFromScene").Forget())
                .AddTo(this);
        }

        private async UniTask OpenSceneAsync(string sceneName)
        {
            if (_currentSceneName != null)
            {
                await SceneManager.UnloadSceneAsync(_currentSceneName);
            }

            _currentSceneName = sceneName;
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
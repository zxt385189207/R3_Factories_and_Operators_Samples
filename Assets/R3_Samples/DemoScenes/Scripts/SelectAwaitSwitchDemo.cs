using R3;
using UnityEngine;
using UnityEngine.UI;

namespace R3_Samples.DemoScenes
{
    public class SelectAwaitSwitchDemo : MonoBehaviour
    {
        [SerializeField] private Button _onNextButton;
        [SerializeField] private Text _resultsText;
        [SerializeField] private AsyncSliderManager _asyncSliderManager;
        [SerializeField] private Text _inputText;

        private readonly ResultText _results = new(10);
        private int _inputValue = 0;
        private readonly Subject<int> _inputSubject = new Subject<int>();

        private void Start()
        {
            _resultsText.text = "";
            _inputText.text = "";

            _inputSubject.AddTo(this);

            _onNextButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _inputText.text = (++_inputValue).ToString();
                    _inputSubject.OnNext(_inputValue);
                });


            _inputSubject
                .SelectAwait(async (x, ct) =>
                {
                    var randomDelay = Random.Range(0.5f, 2.0f);
                    await _asyncSliderManager.AddNewAsyncSliderAsync(randomDelay, x.ToString(), ct);
                    return x + 100;
                }, AwaitOperation.Switch)
                .Subscribe(x =>
                {
                    _results.AddResult(x.ToString());
                    _resultsText.text = _results.ToText();
                })
                .AddTo(this);
        }
    }
}
using System.Text;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace R3_Samples.DemoScenes.Scripts
{
    public class DebounceDemo : MonoBehaviour
    {
        [SerializeField] private Button _onNextButton;
        [SerializeField] private Text _resultsText;
        [SerializeField] private AsyncSliderManager _asyncSliderManager;
        [SerializeField] private Text _inputText;

        private readonly StringBuilder _results = new StringBuilder();
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
                .Debounce((x, ct) => _asyncSliderManager.AddNewAsyncSliderAsync(1, x.ToString(), ct))
                .Subscribe(x =>
                {
                    _results.AppendLine(x.ToString());
                    _resultsText.text = _results.ToString();
                })
                .AddTo(this);
        }
    }
}
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace R3_Samples.DemoScenes
{
    public class WithLatestFromDemo : MonoBehaviour
    {
        [SerializeField] private Button _onNextButton1;
        [SerializeField] private Text _inputText1;
        [SerializeField] private Button _onNextButton2;
        [SerializeField] private Text _inputText2;

        [SerializeField] private Text _resultsText;


        private readonly ResultText _results = new(10);
        private int _inputValue1 = 0;
        private int _inputValue2 = 0;
        private readonly Subject<int> _first = new Subject<int>();
        private readonly Subject<int> _second = new Subject<int>();

        private void Start()
        {
            _resultsText.text = "";
            _inputText1.text = "";
            _inputText2.text = "";

            _first.AddTo(this);
            _second.AddTo(this);

            _onNextButton1
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _inputValue1 += 1;
                    _inputText1.text = _inputValue1.ToString();
                    _first.OnNext(_inputValue1);
                });

            _onNextButton2
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _inputValue2 += 1;
                    _inputText2.text = _inputValue2.ToString();
                    _second.OnNext(_inputValue2);
                });
            
            _first.WithLatestFrom(_second, (x, y) => $"{x},{y}")
                .Subscribe(x =>
                {
                    _results.AddResult(x.ToString());
                    _resultsText.text = _results.ToText();
                })
                .AddTo(this);
        }
    }
}
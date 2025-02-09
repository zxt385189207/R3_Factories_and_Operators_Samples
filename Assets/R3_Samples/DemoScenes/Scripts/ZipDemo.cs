using R3;
using UnityEngine;
using UnityEngine.UI;

namespace R3_Samples.DemoScenes
{
    public class ZipDemo : MonoBehaviour
    {
        [SerializeField] private Button _onNextButton1;
        [SerializeField] private Text _inputText1;
        [SerializeField] private Button _onNextButton2;
        [SerializeField] private Text _inputText2;
        [SerializeField] private Button _onNextButton3;
        [SerializeField] private Text _inputText3;

        [SerializeField] private Text _resultsText;


        private readonly ResultText _results = new(10);
        private int _inputValue1 = 0;
        private int _inputValue2 = 0;
        private int _inputValue3 = 0;
        private readonly Subject<int> _inputSubject1 = new Subject<int>();
        private readonly Subject<int> _inputSubject2 = new Subject<int>();
        private readonly Subject<int> _inputSubject3 = new Subject<int>();

        private void Start()
        {
            _resultsText.text = "";
            _inputText1.text = "";
            _inputText2.text = "";
            _inputText3.text = "";

            _inputSubject1.AddTo(this);
            _inputSubject2.AddTo(this);
            _inputSubject3.AddTo(this);

            _onNextButton1
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _inputValue1 += 1;
                    _inputText1.text = _inputValue1.ToString();
                    _inputSubject1.OnNext(_inputValue1);
                });

            _onNextButton2
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _inputValue2 += 1;
                    _inputText2.text = _inputValue2.ToString();
                    _inputSubject2.OnNext(_inputValue2);
                });

            _onNextButton3
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _inputValue3 += 1;
                    _inputText3.text = _inputValue3.ToString();
                    _inputSubject3.OnNext(_inputValue3);
                });


            Observable.Zip(_inputSubject1, _inputSubject2, _inputSubject3,
                    (x, y, z) => $"{x},{y},{z}")
                .Subscribe(x =>
                {
                    _results.AddResult(x.ToString());
                    _resultsText.text = _results.ToText();
                })
                .AddTo(this);
        }
    }
}
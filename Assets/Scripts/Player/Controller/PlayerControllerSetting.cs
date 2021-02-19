using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerSetting : MonoBehaviour
{
    [Header("____Controller Props_____")]
    [SerializeField] private VirtualJoystick _Controller;

    [SerializeField] float _ControllerDefScale;
    [SerializeField] float _ControllerMaxScale;
    [SerializeField] float _ControllerOffset;

    [Header("____Slider Props____")]
    [SerializeField] private Slider _ScaleSlider;
    [SerializeField] private TMPro.TextMeshProUGUI _ScaleValueText;

    [Space()]
    [SerializeField] private Slider _AlphaSlider;
    [SerializeField] private TMPro.TextMeshProUGUI _AlphaValueText;

    [Header("____Reposer Props____")]
    [SerializeField] private VirtualJoystickReposer _Reposer;
    [SerializeField] private Button _ReposerButton;

    private bool _IsAlreadyInit = false;

    private void Awake()
    {
        if (!_IsAlreadyInit)
        {
            _IsAlreadyInit = true;

            float scale = GameLoger.Instance.ControllerDefScale / _ControllerDefScale * 50f;

            _ScaleValueText.text = ((int)scale).ToString();
            _ScaleSlider.value = scale;

            float alpha = GameLoger.Instance.ControllerAlpha * 100f;
            _AlphaSlider.value = alpha;
            _AlphaValueText.text = ((int)alpha).ToString();

            _ScaleSlider.onValueChanged.AddListener(ScaleValueChanged);
            _AlphaSlider.onValueChanged.AddListener(AlphaValueChanged);

            _ReposerButton.onClick.AddListener(ReposerButtonClick);
        }
    }
    private void ScaleValueChanged(float value)
    {
        _ScaleValueText.text = ((int)value).ToString();
        value *= 0.02f;

        float defScale = value * _ControllerDefScale;
        float maxScale = value * _ControllerMaxScale;

        Vector3 scale = Vector3.one;
                scale.x = scale.y = defScale;

        float offset = _ControllerOffset * value;

        GameLoger.Instance.ControllerDefScale = defScale;
        GameLoger.Instance.ControllerMaxScale = maxScale;
        GameLoger.Instance.ControllerOffset   = offset;

        _Controller.SetButtonScale (defScale, maxScale);
        _Controller.SetButtonOffset(offset);
    }
    private void AlphaValueChanged(float value)
    {
        _AlphaValueText.text = ((int)value).ToString();

        float alpha = value * 0.01f;
        _Controller.SetButtonAlpha(alpha);
        
        GameLoger.Instance.ControllerAlpha = alpha;
    }
    private void ReposerButtonClick()
    {
        _Reposer.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        BackEndServerManager.Instance.Optionsaver();
    }
}

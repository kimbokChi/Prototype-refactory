using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystickSetting : MonoBehaviour
{
    [Header("____Controller Props_____")]
    [SerializeField] private PlayerController _Controller;

    [SerializeField] float _ControllerDefScale;
    [SerializeField] float _ControllerMaxScale;
    [SerializeField] float _ControllerOffset;

    [Header("____Slider Props____")]
    [SerializeField] private Slider _ScaleSlider;
    [SerializeField] private TMPro.TextMeshProUGUI _ScaleValueText;

    [Space()]
    [SerializeField] private Slider _AlphaSlider;
    [SerializeField] private TMPro.TextMeshProUGUI _AlphaValueText;

    private bool _IsAlreadyInit = false;

    private void Awake()
    {
        if (!_IsAlreadyInit)
        {
            _IsAlreadyInit = true;

            _ScaleSlider.onValueChanged.AddListener(ScaleValueChanged);
            _AlphaSlider.onValueChanged.AddListener(AlphaValueChanged);
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
}

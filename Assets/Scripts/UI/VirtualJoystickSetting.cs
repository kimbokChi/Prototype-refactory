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

    private Image[] _ControllerBtns;

    private void Awake()
    {
        if (!_IsAlreadyInit)
        {
            _IsAlreadyInit = true;

            // ====== _ControllerBtns Init ===== //

            _ControllerBtns = new Image[5];
            var buttons = _Controller.GetAllButtons();

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].TryGetComponent(out _ControllerBtns[i]);
            }
            // ====== _ControllerBtns Init ===== //

            _ScaleSlider.onValueChanged.AddListener(ScaleValueChanged);
            _AlphaSlider.onValueChanged.AddListener(AlphaValueChanged);
        }
    }
    private void ScaleValueChanged(float value)
    {
        _ScaleValueText.text = ((int)value).ToString();
    }
    private void AlphaValueChanged(float value)
    {
        _AlphaValueText.text = ((int)value).ToString();

        var color = _ControllerBtns[0].color;
            color.a = value * 0.01f;
        
        for (int i = 0; i < _ControllerBtns.Length; ++i)
        {
            _ControllerBtns[i].color = color;
        }
    }
}

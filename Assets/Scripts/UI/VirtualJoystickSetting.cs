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
        value *= 0.02f;

        float defScale = value * _ControllerDefScale;
        float maxScale = value * _ControllerMaxScale;

        Vector3 scale = Vector3.one;
                scale.x = scale.y = defScale;

        _Controller.SetButtonScale(defScale, maxScale);

        float offset = _ControllerOffset * value;

        for (Direction d = Direction.Up; (int)d < 4; d++)
        {
            Vector3 position = Vector2.zero;

            switch (d)
            {
                case Direction.Up:
                    position = Vector2.up * offset;
                    break;
                case Direction.Down:
                    position = Vector2.down * offset;
                    break;
                case Direction.Right:
                    position = Vector2.right * offset;
                    break;
                case Direction.Left:
                    position = Vector2.left * offset;
                    break;
            }
            _Controller[d].transform.localScale = scale;
            _Controller[d].transform.localPosition = position;
        }

        _Controller.AttackButton.transform.localScale = scale;
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

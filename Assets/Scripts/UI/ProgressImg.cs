using System;
using UnityEngine;
using UnityEngine.UI;

namespace RealWorldVRGame.UI
{
    public class ProgressImg : MonoBehaviour
    {
        [Range(0, 1f)]
        [SerializeField] private float _currentValue = 0.5f;
        public float CurrentValue
        {
            get => _currentValue;
            set { _currentValue = Mathf.Clamp(value, 0, 1f); }
        }

        [SerializeField] private float _changeSpeed = 6f;
        [SerializeField] private Image _img;
        [SerializeField] private bool _isChangeColor = true;

        private void Update()
        {
            _img.fillAmount = Mathf.Lerp(_img.fillAmount, _currentValue, _changeSpeed * Time.deltaTime);
            if (_isChangeColor)
            {
                var currentImgColor = _img.color;
                _img.color = new Color(
                    _currentValue,
                    _currentValue,
                    _currentValue,
                    _currentValue
                    );
            }
        }
    }
}


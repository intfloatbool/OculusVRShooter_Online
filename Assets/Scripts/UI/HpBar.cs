using System;
using UnityEngine;
using UnityEngine.UI;

namespace RealWorldVRGame
{
    public class HpBar : MonoBehaviour
    {
        [SerializeField] private UnitStats _unitStats;
        [SerializeField] private Image _hpBar;
        [SerializeField] private float _hpChangedSpeed = 5f;
        [SerializeField] private Color _goodHpLvlColor = Color.green;
        [SerializeField] private Color _mediumHpLvlColor = Color.yellow;
        [SerializeField] private Color _lowHpLvlColor = Color.red;
        private int _unitMaxHp;
        private void Start()
        {
            _unitMaxHp = _unitStats.CurrentHp;
        }

        private void Update()
        {
            HandleUnitHp();
        }

        private void HandleUnitHp()
        {
            var hpScale = (float) _unitStats.CurrentHp / (float) _unitMaxHp;
            _hpBar.fillAmount = Mathf.Lerp(_hpBar.fillAmount, hpScale, _hpChangedSpeed * Time.deltaTime);

            var highLevel = 0.7f;
            var mediumLevel = 0.4f;
            var lowLevel = 0.2f;
            if (_hpBar.fillAmount > highLevel)
            {
                ChangeColorLoop(_goodHpLvlColor);
            }
            else if (_hpBar.fillAmount > mediumLevel)
            {
                ChangeColorLoop(_mediumHpLvlColor);
            }
            else if (_hpBar.fillAmount > lowLevel)
            {
                ChangeColorLoop(_lowHpLvlColor);
            }
        }

        private void ChangeColorLoop(Color color)
        {
            _hpBar.color = Color.Lerp(_hpBar.color, color, _hpChangedSpeed * Time.deltaTime);
        }
    }
}

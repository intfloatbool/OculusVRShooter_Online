using System;
using System.Collections;
using System.Collections.Generic;
using RealWorldVRGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugTesting
{
    public class PositionDebugText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPosition;
        [SerializeField] private TextMeshProUGUI _textMeshEuler;
        [SerializeField] private TextMeshProUGUI _currentVrBtnText;
        [SerializeField] private Transform _trackingTransform;

        private void Update()
        {
            if(_textMeshPosition != null)
                _textMeshPosition.text = _trackingTransform.position.ToString();
            if(_textMeshEuler != null)
                _textMeshEuler.text = _trackingTransform.eulerAngles.ToString();

            HandleVrInputBtnText();
        }
        private void HandleVrInputBtnText()
        {
            var btnType = Enum.GetNames(typeof(OVRInput.Button));
            var controllerType = Enum.GetNames(typeof(OVRInput.Controller));
            for (int i = 0; i < btnType.Length; i++)
            {
                for (int j = 0; j < controllerType.Length; j++)
                {
                    var btn = (OVRInput.Button) i;
                    var controller = (OVRInput.Controller) j;
                    
                    if (OVRInput.GetDown(btn, controller))
                    {
                        _currentVrBtnText.text = $"BTN: {btn.ToString()}, \n CTRL: {controller.ToString()}";
                    }
                }
            }
            
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame.Effects
{
    public class DelayDisable : MonoBehaviour
    {
        [SerializeField] private float _delayDisable = 2f;
        private WaitForSeconds _waiting;
        private void OnEnable()
        {
            if (_waiting == null)
            {
                _waiting = new WaitForSeconds(_delayDisable);
            }

            StartCoroutine(DelayDisableCoroutine());
        }

        private IEnumerator DelayDisableCoroutine()
        {
            yield return _waiting;
            gameObject.SetActive(false);
        }
    }

}

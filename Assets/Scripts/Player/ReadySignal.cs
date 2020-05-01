using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    public class ReadySignal : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _alertClip;
        [SerializeField] private AutoCalibrate _autoCalibrate;
        private IEnumerator Start()
        {
            var isReferencesReady = _audioSource != null &&
                _alertClip != null && _autoCalibrate != null;
            if(isReferencesReady)
            {
                while(!_autoCalibrate.IsCalibrated)
                {
                    _audioSource.PlayOneShot(_alertClip);
                    yield return new WaitForSeconds(_alertClip.length);
                }
            }
            else
            {
                yield break;
            }
        }
    }

}

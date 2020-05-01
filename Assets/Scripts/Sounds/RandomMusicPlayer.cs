using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RealWorldVRGame
{
    public class RandomMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _musics;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _isEnabled = true;

        private IEnumerator Start()
        {
            while (_isEnabled)
            {
                var rndClip = _musics[Random.Range(0, _musics.Length)];
                var delay = 1f;
                if (rndClip != null)
                {
                    delay = rndClip.length;
                    if (_audioSource != null)
                    {
                        _audioSource.clip = rndClip;
                        _audioSource.Play();
                    }
                }
                yield return new WaitForSeconds(delay);
                
            }
        }
    }
}

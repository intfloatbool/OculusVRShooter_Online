using Passer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    [RequireComponent(typeof(HumanoidControl))]
    public class HumanoidExternalProperties : MonoBehaviour
    {
        private HumanoidControl _humanoidControl;
        [SerializeField] private bool _isTranslateByPhysics;
        private void Awake()
        {
            _humanoidControl = GetComponent<HumanoidControl>();

            _humanoidControl.IsTranslateByPhysics = _isTranslateByPhysics;
        }
    }

}

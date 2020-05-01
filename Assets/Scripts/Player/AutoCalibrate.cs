using System;
using System.Collections;
using System.Collections.Generic;
using Passer;
using Passer.Humanoid.Tracking;
using UnityEngine;

namespace RealWorldVRGame
{
    public class AutoCalibrate : MonoBehaviour
    {
        [SerializeField] private HumanoidControl _humanoid;
        [SerializeField] private bool _isTracking;
        [SerializeField] private KeyCode _debugKeyToCalibrate = KeyCode.C;
        [SerializeField] private bool _isCalibrated = false;
        public bool IsCalibrated => _isCalibrated;

        public void Update() {
            
            if (OVRInput.GetDown(OVRInput.RawButton.B) //B button
                || Input.GetKeyDown(_debugKeyToCalibrate))
            {
                _humanoid.Calibrate();
                _isCalibrated = true;
            }
        }

        private void AutoCalibrateControl()
        {
#if hOCULUS
            if (_humanoid.headTarget.oculus.status == Status.Tracking) {
                if (!_isTracking) {
// Tracking starts
                    _humanoid.Calibrate();
                }
                _isTracking = true;
            }
            else if (_humanoid.headTarget.oculus.status == Status.Present)
                _isTracking = false;
#endif
        }
    }
}

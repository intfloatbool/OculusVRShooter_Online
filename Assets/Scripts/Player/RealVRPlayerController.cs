using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    public class RealVRPlayerController : SimpleCapsuleWithStickMovement
    {
		[SerializeField] protected bool _isMoveFromEditor = true;
		private readonly string VerticalAxisName = "Vertical";
		private readonly string HorizontalAxisName = "Horizontal";

		protected override void RotatePlayerToHMD()
		{
			Transform root = CameraRig.trackingSpace;
			Transform centerEye = CameraRig.centerEyeAnchor;

			Vector3 prevPos = root.position;
			Quaternion prevRot = root.rotation;

			transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

			root.position = prevPos;
			root.rotation = prevRot;
		}
		protected override void StickMovement()
		{
			Quaternion ort = CameraRig.centerEyeAnchor.rotation;
			Vector3 ortEuler = ort.eulerAngles;
			ortEuler.z = ortEuler.x = 0f;
			ort = Quaternion.Euler(ortEuler);

			Vector3 moveDir = Vector3.zero;
			Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
			if (_isMoveFromEditor)
			{
				primaryAxis = new Vector2
				{
					x = Input.GetAxis(HorizontalAxisName),
					y = Input.GetAxis(VerticalAxisName)
				};
			}
			moveDir += ort * (primaryAxis.x * Vector3.right);
			moveDir += ort * (primaryAxis.y * Vector3.forward);

			_rigidbody.MovePosition(_rigidbody.position + moveDir * Speed * Time.fixedDeltaTime);
		}

		protected override void SnapTurn()
		{
			Vector3 euler = transform.rotation.eulerAngles;

			if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
				(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
				||
				Input.GetKeyDown(KeyCode.Q))
			{
				if (ReadyToSnapTurn)
				{
					euler.y -= RotationAngle;
					ReadyToSnapTurn = false;
				}
			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
				(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
				||
				Input.GetKeyDown(KeyCode.E))
			{
				if (ReadyToSnapTurn)
				{
					euler.y += RotationAngle;
					ReadyToSnapTurn = false;
				}
			}
			else
			{
				ReadyToSnapTurn = true;
			}

			transform.rotation = Quaternion.Euler(euler);
		}
	} 
}


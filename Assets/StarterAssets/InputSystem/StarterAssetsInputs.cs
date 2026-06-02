using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;

        public void Initialize()
        {
            move = new Vector2(0, 0);
            //look = new Vector2(0, 0);
            jump = false;
            sprint = false;
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
        }

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
            }
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
        }

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
        }
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			if (CameraManager.Instance.IsBlending)
				return;

			move = newMoveDirection;
        } 

		public void LookInput(Vector2 newLookDirection)
		{
            if (CameraManager.Instance.IsBlending)
                return;

            look = newLookDirection;
        }

		public void JumpInput(bool newJumpState)
		{
            if (CameraManager.Instance.IsBlending)
                return;

            jump = newJumpState;
        }

		public void SprintInput(bool newSprintState)
		{
            if (CameraManager.Instance.IsBlending)
                return;

            sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
            MouseManager.Instance.SetCursorLockState(true);
		}
    }
}
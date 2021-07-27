using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
	public class InputsHandler : MonoBehaviour
	{
		public Vector2 move;
		public bool isMove;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool aim;

		[Header("Movement Settings")]
		public bool analogMovement;
		private GameObject _chiusky;
		private GameObject _gameManager;
		private GameController _gameController;
		private ChiuskyController _chiuskyController;

		private void Awake()
		{
			if (_chiusky == null)
			{
				_chiusky = GameObject.FindGameObjectWithTag("Chiusky");
			}
			
			if (_gameManager == null)
			{
				_gameManager = GameObject.FindGameObjectWithTag("GameManager");
			}
		}
		private void Start()
		{
			_chiuskyController = _chiusky.GetComponent<ChiuskyController>();
			_gameController = _gameManager.GetComponent<GameController>();
		}

		public void OnMove(InputValue value)
		{
			isMove = !Vector2.zero.Equals(value.Get<Vector2>());
			MoveInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		
		public void OnInteract(InputValue value)
		{
			if (value.isPressed)
			{
				_chiuskyController.HandleInteract(value);
			}
		}
		
		public void OnPause(InputValue value)
		{
			if (value.isPressed)
			{
				_gameController.HandlePause(value);
			}
		}
		
		public void OnAim(InputValue value)
		{
			_chiuskyController.HandleAim(value);
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newState)
		{
			jump = newState;
		}

		public void SprintInput(bool newState)
		{
			sprint = newState;
		}
	}
	
}
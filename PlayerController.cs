using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class KeyBindings
{
	public KeyCode Jump = KeyCode.Space;
	public KeyCode Attack = KeyCode.Mouse0;
	public string HorizontalAxis = "Horizontal";
}

/* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

[System.Serializable]
public class PlayerInput {

	public KeyBindings KeyBindings;
    public float MoveAxisDeadZone = 0.2f;

	public Vector2 MoveInput { get; private set; }
	public Vector2 LastMoveInput { get; private set; }
	public bool JumpInput { get; private set; }
    public bool HasMoveInput { get; private set; }

    public void UpdateInput()
	{
        Vector2 moveInput = new Vector2(Input.GetAxis(KeyBindings.HorizontalAxis), 0);
		if (Mathf.Abs(moveInput.x) < MoveAxisDeadZone)
		{
			moveInput.x = 0.0f;
		}

        bool hasMoveInput = moveInput.sqrMagnitude > 0.0f;

		if (HasMoveInput && !hasMoveInput)
		{
			LastMoveInput = MoveInput;
		}

        MoveInput = moveInput;
		HasMoveInput = hasMoveInput;

        JumpInput = Input.GetKey(KeyBindings.Jump);
    }
}

/* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

[System.Serializable]
public class MovementSettings {
    public float Acceleration = 25.0f; // In meters/second
	public float Decceleration = 25.0f; // In meters/second
	public float MaxHorizontalSpeed = 8.0f; // In meters/second
	public float JumpSpeed = 10.0f; // In meters/second
	public float JumpAbortSpeed = 10.0f; // In meters/second
}

/* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

[System.Serializable]
public class GravitySettings
{
	public float Gravity = 20.0f; // Gravity applied when the player is airborne
	public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
	public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
}

/* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public MovementSettings _movementSettings;
    public PlayerInput _playerInput;
    public GravitySettings _gravitySettings;
    private CharacterController _characterController;
    private PlayerAnimator _playerAnimator;

    private float _targetHorizontalSpeed; // In meters/second
	private float _horizontalSpeed; // In meters/second
	private float _verticalSpeed; // In meters/second

	private Vector2 _controlRotation; // X (Pitch), Y (Yaw)
	private Vector3 _movementInput;
	private Vector3 _lastMovementInput;
    private bool _hasMovementInput;
	private bool _jumpInput;

    public Vector3 Velocity => _characterController.velocity;
	public Vector3 HorizontalVelocity => _characterController.velocity.SetY(0.0f);
	public Vector3 VerticalVelocity => _characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);
	public bool IsGrounded { get; private set; }


    // Start is called before the first frame update
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    private void Update()
    {
        _playerInput.UpdateInput();
        SetJumpInput(_playerInput.JumpInput);

        UpdateHorizontalSpeed();
        UpdateVerticalSpeed();
        SetMovementInput(GetMovementInput());

        Vector3 movement = _horizontalSpeed * GetMovementDirection() + _verticalSpeed * Vector3.up;

        if(movement.x < 0){
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * 10);
        }else if(movement.x > 0){
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 10);
        }
		_characterController.Move(movement * Time.deltaTime);

        IsGrounded = _characterController.isGrounded;

        _playerAnimator.UpdateStates();
    }

    private Vector3 GetMovementInput(){
        Vector3 movementInput = Vector3.right * _playerInput.MoveInput.x;

		if (movementInput.sqrMagnitude > 1f)
		{
			movementInput.Normalize();
		}

		return movementInput;
    }

    void SetMovementInput(Vector3 movementInput){
        bool hasMovementInput = movementInput.sqrMagnitude > 0.0f;

		if (_hasMovementInput && !hasMovementInput)
		{
			_lastMovementInput = _movementInput;
		}

		_movementInput = movementInput;
		_hasMovementInput = hasMovementInput;
    }

    private Vector3 GetMovementDirection()
	{
		Vector3 moveDir = _hasMovementInput ? _movementInput : _lastMovementInput;
		if (moveDir.sqrMagnitude > 1f)
		{
			moveDir.Normalize();
		}
		return moveDir;
	}

    private void UpdateHorizontalSpeed()
	{
		Vector3 movementInput = _movementInput;
		if (movementInput.sqrMagnitude > 1.0f)
		{
			movementInput.Normalize();
		}

		_targetHorizontalSpeed = movementInput.magnitude * _movementSettings.MaxHorizontalSpeed;
		float acceleration = _hasMovementInput ? _movementSettings.Acceleration : _movementSettings.Decceleration;

		_horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
	}

    private void UpdateVerticalSpeed()
	{
		if (IsGrounded)
		{
		    _verticalSpeed = -_gravitySettings.GroundedGravity;

			if (_jumpInput)
			{
				_verticalSpeed = _movementSettings.JumpSpeed;
				IsGrounded = false;
			}
		} 
		else
		{
			if (!_jumpInput && _verticalSpeed > 0.0f)
			{
				// This is what causes holding jump to jump higher than tapping jump.
				_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -_gravitySettings.MaxFallSpeed, _movementSettings.JumpAbortSpeed * Time.deltaTime);
			}
			_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -_gravitySettings.MaxFallSpeed, _gravitySettings.Gravity * Time.deltaTime);
		}
	}

    public void SetJumpInput(bool jumpInput)
	{
		_jumpInput = jumpInput;
	}
}

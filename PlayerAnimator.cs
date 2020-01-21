using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CharacterAnimatorParamId
{
	public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
	public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
	public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
}



[System.Serializable]
public class CharacterIK 
{
	public Transform LeftHand;
	public Transform RightHand;
	public Transform LeftFoot;
	public Transform RightFoot;
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
	public CharacterIK CharacterIK;
    	private Animator _animator;
	private PlayerController _playerController;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_playerController = GetComponent<PlayerController>();
	}

    // Update is called once per frame
    public void UpdateStates()
    {
        float normHorizontalSpeed = _playerController.HorizontalVelocity.magnitude / _playerController._movementSettings.MaxHorizontalSpeed;
		_animator.SetFloat(CharacterAnimatorParamId.HorizontalSpeed, normHorizontalSpeed);

		float jumpSpeed = _playerController._movementSettings.JumpSpeed;
		float normVerticalSpeed = _playerController.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);
		_animator.SetFloat(CharacterAnimatorParamId.VerticalSpeed, normVerticalSpeed);

		_animator.SetBool(CharacterAnimatorParamId.IsGrounded, _playerController.IsGrounded);

    }

	void OnAnimatorIK()
	{
		//Right Hand
		if(CharacterIK.RightHand != null){
			_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
			_animator.SetIKPosition(AvatarIKGoal.RightHand, CharacterIK.RightHand.position);

			_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
			_animator.SetIKRotation(AvatarIKGoal.RightHand, CharacterIK.RightHand.rotation);
		}

		//Left Hand
		if(CharacterIK.LeftHand != null){
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
			_animator.SetIKPosition(AvatarIKGoal.LeftHand, CharacterIK.LeftHand.position);

			_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
			_animator.SetIKRotation(AvatarIKGoal.LeftHand, CharacterIK.LeftHand.rotation);
		}

		//Right Foot
		if(CharacterIK.RightFoot != null){
			_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
			_animator.SetIKPosition(AvatarIKGoal.RightFoot, CharacterIK.RightFoot.position);

			_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
			_animator.SetIKRotation(AvatarIKGoal.RightFoot, CharacterIK.RightFoot.rotation);
		}

		//Left Foot
		if(CharacterIK.LeftFoot != null){
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
			_animator.SetIKPosition(AvatarIKGoal.LeftFoot, CharacterIK.LeftFoot.position);

			_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
			_animator.SetIKRotation(AvatarIKGoal.LeftFoot, CharacterIK.LeftFoot.rotation);
		}
	}

	/// <summary>
	/// Draws Gizmos onto the IK Target transforms
	/// </summary>
	void OnDrawGizmos()
	{
		if(CharacterIK.RightHand != null){
			Gizmos.DrawWireSphere(CharacterIK.RightHand.position, .1f);
		}

		if(CharacterIK.LeftHand != null){
			Gizmos.DrawWireSphere(CharacterIK.LeftHand.position, .1f);
		}

		if(CharacterIK.RightFoot != null){
			Gizmos.DrawWireSphere(CharacterIK.RightFoot.position, .1f);
		}

		if(CharacterIK.LeftFoot != null){
			Gizmos.DrawWireSphere(CharacterIK.LeftFoot.position, .1f);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NetworkCharacterOrientation3D : CharacterAbility
{
    [SerializeField] private float RotationSpeed = 10f;
    [SerializeField] private RotationMode rotationMode;
    private Quaternion _lastRotation;
    private RotationMode currentRotationMode;
    private CharacterHandleWeapon _characterHandleWeapon;
    private InputManager InputManager => _character.LinkedInputManager;
    [Networked] private  Quaternion _targetRotation { get; set; }

    private enum RotationMode
    {
        None,
        FaceMovement,
        FaceWeapon,
        Both
    }
    
    
    public override void Spawned()
    {
        _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
    }

    public override void FixedUpdateNetwork()
    {
        if(rotationMode == RotationMode.None) return;
            if(_controller3D == null) return;
        
        currentRotationMode = rotationMode;

        //Character won't rotate to face weapon if it's not equipped
        if (currentRotationMode is RotationMode.FaceWeapon or RotationMode.Both)
        {
            RotateToFaceWeapon();
        }
        
        
        //Check if character has a weapon
        if (currentRotationMode is RotationMode.FaceMovement or RotationMode.Both)
        {
            RotateToFaceMovement();
        }
        
    }
    
    private void RotateToFaceWeapon()
    {
        if(_characterHandleWeapon == null) return;
        if(_characterHandleWeapon.CurrentWeapon == null) return;
        if(_characterHandleWeapon.WeaponAimComponent== null) return;
        
        var aim = _characterHandleWeapon.WeaponAimComponent.CurrentAim;
        
        _targetRotation = Quaternion.LookRotation(aim);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, RunnerDeltaTime * RotationSpeed);
        
        currentRotationMode = RotationMode.FaceWeapon;
    }

    
    //Rotate the model to face movement direction
    private void RotateToFaceMovement()
    {
        if (_controller3D.Velocity.sqrMagnitude < 1f)
        {
            transform.rotation =  Quaternion.Slerp(transform.rotation, _lastRotation, RunnerDeltaTime * RotationSpeed);
            return;
        }
        
        var speed = _controller3D.Velocity;
        var movementDirection = speed.normalized;
        movementDirection.y = 0f;
        
        //Smoothly rotate to face movement direction
        var targetRotation = Quaternion.LookRotation(movementDirection);
        var rotation = transform.rotation;
        rotation =
            Quaternion.Lerp(rotation, targetRotation, RunnerDeltaTime * 10f);
        transform.rotation = rotation;
        _lastRotation = rotation;
        
        
        currentRotationMode = RotationMode.FaceMovement;
    }
}



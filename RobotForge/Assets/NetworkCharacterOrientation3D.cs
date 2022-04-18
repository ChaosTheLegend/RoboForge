using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NetworkCharacterOrientation3D : CharacterAbility
{
    [SerializeField] private GameObject ModelToRotate;
    [SerializeField] private float RotationSpeed = 10f;
    [SerializeField] private RotationMode rotationMode;
    private Quaternion _lastRotation;
    
    private CharacterHandleWeapon _characterHandleWeapon;
    
    [Networked] private  Quaternion _targetRotation { get; set; }
    
    private Vector3 _aim;
    private RotationMode currentRotationMode;
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
        
        if (ModelToRotate == null) return;
        if(_controller3D == null) return;
        
        currentRotationMode = RotationMode.None;
        
        //Character won't rotate to face weapon if it's not equipped
        if(rotationMode is RotationMode.FaceWeapon or RotationMode.Both) RotateToFaceWeapon();
        if(currentRotationMode == RotationMode.FaceWeapon) return;
        
        //Check if character has a weapon
        if(rotationMode is RotationMode.FaceMovement or RotationMode.Both) RotateToFaceMovement();
        
    }
    
    private void RotateToFaceWeapon()
    {
        if(_characterHandleWeapon == null) return;
        var weapon = _characterHandleWeapon.CurrentWeapon;
        if (weapon == null) return;
        
        //Smoothly rotate towards the aim
        var aim = _characterHandleWeapon.WeaponAimComponent.CurrentAim;
        aim.y = 0;
        _targetRotation = Quaternion.LookRotation(aim);
        ModelToRotate.transform.rotation = Quaternion.Slerp(ModelToRotate.transform.rotation, _targetRotation, RunnerDeltaTime * RotationSpeed);
        currentRotationMode = RotationMode.FaceWeapon;
    }

    
    //Rotate the model to face movement direction
    private void RotateToFaceMovement()
    {
        if (_controller3D.Velocity.sqrMagnitude < 1f)
        {
            ModelToRotate.transform.rotation =  Quaternion.Slerp(ModelToRotate.transform.rotation, _lastRotation, RunnerDeltaTime * RotationSpeed);
            return;
        }
        
        var speed = _controller3D.Velocity;
        var movementDirection = speed.normalized;
        movementDirection.y = 0f;
        
        //Smoothly rotate to face movement direction
        var targetRotation = Quaternion.LookRotation(movementDirection);
        var rotation = ModelToRotate.transform.rotation;
        rotation =
            Quaternion.Lerp(rotation, targetRotation, RunnerDeltaTime * 10f);
        ModelToRotate.transform.rotation = rotation;
        _lastRotation = rotation;

        currentRotationMode = RotationMode.FaceMovement;
    }
}



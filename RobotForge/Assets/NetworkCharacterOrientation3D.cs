using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NetworkCharacterOrientation3D : CharacterAbility
{
    [SerializeField] private GameObject ModelToRotate;

    private Quaternion _lastRotation;
    
    public override void FixedUpdateNetwork()
    {
        RotateToFaceMovement();
    }


    //TODO : implement this
    private void RotateToFaceWeapon()
    {
        
    }

    
    //Rotate the model to face movement direction
    private void RotateToFaceMovement()
    {
        if (ModelToRotate == null) return;
        if(_controller3D == null) return;
        if (_controller3D.Velocity.sqrMagnitude < 1f)
        {
            ModelToRotate.transform.rotation =  Quaternion.Lerp(ModelToRotate.transform.rotation, _lastRotation, RunnerDeltaTime * 10f);
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
    }
    
    
}

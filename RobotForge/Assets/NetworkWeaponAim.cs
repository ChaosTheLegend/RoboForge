using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetworkWeaponAim : WeaponAim
{
    private InputManager InputManager => _weapon.Owner.LinkedInputManager;
    private Vector3 _weaponAim;
    protected override void Initialization()
    {
        base.Initialization();
    }

    public override void FixedUpdateNetwork()
    {
        RotateToMouse();
    }
    
    private void RotateToMouse()
    {
        var mousePos = InputManager.MousePosition;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        float distance;
        if (_playerPlane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            _direction = target;
        }

        _direction.y = 0; 
        _currentAim = _direction - _weapon.Owner.transform.position;
        _weaponAim = _direction - transform.position;
        _weaponAim.y = 0;
        var targetRotation = Quaternion.LookRotation(_weaponAim);
        transform.rotation = targetRotation;
    }
    
    protected override void InitializeReticle()
    {
        
    }
}

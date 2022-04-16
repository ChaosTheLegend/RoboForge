using System;
using System.Collections.Generic;
using Fusion;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Network
{
    public struct NetworkInputData : INetworkInput
    {
        //Buttons
        public MMInput.ButtonStates Jump;
        public MMInput.ButtonStates Run;
        public MMInput.ButtonStates Interact;
        public MMInput.ButtonStates Dash;
        public MMInput.ButtonStates Crouch;
        public MMInput.ButtonStates Shoot;
        public MMInput.ButtonStates SecondaryShoot;
        public MMInput.ButtonStates Reload;
        public MMInput.ButtonStates SwitchWeapon;
        public MMInput.ButtonStates Pause;
        public MMInput.ButtonStates TimeControl;
        public MMInput.ButtonStates SwitchCharacter;
        
        
        public bool ProcessedButtons;
        
        //axis
        public Vector2 PrimaryMovement; 
        public bool ProcessedMovement;
        
        public Vector2 SecondaryMovement;
        public bool ProcessedSecondaryMovement;
    }
    
    public class NetworkInputManager : InputManager 
    {
        protected override void Start()
        {
            Initialization();
        }

        public void InitializeManually() => Initialization();

        protected override void Initialization()
        {
            base.Initialization();
            _axisCamera = "Player1_CameraRotationAxis";
        }

        public void TriggerButtons(NetworkInputData InputData)
        {
            _primaryMovement = InputData.PrimaryMovement;
            _secondaryMovement = SecondaryMovement;
            
            UpdateButton(InputData.Jump, JumpButton);
            UpdateButton(InputData.Run, RunButton);
            UpdateButton(InputData.Dash, DashButton);
            UpdateButton(InputData.Crouch, CrouchButton);
            UpdateButton(InputData.Shoot, ShootButton);
            UpdateButton(InputData.SecondaryShoot, SecondaryShootButton);
            UpdateButton(InputData.Interact, InteractButton);
            UpdateButton(InputData.Reload, ReloadButton);
            UpdateButton(InputData.Pause, PauseButton);
            UpdateButton(InputData.SwitchWeapon, SwitchWeaponButton);
            UpdateButton(InputData.SwitchCharacter, SwitchCharacterButton);
            UpdateButton(InputData.TimeControl, TimeControlButton);
        }
        
        protected virtual void UpdateButton(MMInput.ButtonStates buttonState, MMInput.IMButton imButton)
        {
            imButton.State.ChangeState(buttonState);
        }

        protected override void GetInputButtons()
        {
            // useless now
        }

        public override void SetMovement()
        {
            //do nothing
        }

        public override void SetSecondaryMovement()
        {
            //do nothing
        }

        protected override void SetShootAxis()
        {
            //do nothing
        }
    }
}

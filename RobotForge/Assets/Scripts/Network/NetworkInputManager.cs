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
        public float Horizontal;
        public float Vertical;
        public bool ProcessedMovement;
        
        public float SecondaryHorizontal;
        public float SecondaryVertical;
        public bool ProcessedSecondaryMovement;
        
        public float ShootAxis;
        public bool ProcessedShootAxis;

        
        public float SecondaryShootAxis;
        public bool ProcessedSecondaryShootAxis;
    }
    
    public class NetworkInputManager : InputManager 
    {
        public NetworkInputData inputData;


        protected override void Start()
        {
            Initialization();
        }

        public void InitializeManually() => Initialization();

        protected override void GetInputButtons()
        {
            if (inputData.ProcessedButtons) return;
            
            foreach (var btn in ButtonList)
            {
                if (btn.ButtonID == $"{PlayerID}_Jump")
                    TriggerButtonState(btn, inputData.Jump);
                else if (btn.ButtonID == $"{PlayerID}_Run")
                    TriggerButtonState(btn, inputData.Run);
                else if (btn.ButtonID == $"{PlayerID}_Interact")
                    TriggerButtonState(btn, inputData.Interact);
                else if (btn.ButtonID == $"{PlayerID}_Dash")
                    TriggerButtonState(btn, inputData.Dash);
                else if (btn.ButtonID == $"{PlayerID}_Crouch")
                    TriggerButtonState(btn, inputData.Crouch);
                else if (btn.ButtonID == $"{PlayerID}_Shoot")
                    TriggerButtonState(btn, inputData.Shoot);
                else if (btn.ButtonID == $"{PlayerID}_SecondaryShoot")
                    TriggerButtonState(btn, inputData.SecondaryShoot);
                else if (btn.ButtonID == $"{PlayerID}_Reload")
                    TriggerButtonState(btn, inputData.Reload);
                else if (btn.ButtonID == $"{PlayerID}_SwitchWeapon")
                    TriggerButtonState(btn, inputData.SwitchWeapon);
                else if (btn.ButtonID == $"{PlayerID}_Pause")
                    TriggerButtonState(btn, inputData.Pause);
                else if (btn.ButtonID == $"{PlayerID}_TimeControl")
                    TriggerButtonState(btn, inputData.TimeControl);
                else if (btn.ButtonID == $"{PlayerID}_SwitchCharacter") TriggerButtonState(btn, inputData.SwitchCharacter);
            }

            inputData.ProcessedButtons = true;
        }

        protected override void InitializeButtons()
        {
            var prefix = PlayerID;
            ButtonList = new List<MMInput.IMButton> ();
            ButtonList.Add(JumpButton = new MMInput.IMButton (prefix, "Jump", JumpButtonDown, JumpButtonPressed, JumpButtonUp));
            ButtonList.Add(RunButton  = new MMInput.IMButton (prefix, "Run", RunButtonDown, RunButtonPressed, RunButtonUp));
            ButtonList.Add(InteractButton = new MMInput.IMButton(prefix, "Interact", InteractButtonDown, InteractButtonPressed, InteractButtonUp));
            ButtonList.Add(DashButton  = new MMInput.IMButton (prefix, "Dash", DashButtonDown, DashButtonPressed, DashButtonUp));
            ButtonList.Add(CrouchButton  = new MMInput.IMButton (prefix, "Crouch", CrouchButtonDown, CrouchButtonPressed, CrouchButtonUp));
            ButtonList.Add(SecondaryShootButton = new MMInput.IMButton(prefix, "SecondaryShoot", SecondaryShootButtonDown, SecondaryShootButtonPressed, SecondaryShootButtonUp));
            ButtonList.Add(ShootButton = new MMInput.IMButton (prefix, "Shoot", ShootButtonDown, ShootButtonPressed, ShootButtonUp)); 
            ButtonList.Add(ReloadButton = new MMInput.IMButton (prefix, "Reload", ReloadButtonDown, ReloadButtonPressed, ReloadButtonUp));
            ButtonList.Add(SwitchWeaponButton = new MMInput.IMButton (prefix, "SwitchWeapon", SwitchWeaponButtonDown, SwitchWeaponButtonPressed, SwitchWeaponButtonUp));
            ButtonList.Add(PauseButton = new MMInput.IMButton(prefix, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp));
            ButtonList.Add(TimeControlButton = new MMInput.IMButton(prefix, "TimeControl", TimeControlButtonDown, TimeControlButtonPressed, TimeControlButtonUp));
            ButtonList.Add(SwitchCharacterButton = new MMInput.IMButton(prefix, "SwitchCharacter", SwitchCharacterButtonDown, SwitchCharacterButtonPressed, SwitchCharacterButtonUp));
        }
        
        //Process only camera in update, nobody cares about this shit
        protected override void Update()
        { 
            SetCameraRotationAxis();
        }

        public void ProcessButtons(NetworkInputData inputData)
        {
            this.inputData = inputData;
            ProcessButtons();
        }
        
        public void ProcessButtons()
        {
            SetMovement();	
            SetSecondaryMovement ();
            SetShootAxis ();
            GetInputButtons ();
            GetLastNonNullValues();
            
        }

        private void TriggerButtonState(MMInput.IMButton button, MMInput.ButtonStates state)
        {
            switch (state)
            {
                case MMInput.ButtonStates.Off:
                    break;
                case MMInput.ButtonStates.ButtonDown:
                    button.TriggerButtonDown();
                    break;
                case MMInput.ButtonStates.ButtonPressed:
                    button.TriggerButtonPressed();
                    break;
                case MMInput.ButtonStates.ButtonUp:
                    button.TriggerButtonUp();
                    break;
            }
        }

        //Do not process button states
        public override void ProcessButtonStates() { }


        protected override void InitializeAxis()
        {
            var prefix = "Player1";
            _axisHorizontal = prefix+"_Horizontal";
            _axisVertical = prefix+"_Vertical";
            _axisSecondaryHorizontal = prefix+"_SecondaryHorizontal";
            _axisSecondaryVertical = prefix+"_SecondaryVertical";
            _axisShoot = prefix+"_ShootAxis";
            _axisShootSecondary = prefix + "_SecondaryShootAxis";
            _axisCamera = prefix + "_CameraRotationAxis";
        }

        public override void SetMovement()
        {
            if (!IsMobile || inputData.ProcessedMovement) return;
            
            _primaryMovement.x = inputData.Horizontal;
            _primaryMovement.y = inputData.Vertical;
        
            _primaryMovement = ApplyCameraRotation(_primaryMovement);

            inputData.ProcessedMovement = true;
        }

        public override void SetSecondaryMovement()
        {
            if (!IsMobile || inputData.ProcessedSecondaryMovement) return;

            _secondaryMovement.x = inputData.SecondaryHorizontal;
            _secondaryMovement.y = inputData.SecondaryVertical;		
        
            _secondaryMovement = ApplyCameraRotation(_secondaryMovement);

            inputData.ProcessedSecondaryMovement = false;
        }

        protected override void SetShootAxis()
        {
            if (IsMobile) return;

            if (inputData.ProcessedShootAxis)
            {
                ShootAxis = MMInput.ProcessAxisAsButton(inputData.ShootAxis, Threshold.y, ShootAxis);
                inputData.ProcessedShootAxis = true;
            }

            if (inputData.ProcessedSecondaryShootAxis)
            {
                SecondaryShootAxis = MMInput.ProcessAxisAsButton(inputData.SecondaryShootAxis, Threshold.y, SecondaryShootAxis, MMInput.AxisTypes.Positive);
                inputData.ProcessedSecondaryShootAxis = true;
            }
        }
    }
}

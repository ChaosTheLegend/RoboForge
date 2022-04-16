using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Network
{
    public class NetworkCharacter : Character
    {
        [SerializeField] private InputManager linkedInputManager;
     
    
        //Connected child input manager
        public override void SetInputManager()
        {
            if (CharacterType == CharacterTypes.AI)
            {
                LinkedInputManager = null;
                UpdateInputManagersInAbilities();
                return;
            }
        
            LinkedInputManager = linkedInputManager;
            UpdateInputManagersInAbilities();
        }
    }
}

using Fusion;

namespace Network
{
    public class NetworkInputPreprocessor : NetworkBehaviour
    {
        private NetworkInputManager inputManager;
        public override void Spawned()
        {
            inputManager = GetComponent<NetworkInputManager>();
            inputManager.InitializeManually();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                inputManager.TriggerButtons(data);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Scripts.Controllers
{
    public class CamController : MonoBehaviour
    {
        public PlayerInput playerInput;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            DebugHelper.DebugLookInputValues(playerInput);
        }
    }
}

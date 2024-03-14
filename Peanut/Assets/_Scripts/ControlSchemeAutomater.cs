using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Scripts
{
    public class ControlSchemeAutomater : MonoBehaviour
    {
        public PlayerInput PlayerInput;
        public string KeyboardAndMouseScheme = "Keyboard&Mouse";
        public string GamepadScheme = "Gamepad";

        void Start()
        {
            // Subscribe to input device connection event
            InputSystem.onDeviceChange += OnInputDeviceChange;

            // Check and switch to appropriate control scheme at start
            SwitchControlScheme();
        }

        void OnDestroy()
        {
            // Unsubscribe from input device connection event
            InputSystem.onDeviceChange -= OnInputDeviceChange;
        }

        void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
        {
            // Check if the changed device is a gamepad
            if (device != null && device is Gamepad)
            {
                // Automatically switch control scheme when gamepad is connected or disconnected
                SwitchControlScheme();
            }
        }

        void SwitchControlScheme()
        {
            if (IsGamepadConnected())
            {
                Debug.Log("YIPPEE");
                // Switch to gamepad control scheme
                PlayerInput.SwitchCurrentControlScheme(GamepadScheme);
            }
            else
            {
                Debug.Log("OUWWW");
                // Switch to keyboard and mouse control scheme
                PlayerInput.SwitchCurrentControlScheme(KeyboardAndMouseScheme);
            }
        }

        bool IsGamepadConnected()
        {
            // Check if any gamepad is currently connected
            var gamepads = Gamepad.all;
            return gamepads.Count > 0;
        }
    }
}

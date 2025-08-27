using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private PlayerController playerController;

    public float HorizontalInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        playerController = new PlayerController();
        playerController.Player.Jump.started += ctx => JumpPressed = true;
        playerController.Player.Interact.started += ctx => InteractPressed = true;
    }

    private void OnEnable()
    {
        playerController.Player.Enable();
    }

    private void OnDisable()
    {
        playerController.Player.Disable();
        playerController.Player.Jump.started -= ctx => JumpPressed = true;
        playerController.Player.Interact.started -= ctx => InteractPressed = true;
    }

    private void Update()
    {
        // === PERBAIKAN DI SINI ===
        // 1. Baca input sebagai tipe data aslinya, yaitu Vector2.
        Vector2 moveInput = playerController.Player.Move.ReadValue<Vector2>();

        // 2. Ambil komponen .x nya saja untuk gerakan horizontal.
        HorizontalInput = moveInput.x;
        // === SELESAI PERBAIKAN ===

        IsRunning = playerController.Player.Run.IsPressed();
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
    }
}


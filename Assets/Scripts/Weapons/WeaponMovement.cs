using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Statistics))]
public class WeaponMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(0f, 45f)] private float tiltAngle = 25f;
    [SerializeField, Range(0f, 50f)] private float dashSpeed = 2f;
    [SerializeField, Range(0f, 1f)] private float acceleration = 0.1f;
    [SerializeField, Range(0.1f, 5f)] private float dashCooldown = 1.5f;

    private CharacterController controller;
    private Keyboard keyboard;
    private Vector3 initialPosition;
    private float speed;
    private float tilt = 0f;
    private Vector3 velocity = Vector3.zero;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private const float dashDuration = 0.3f;
    private float lastDashTime = -999f;
    private float dashAnimAngle = 0f;

    private Statistics stats;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        keyboard = Keyboard.current;
        initialPosition = transform.localPosition;
        stats = GetComponent<Statistics>();
    }

    private void Start()
    {
        speed = stats.GetStatistic(StatisticsType.Speed);

    }
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        HandleMovementAndAnimation();
    }

    private void HandleMovementAndAnimation()
    {
        // Input direction
        Vector3 inputDir = Vector3.zero;
        float horizontal = 0f;

        if (keyboard.aKey.isPressed || keyboard.qKey.isPressed)
        {
            inputDir.x -= 1f;
            horizontal -= 1f;
        }
        if (keyboard.dKey.isPressed)
        {
            inputDir.x += 1f;
            horizontal += 1f;
        }
        if (keyboard.wKey.isPressed || keyboard.zKey.isPressed)
            inputDir.z += 1f;
        if (keyboard.sKey.isPressed)
            inputDir.z -= 1f;

        // Dash avec cooldown
        bool canDash = (Time.time - lastDashTime) >= dashCooldown;
        if (keyboard.leftShiftKey.wasPressedThisFrame && inputDir.sqrMagnitude > 0f && !isDashing && canDash)
        {
            isDashing = true;
            dashTimer = dashDuration;
            velocity = inputDir.normalized * dashSpeed;
            lastDashTime = Time.time;
            dashAnimAngle = 0f; // reset animation
        }

        // Inertie
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            dashAnimAngle += 360f * (Time.deltaTime / dashDuration); 
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashAnimAngle = 0f;
            }
        }
        else
        {
            Vector3 targetVelocity = inputDir.normalized * speed;
            velocity = Vector3.Lerp(velocity, targetVelocity, acceleration);
        }

        // D�placement
        controller.Move(velocity * Time.deltaTime);

        // Animation tilt + dash spin
        tilt = Mathf.Lerp(tilt, horizontal * tiltAngle, Time.deltaTime * 8f);
        Quaternion baseRotation = Quaternion.Euler(-90f, 0f, 90f);
        Quaternion tiltRotation = Quaternion.Euler(tilt, 0f, 0f);
        Quaternion dashRotation = isDashing ? Quaternion.Euler(dashAnimAngle, 0f , 0f) : Quaternion.identity;
        if (transform != null)
            transform.localRotation = baseRotation * tiltRotation * dashRotation;
    }
}
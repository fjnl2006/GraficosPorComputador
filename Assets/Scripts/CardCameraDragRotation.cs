using UnityEngine;
using UnityEngine.InputSystem;

public class CardDragTilt : MonoBehaviour
{
    [Header("Rotación máxima")]
    [SerializeField] private float maxYaw = 18f;
    [SerializeField] private float maxPitch = 12f;

    [Header("Sensibilidad")]
    [SerializeField] private float sensitivity = 0.0025f;

    [Header("Suavizado")]
    [SerializeField] private float followSpeed = 14f;
    [SerializeField] private float returnSpeed = 8f;

    [Header("Shader Holográfico")]
    [SerializeField] private Renderer cardRenderer;
    [SerializeField] private float tiltStrength = 0.5f; // Multiplicador de desplazamiento UV

    private static readonly int PointerPosID = Shader.PropertyToID("_pointerPosition");
    private MaterialPropertyBlock propBlock;

    private Quaternion initialRotation;
    private Vector2 dragAmount;
    private Vector2 targetRotation;
    private Vector2 currentRotation;
    private bool isDragging;

    private void Awake()
    {
        initialRotation = transform.localRotation;
        if (cardRenderer == null) cardRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        ReadMouse();
        RotateCard();
        UpdateShaderTilt();
    }

    private void ReadMouse()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            dragAmount = Vector2.zero;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            targetRotation = Vector2.zero;
        }

        if (!isDragging) return;

        Vector2 delta = mouse.delta.ReadValue();
        dragAmount += delta * sensitivity;

        float x = Mathf.Clamp(dragAmount.x, -1f, 1f);
        float y = Mathf.Clamp(dragAmount.y, -1f, 1f);

        float elasticX = Mathf.Sin(x * Mathf.PI * 0.5f);
        float elasticY = Mathf.Sin(y * Mathf.PI * 0.5f);

        targetRotation = new Vector2(
            -elasticY * maxPitch,
            -elasticX * maxYaw
        );
    }

    private void RotateCard()
    {
        float speed = isDragging ? followSpeed : returnSpeed;

        currentRotation = Vector2.Lerp(
            currentRotation,
            targetRotation,
            Time.deltaTime * speed
        );

        Quaternion pitch = Quaternion.AngleAxis(currentRotation.x, Vector3.right);
        Quaternion yaw = Quaternion.AngleAxis(currentRotation.y, Vector3.up);

        transform.localRotation = initialRotation * yaw * pitch;
    }

    private void UpdateShaderTilt()
    {
        if (cardRenderer == null) return;

        // Normalizamos la inclinación actual entre -1 y 1
        Vector2 normalizedTilt = new Vector2(
            (currentRotation.y / maxYaw) * tiltStrength,
            (currentRotation.x / maxPitch) * tiltStrength
        );

        cardRenderer.GetPropertyBlock(propBlock);
        propBlock.SetVector(PointerPosID, normalizedTilt);
        cardRenderer.SetPropertyBlock(propBlock);
    }
}
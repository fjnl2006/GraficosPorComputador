
using UnityEngine;

public class CardHolographicController : MonoBehaviour
{
    [SerializeField] private Renderer cardRenderer;

    private MaterialPropertyBlock propertyBlock;

    private static readonly int PointerPosition =
        Shader.PropertyToID("_PointerPosition");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (cardRenderer == null)
            cardRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                Vector2 uv = hit.textureCoord;

                cardRenderer.GetPropertyBlock(propertyBlock);

                propertyBlock.SetVector(
                    PointerPosition,
                    new Vector4(uv.x, uv.y, 0, 0)
                );

                cardRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}

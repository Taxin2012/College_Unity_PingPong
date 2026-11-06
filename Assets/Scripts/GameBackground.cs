using UnityEngine;

/// <summary>
/// Показывает выбранный в меню фон на игровой сцене.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class GameBackground : MonoBehaviour
{
    public Sprite[] backgrounds;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyBackground();
    }

    private void ApplyBackground()
    {
        if (spriteRenderer == null || backgrounds == null || backgrounds.Length == 0)
            return;

        int index = Mathf.Clamp(GameSettings.BackgroundIndex, 0, backgrounds.Length - 1);
        Sprite sprite = backgrounds[index];
        if (sprite == null)
            return;

        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = -100;

        Camera camera = Camera.main;
        float height = camera != null ? camera.orthographicSize * 2f : 10f;
        float width = camera != null ? height * camera.aspect : 18f;

        Vector2 spriteSize = sprite.bounds.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        float scale = Mathf.Max(width / spriteSize.x, height / spriteSize.y);
        transform.localScale = new Vector3(scale, scale, 1f);
        transform.position = new Vector3(0f, 0f, 0f);
    }
}

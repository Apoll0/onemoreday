using UnityEngine;

public static class SpriteRendererExtensions
{
    public static void ClearAlpha(this SpriteRenderer target)
    {
        Color color = target.color;
        color.a = 0.0f;
        target.color = color;
    }
    
    public static void SetAlpha(this SpriteRenderer target, float a)
    {
        Color color = target.color;
        color.a = a;
        target.color = color;
    }
}

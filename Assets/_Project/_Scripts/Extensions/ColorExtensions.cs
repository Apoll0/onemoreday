using UnityEngine;

public static class ColorExtensions
{
    public static Color Darken(this Color color, float factor = 0.1f)
    {
        factor = Mathf.Clamp01(factor); // Ограничиваем фактор от 0 до 1
        return new Color(color.r * (1 - factor), color.g * (1 - factor), color.b * (1 - factor), color.a);
    }
}
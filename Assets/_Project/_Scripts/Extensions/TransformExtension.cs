using UnityEngine;

public static class TransformExtension
{
    public static void SetX(this RectTransform target, float value)
    {
        if (target == null)
            return;
        target.anchoredPosition = new Vector2(value, target.anchoredPosition.y);
    }

    public static void SetY(this RectTransform target, float value)
    {
        if (target == null)
            return;
        target.anchoredPosition = new Vector2(target.anchoredPosition.x, value);
    }

    public static void SetWidth(this RectTransform target, float value)
    {
        if (target == null)
            return;
        target.sizeDelta = new Vector2(value, target.sizeDelta.y);
    }

    public static void SetHeight(this RectTransform target, float value)
    {
        if (target == null)
            return;
        target.sizeDelta = new Vector2(target.sizeDelta.x, value);
    }
    
    public static void SetSize(this RectTransform target, float value)
    {
        if (target == null)
            return;
        target.sizeDelta = new Vector2(value, value);
    }

    public static void CopyValuesFrom(this Transform target, Transform transform)
    {
        if (target == null)
            return;
        target.SetParent(transform.parent);
        target.position = transform.position;
        target.localScale = transform.localScale;
        target.eulerAngles = transform.eulerAngles;
    }
    
    public static void SetScale(this Transform target, Vector3 value)
    {
        if (target == null)
            return;
        target.localScale = value;
    }

    public static void SetScale(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 scale = target.localScale;
        scale.x = value;
        scale.y = value;
        target.localScale = scale;
    }

    public static void SetScaleX(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 scale = target.localScale;
        scale.x = value;
        target.localScale = scale;
    }
    
    public static void SetScaleY(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 scale = target.localScale;
        scale.y = value;
        target.localScale = scale;
    }
    
    public static void SetX(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 position = target.position;
        position.x = value;
        target.position = position;
    }

    public static void SetY(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 position = target.position;
        position.y = value;
        target.position = position;
    }
    
    public static void SetZ(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 position = target.position;
        position.z = value;
        target.position = position;
    }
    
    public static void MoveBy(this Transform target, Vector3 value)
    {
        if (target == null)
            return;
        target.position += value;
    }

    public static void SetLocalX(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 localPosition = target.localPosition;
        localPosition.x = value;
        target.localPosition = localPosition;
    }

    public static void SetLocalY(this Transform target, float value)
    {
        if (target == null)
            return;
        Vector3 localPosition = target.localPosition;
        localPosition.y = value;
        target.localPosition = localPosition;
    }
    
    public static void AlignXWithGrid(this Transform target, float gridStep)
    {
        if (target == null)
            return;
        Vector3 position = target.position;
        position.x = AlignValueWithGrid(position.x, gridStep);
        target.position = position;
    }
    
    public static void AlignYWithGrid(this Transform target, float gridStep)
    {
        if (target == null)
            return;
        Vector3 position = target.position;
        position.y = AlignValueWithGrid(position.y, gridStep);
        target.position = position;
    }
    
    public static void AlignPositionWithGrid(this Transform target, float gridStep)
    {
        if (target == null)
            return;
        Vector3 position = target.position;
        position.x = AlignValueWithGrid(position.x, gridStep);
        position.y = AlignValueWithGrid(position.y, gridStep);
        target.position = position;
    }
    
    public static void AlignLocalPositionWithGrid(this Transform target, float gridStep)
    {
        if (target == null)
            return;
        Vector3 localPosition = target.localPosition;
        localPosition.x = AlignValueWithGrid(localPosition.x, gridStep);
        localPosition.y = AlignValueWithGrid(localPosition.y, gridStep);
        target.localPosition = localPosition;
    }

    private static float AlignValueWithGrid(float value, float gridStep)
    {
        float mod = value % gridStep;
        float result = value - mod;
        if (Mathf.Abs(mod) >= 0.5f * gridStep)
            result += value > 0.0f ? gridStep : -gridStep;
        return result;
    }
}

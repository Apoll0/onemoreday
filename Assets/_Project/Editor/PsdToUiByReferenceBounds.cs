using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PsdToUiByReferenceBounds
{
    [MenuItem("Tools/PSD Importer/Convert Selected PSD To UI (Reference Bounds)")]
    static void ConvertSelected()
    {
        var psdRoot = Selection.activeGameObject;
        if (!psdRoot)
        {
            Debug.LogError("Выбери корневой объект PSD (созданный PSD Importer).");
            return;
        }

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            Debug.LogError("Canvas в сцене не найден.");
            return;
        }

        var canvasRT = canvas.transform as RectTransform;

        var srs = psdRoot.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs == null || srs.Length == 0)
        {
            Debug.LogError("В выбранном объекте нет SpriteRenderer.");
            return;
        }

        // 1) Находим референс: самый большой sprite.rect (обычно фон/документ)
        SpriteRenderer reference = null;
        float bestArea = -1f;

        foreach (var sr in srs)
        {
            if (!sr.sprite) continue;
            var r = sr.sprite.rect;
            float area = r.width * r.height;
            if (area > bestArea)
            {
                bestArea = area;
                reference = sr;
            }
        }

        if (!reference)
        {
            Debug.LogError("Не удалось найти референсный SpriteRenderer со sprite.");
            return;
        }

        // 2) Считаем px per world unit по референсу
        var refRect = reference.sprite.rect;      // px
        var refBounds = reference.bounds;         // world

        // защита от деления на 0
        if (refBounds.size.x <= 0.00001f || refBounds.size.y <= 0.00001f)
        {
            Debug.LogError("Bounds у референсного слоя слишком маленький/нулевой.");
            return;
        }

        float pxPerUnitX = refRect.width / refBounds.size.x;
        float pxPerUnitY = refRect.height / refBounds.size.y;

        // 3) Создаём UI-root
        var uiRoot = new GameObject(psdRoot.name + "_UI", typeof(RectTransform));
        var uiRootRT = uiRoot.GetComponent<RectTransform>();
        uiRootRT.SetParent(canvasRT, false);
        uiRootRT.anchorMin = uiRootRT.anchorMax = new Vector2(0.5f, 0.5f);
        uiRootRT.pivot = new Vector2(0.5f, 0.5f);
        uiRootRT.anchoredPosition = Vector2.zero;
        uiRootRT.localScale = Vector3.one;

        Vector3 refCenter = refBounds.center;

        // (опционально) порядок — по sortingOrder
        System.Array.Sort(srs, (a, b) => a.sortingOrder.CompareTo(b.sortingOrder));

        int sibling = 0;

        foreach (var sr in srs)
        {
            if (!sr.sprite) continue;

            var go = new GameObject(sr.name, typeof(RectTransform), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            var img = go.GetComponent<Image>();

            img.sprite = sr.sprite;
            img.preserveAspect = true;
            img.raycastTarget = false;

            rt.SetParent(uiRootRT, false);

            // Размер по bounds (world) -> px
            Vector2 sizePx = new Vector2(sr.bounds.size.x * pxPerUnitX, sr.bounds.size.y * pxPerUnitY);
            rt.sizeDelta = sizePx;

            // Позиция относительно референса (world) -> px
            Vector3 delta = sr.bounds.center - refCenter;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(delta.x * pxPerUnitX, delta.y * pxPerUnitY);

            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            rt.SetSiblingIndex(sibling++);

            sr.gameObject.SetActive(false);
        }

        Debug.Log($"ГОТОВО: UI создан по reference bounds. Reference layer: {reference.name}");
    }
}

using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPNoiseJitter : MonoBehaviour
{
    private static readonly int kFaceTex = Shader.PropertyToID("_FaceTex");
    
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float noiseScale = 1f;

    void OnEnable()
    {
        ApplyRandomNoise();
        text.OnPreRenderText += OnPrerenderText;
    }

    private void OnDisable()
    {
        text.OnPreRenderText -= OnPrerenderText;
    }

    [Button(enabledMode:EButtonEnableMode.Always)]
    public void ApplyRandomNoise()
    {
        var mat = text.fontMaterial;

        Vector2 offset = new Vector2(
            Random.value * 10f,
            Random.value * 10f
        );

        mat.SetTextureOffset(kFaceTex, offset);
        mat.SetTextureScale(kFaceTex, new Vector2(1f,1f) * noiseScale);
    }

    private void OnPrerenderText(TMP_TextInfo textInfo)
    {
        ApplyRandomNoise();
    }
}
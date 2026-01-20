using UnityEngine;

public class BranchWindSkin : MonoBehaviour
{
    [Header("Bones root (first bone in chain)")]
    [SerializeField] private Transform rootBone;

    [Header("Wind")]
    [SerializeField] private float windStrength = 1f;    // регулируемая сила
    [SerializeField] private float maxAngle = 10f;       // макс угол на кончике
    [SerializeField] private float baseFreq = 0.7f;      // скорость качания
    [SerializeField] private float noiseFreq = 0.35f;    // скорость хаоса
    [SerializeField] private float noiseAmount = 0.7f;   // насколько хаотично
    [SerializeField] private float smoothing = 8f;       // плавность

    private Transform[] _bones;
    private Quaternion[] _initialRot;
    private float _seed;

    public float WindStrength
    {
        get => windStrength;
        set => windStrength = Mathf.Max(0f, value);
    }

    private void Awake()
    {
        _seed = Random.Range(0f, 1000f);

        // Собираем все кости по иерархии
        _bones = rootBone.GetComponentsInChildren<Transform>(true);

        _initialRot = new Quaternion[_bones.Length];
        for (int i = 0; i < _bones.Length; i++)
            _initialRot[i] = _bones[i].localRotation;
    }

    private void LateUpdate()
    {
        if (_bones == null || _bones.Length == 0) return;

        float t = Time.time;

        for (int i = 0; i < _bones.Length; i++)
        {
            if (i == 0) continue;
            
            // 0 у корня, 1 у дальних костей
            float k = (_bones.Length <= 1) ? 1f : (float)i / (_bones.Length - 1);

            // усиление к концу (кривой градиент)
            float ampK = k * k; // можно k^3 если хочешь сильнее различие

            float sin = Mathf.Sin((t + _seed) * baseFreq + k * 1.4f);

            float noise = Mathf.PerlinNoise(_seed + t * noiseFreq, k * 3.2f) * 2f - 1f;

            float angle = (sin + noise * noiseAmount) * maxAngle * ampK * windStrength;

            Quaternion target = _initialRot[i] * Quaternion.Euler(0, 0, angle);

            _bones[i].localRotation = Quaternion.Slerp(
                _bones[i].localRotation,
                target,
                1f - Mathf.Exp(-smoothing * Time.deltaTime)
            );
        }
    }
}

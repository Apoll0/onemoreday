using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "Scriptable Objects/GameConstants")]
public class GameConstants : HMScriptableSingleton<GameConstants>
{
    [SerializeField] private int _statDefaultValue = 10;
    [SerializeField] private int _lifeAddCooldownSeconds = 300;
    [SerializeField] private int _energyStartQuantity = 5;
    [SerializeField] private float _rareEventChance = 0.2f;
    
    [Header("Animations")]
    [SerializeField] private float _cardRotateDuration = 0.5f;

    #region Properties

    public static int StatDefault => Instance._statDefaultValue;
    public static int EnergyAddCooldown => Instance._lifeAddCooldownSeconds;
    public static int EnergyStartQuantity => Instance._energyStartQuantity;
    public static float CardRotateDuration => Instance._cardRotateDuration;
    public static float RareEventChance => Instance._rareEventChance;

    #endregion
}

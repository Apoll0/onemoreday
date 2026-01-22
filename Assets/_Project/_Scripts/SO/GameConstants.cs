using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "Scriptable Objects/GameConstants")]
public class GameConstants : HMScriptableSingleton<GameConstants>
{
    [SerializeField] private int _statDefaultValue = 10;
    [SerializeField] private int _lifeAddCooldownSeconds = 300;
    [SerializeField] private int _energyStartQuantity = 5;
    [SerializeField] private float _rareEventChance = 0.2f;
    [SerializeField] private float _midEventChance = 0.4f;
    [SerializeField] private float _firstQuestionProbability = 0.5f;
    [SerializeField] private float _everyQuestionProbability = 0.2f;
    
    [Header("Animations")]
    [SerializeField] private float _cardRotateDuration = 0.5f;
    [SerializeField] private float _statChangeDuration = 0.5f;
    [SerializeField] private float _waitLastChanceDuration = 3f;

    #region Properties

    public static int StatDefault => Instance._statDefaultValue;
    public static int EnergyAddCooldown => Instance._lifeAddCooldownSeconds;
    public static int EnergyStartQuantity => Instance._energyStartQuantity;
    public static float RareEventChance => Instance._rareEventChance;
    public static float MidEventChance => Instance._midEventChance;
    public static float FirstQuestionProbability => Instance._firstQuestionProbability;
    public static float EveryQuestionProbability => Instance._everyQuestionProbability;
    public static float CardRotateDuration => Instance._cardRotateDuration;
    public static float StatChangeDuration => Instance._statChangeDuration;
    public static float WaitLastChanceDuration => Instance._waitLastChanceDuration;
    

    #endregion
}

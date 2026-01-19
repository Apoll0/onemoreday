using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "Scriptable Objects/GameConstants")]
public class GameConstants : HMScriptableSingleton<GameConstants>
{
    [SerializeField] private int _statDefaultValue = 10;
    [SerializeField] private int _lifeAddCooldownSeconds = 300;
    [SerializeField] private int _energyStartQuantity = 5;

    #region Properties

    public static int StatDefault => Instance._statDefaultValue;
    public static int EnergyAddCooldown => Instance._lifeAddCooldownSeconds;
    public static int EnergyStartQuantity => Instance._energyStartQuantity;

    #endregion
}

using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    #region Exposed variables

    [SerializeField] private GameObject _viewsContainer;
    [SerializeField] private GameObject _startView;

    #endregion
    
    #region Button callbacks

    public void StartButtonPressed()
    {
        GameManager.Instance.StartGame();
    }

    public void SettingsButtonPressed()
    {
        // TODO: Implement settings UI
    }
    
    #endregion
}

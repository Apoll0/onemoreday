using System;
using UnityEngine;

public class AddScull : MonoBehaviour
{
    [SerializeField] private GameObject[] _sculls;
    
    private void Awake()
    {
        foreach (GameObject scull in _sculls)
            scull.SetActive(false);
        
        // reverse the array to show sculls in correct order
        Array.Reverse(_sculls);
    }

    private void OnEnable()
    {
        DataManager.OnScullsCountChanged += UpdateSculls;
        UpdateSculls(DataManager.Instance.ScullsCount);
    }

    private void OnDisable()
    {
        DataManager.OnScullsCountChanged -= UpdateSculls;
    }

    private void UpdateSculls(int count)
    {
        var clampedCount = Mathf.Clamp(count, 0, _sculls.Length);
        for (int i = 0; i < _sculls.Length; i++)
        {
            _sculls[i].SetActive(i < clampedCount);
        }
    }
}

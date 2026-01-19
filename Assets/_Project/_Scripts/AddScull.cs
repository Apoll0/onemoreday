using System;
using UnityEngine;

public class AddScull : MonoBehaviour
{
    [SerializeField] private GameObject[] _sculls;
    
    private int _currentIndex = 0;

    private void Start()
    {
        foreach (GameObject scull in _sculls)
            scull.SetActive(false);
        
        // reverse the array to show sculls in correct order
        Array.Reverse(_sculls);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_currentIndex < _sculls.Length)
            {
                _sculls[_currentIndex].SetActive(true);
                _currentIndex++;
            }
            else
            {
                for (int i = 0; i < _sculls.Length; i++)
                {
                    _sculls[i].SetActive(false);
                }
                _currentIndex = 0;
            }
        }
    }
}

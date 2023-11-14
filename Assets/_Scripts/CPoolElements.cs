using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class CPoolElements
{
    private List<GameObject> m_ElementsList = new();
    int m_CurrentIndex;

    public CPoolElements (GameObject _Prefab, int _Num, Transform _Parent)
    {
        for (int i = 0; i < _Num; i++)
        {
            GameObject l_Instance = GameObject.Instantiate(_Prefab);
            m_ElementsList.Add(l_Instance);
            l_Instance.transform.SetParent(_Parent);
        }
    }

    public GameObject GetNextElement()
    {
        m_CurrentIndex++;
        if (m_CurrentIndex >= m_ElementsList.Count)
            m_CurrentIndex = 0;
        return m_ElementsList[m_CurrentIndex];
    }

    public void SetActiveAllElements(bool _Value)
    {
        foreach (GameObject l_Obj in m_ElementsList)
        {
            l_Obj.SetActive(_Value);
        }
    }
}

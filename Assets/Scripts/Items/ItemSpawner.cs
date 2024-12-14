using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Serializable]
    public class ItemSpawnerData
    {
        public string name;
        public GameObject gameObject;
        public int dropProbability;
    }

    [SerializeField] ItemSpawnerData[] _itemList;
    [SerializeField] int _noDropProbability;
    [SerializeField] GameObject destroyOnSpawn;

    List<int> _probabilities = new List<int>();

    private void Awake()
    {
        for(int i = 0; i < _itemList.Length; i++)
        {
            for(int n = 0; n < _itemList[i].dropProbability; n++)
            {
                _probabilities.Add(i);
            }
        }
        for(int i = 0; i < _noDropProbability; i++)
        {
            _probabilities.Add(-1);
        }
        _probabilities = randomizeList(_probabilities);
    }

    List<int> randomizeList(List<int> _list)
    {
        List<int> list = _list;
        for (int i = 0; i < list.Count; i++)
        {
            int aux = list[i];
            int r = UnityEngine.Random.Range(0, list.Count);
            int randomSelected = list[r];
            list[i] = randomSelected;
            list[r] = aux;
        }
        return list;
    }


    public void DropItem()
    {
        InstanceItem(CalculateProbability());
    }

    private int CalculateProbability()
    {
        if(_probabilities.Count == 0)
        {
            return -1;
        }
        int value = UnityEngine.Random.Range(0, _probabilities.Count);
        int index = _probabilities[value];
        return index;
    }

    private void InstanceItem()
    {
        Instantiate(_itemList[0].gameObject, transform.position, Quaternion.identity);
    }

    private void InstanceItem(int index)
    {
        if(index >= 0)
        {
            Instantiate(_itemList[index].gameObject, transform.position, Quaternion.identity);
        }
        if (destroyOnSpawn)
        {
            Destroy(destroyOnSpawn);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Item[] _itemList;
    [SerializeField] int _noDropProbability;
    [SerializeField] int[] _dropProbability;

    List<int> _probabilities = new List<int>();

    private void Awake()
    {
        for(int i = 0; i < _dropProbability.Length; i++)
        {
            for(int n = 0; n < _dropProbability[i]; n++)
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
            int r = Random.Range(0, list.Count);
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
        int value = Random.Range(0, _probabilities.Count);
        int index = _probabilities[value];
        return index;
    }

    private void InstanceItem()
    {
        Instantiate(_itemList[0], transform.position, Quaternion.identity);
    }

    private void InstanceItem(int index)
    {
        if(index < 0) { return; }
        Instantiate(_itemList[index], transform.position, Quaternion.identity);
    }
}

using Game.Tool;
using Game.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePoolManager : Singleton<GamePoolManager>
{
    [System.Serializable]
    private class PoolItem//element of pool, each element is a small group with a number of same item belongs with "item name"
    {
        public string ItemName;
        public GameObject Item;
        public int InitMaxCount;//record of its number
    }
    [SerializeField] private List<PoolItem> _configPoolItems=new(); //pool of group
    //pool divided by "item name", each name contains a queue consisted by different kinds of item
    private Dictionary<string,Queue<GameObject>>_poolCenter=new Dictionary<string,Queue<GameObject>>();

    private Dictionary<string, GameObject> _subPool=new Dictionary<string,GameObject>();
    private GameObject _poolItemParent;
    private void InitPool()
    {
        if(_configPoolItems.Count == 0)return;
        //divide
        for (int i = 0; i < _configPoolItems.Count; i++){
            for(int j = 0; j < _configPoolItems[i].InitMaxCount; j++)
            {
                var item = Instantiate(_configPoolItems[i].Item);
                item.SetActive(false);
                if (!_poolCenter.ContainsKey(_configPoolItems[i].ItemName)){
                    _poolCenter.Add(_configPoolItems[i].ItemName,new Queue<GameObject>());
                    _subPool.Add(_configPoolItems[i].ItemName, new GameObject(_configPoolItems[i].ItemName));
                    _subPool[_configPoolItems[i].ItemName].transform.SetParent(_poolItemParent.transform);
                }
                item.transform.SetParent(_subPool[_configPoolItems[i].ItemName].transform);
                _poolCenter[_configPoolItems[i].ItemName].Enqueue(item);
            }
        }
    }

    public void TryGetPoolItem(string name, Vector3 position, Quaternion rotation)
    {
        //if need fresh item
        if (_poolCenter.ContainsKey(name) && !_poolCenter[name].Peek().activeSelf)
        {
            var item = _poolCenter[name].Dequeue();
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);
        }
        else
        {
            DevelopmentToos.WTF("there is no pool named "+ name);
        }
    }

    public GameObject TryGetPoolItem(string name)
    {
        if (_poolCenter.ContainsKey(name)&&!_poolCenter[name].Peek().activeSelf)
        {
            var item = _poolCenter[name].Dequeue();
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);
            return item;
        }
        DevelopmentToos.WTF("there is no pool named " + name);
        return null;
    }
    private void Start()
    {
        _poolItemParent = new GameObject("Pool Parent");
        _poolItemParent.transform.SetParent(transform);
        InitPool();
    }
}

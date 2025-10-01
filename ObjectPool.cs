using System.Collections.Generic;
using UnityEngine;

namespace Hilam
{
    public class ObjectPool<T> where T: MonoBehaviour, IPoolObject
    {
        private T _objectPrefab;
        private int _poolSize = 10;
        private Transform _parent;
        
        private Queue<T> _pool = new Queue<T>();
        private List<T> _activeObjects = new List<T>();

        public ObjectPool(T objectPrefab, int poolSize, Transform parent)
        {
            _objectPrefab = objectPrefab;
            _poolSize = poolSize;
            _parent = parent;

            _pool = new Queue<T>();
            _activeObjects = new List<T>();
            
            CreatePoolObjects();
        }
        
        public void CreatePoolObjects()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                T obj = Object.Instantiate(_objectPrefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }
        public T Spawn()
        {
            T obj = GetFromPool();
            obj.gameObject.SetActive(true);
            obj.Initialize();
            _activeObjects.Add(obj);
            return obj;
        }
        
        private T GetFromPool()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }

            // If pool is empty, create new
            T obj = Object.Instantiate(_objectPrefab, _parent);
            obj.gameObject.SetActive(false);
            
            // Force return the oldest active collectable
            if (_activeObjects.Count > 0)
            {
                T oldest = _activeObjects[0];
                ReturnToPool(oldest);
                return GetFromPool(); // Recursive call to get the just-returned object
            }
            
            return obj;
        }


        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _activeObjects.Remove(obj);
            _pool.Enqueue(obj);
        }

        public void DespawnAllObjects()
        {
            var activeList = new List<T>(_activeObjects);
            
            foreach (T collectable in activeList)
            {
                if (collectable != null)
                {
                    ReturnToPool(collectable);
                }
            }
            
            _activeObjects.Clear();
        }

        public int GetActiveCount()
        {
            return _activeObjects.Count;
        }
        public List<T> GetActiveObjects()
        {
            return _activeObjects;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Author: Ashutosh Rautela
/// Project: Algorythma Unity Test
/// Contact: arautela2772@gmail.com
/// </summary>
namespace AR
{
    /// <summary>
    /// This is the most simple PoolManager existing ever. I have created this as a demo for the test
    /// </summary>
    public class ARPoolManager : MonoBehaviour
    {

        private GameObject prefab;
        public List<Pool> pools;

        private static ARPoolManager instance;

        //Making the Constructor private to prevent Object Constructor from outside
        private ARPoolManager()
        { }

        /// <summary>
        /// Making PoolManager as SingleTon. If any duplicate gameobject is found on another scene, delete it immediately
        /// </summary>
        private void SetUpSingleton()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        //MonoBehaviour LifeCycleHook
        private void Awake()
        {
            this.SetUpSingleton();
        }

        //MonoBehaviour LifeCycleHook
        private void Start()
        {
            this.pools.ForEach(pool => pool.Refresh());
        }

        #region Private_Methods

        #endregion


        #region Public_Methods
        public Transform GetObjectFromPool(Pool poolType)
        {
            return poolType.GetInactiveObjectFromPool() ?? poolType.AddElementsToPool(1, true)[0];
        }

        public Transform GetObjectFromPool(GameObject objectType)
        {
            foreach (Pool oPool in this.pools)
            {
                if (oPool.objectToPool.gameObject == objectType)
                {
                    Debug.Log("Found correct pool");
                    return this.GetObjectFromPool(oPool);
                }
            }
            return null;
        }

        public void DestroyObject(Transform element)
        {
            element.gameObject.SetActive(false);
        }

        public void CreateNewPool(Transform objectToCreatePoolOf)
        {
            this.pools.Add(new Pool());
        }
        #endregion

        #region Properties
        public static ARPoolManager Instance
        {
            get { return ARPoolManager.instance; }
        }
        #endregion
    }

    /// <summary>
    /// Pool class that will create a record of the Pool properties.
    /// </summary>
    [Serializable]
    public class Pool
    {
        public Transform objectToPool;         //Object Type Pool
        public Transform parent;
        public short initialNumberOfObjects = 2;
        public short maxLimit = 100;
        [SerializeField] private List<Transform> objects;

        public void Refresh()
        {
            if (this.initialNumberOfObjects > 0 && this.objects.Count == 0)
            {
                this.AddElementsToPool(this.initialNumberOfObjects);
            }
        }

        public List<Transform> AddElementsToPool(short numberOfElementsToAdd, bool spawnObjects = false)
        {
            List<Transform> elementsAdded = new List<Transform>();
            if (this.objects.Count > this.maxLimit)
            {
                Debug.LogError("Pool Size Exceeded!! Please extend your pool size");
                return null;
            }
            for (int i = 0; i < numberOfElementsToAdd; i++)
            {
                Transform elementToAdd = GameObject.Instantiate(objectToPool.gameObject, Vector3.zero, Quaternion.identity, this.parent ?? ARPoolManager.Instance.transform).transform;
                elementToAdd.gameObject.SetActive(spawnObjects);
                elementsAdded.Add(elementToAdd);
                elementToAdd.name = objectToPool.name + "(P" + elementsAdded.IndexOf(elementToAdd) + ")";
                this.objects.Add(elementToAdd);
            }
            return elementsAdded;
        }

        public Transform GetInactiveObjectFromPool()
        {
            for (int i = 0; i < this.Objects.Count; i++)
            {
                if (!this.Objects[i].gameObject.activeSelf)
                {
                    this.Objects[i].gameObject.SetActive(true);
                    return this.Objects[i];
                }
            }
            return null;
        }

        #region Properties
        public List<Transform> Objects
        {
            get { return this.objects; }
        }
        #endregion
    }
}


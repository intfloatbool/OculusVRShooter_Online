using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RealWorldVRGame
{
    public class EffectsPoolManager : MonoBehaviour
    {
        public static EffectsPoolManager Instance { get; private set; }
        [SerializeField] private GameObject _bloodEffectPrefab;
        [SerializeField] private List<GameObject> _bloodEffects = new List<GameObject>();
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("Another instance using!");
                Destroy(gameObject);
            }
        }

        public void ShowBloodEffect(Vector3 pointWhereShow, Transform lookATransform = null)
        {
            var bloodObject = GetPooledObject(_bloodEffects, _bloodEffectPrefab);
            bloodObject.SetActive(true);
            bloodObject.transform.position = pointWhereShow;
            if (lookATransform != null)
            {
                bloodObject.transform.LookAt(lookATransform);
            }
        }

        private GameObject GetPooledObject(List<GameObject> pooledObjects, GameObject prefab)
        {
            var firstUnusedObject = pooledObjects.FirstOrDefault(o => o.activeInHierarchy == false);
            if (firstUnusedObject == null)
            {
                var newObject = Instantiate(prefab);
                firstUnusedObject = newObject;
                pooledObjects.Add(firstUnusedObject);
            }
            return firstUnusedObject;
        }
    }

}

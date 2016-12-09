using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace sugi.cc
{
    public class ProbabilityObjectEmitter : MonoBehaviour
    {
        [Header("spawnObject(object, emitRate)")]
        public GameObjectFloatPair[] spawnObjects = new[] { new GameObjectFloatPair(null, 1f) };

        Dictionary<float, GameObject> probabilityObjMap;

        protected virtual void OnEnable()
        {
            BuildDictionary();
        }
        void BuildDictionary()
        {
            var total = spawnObjects.Sum(b => b.num);
            var accum = 0f;
            probabilityObjMap = spawnObjects.Where(b => 0 < b.num).ToDictionary(b => { accum += b.num; return accum / total; }, b => b.gameObject);
        }

        GameObject GetObject()
        {
            var val = Random.value;
            return probabilityObjMap.Where(b => val < b.Key).First().Value;
        }

        public virtual GameObject GetEmitObject()
        {
            var go = Instantiate(GetObject());
            return go;
        }
    }
}
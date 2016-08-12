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
        public float emission = 10f;
        public RandomOp emitLimitOp = new RandomOp(-1f, -1f);
        public RandomOp emitDelayOp = new RandomOp(-1f, -1f);

        Dictionary<float, GameObject> probabilityObjMap;

        protected virtual void Start()
        {
            BuildDictionary();
            var emitDelay = emitDelayOp.randomValue;
            if (0 < emitDelay)
                Invoke("EmitStart", emitDelay);
        }

        public virtual void EmitStart()
        {
            StartCoroutine(EmitCoroutine());
        }

        protected virtual IEnumerator EmitCoroutine()
        {
            var emitLimit = emitLimitOp.randomValue;
            var numEmit = 0;
            while (true)
            {
                var emitPerFrame = Time.deltaTime * emission;
                for (var i = 0; i < Mathf.FloorToInt(emitPerFrame); i++)
                {
                    EmitObject();
                    numEmit++;
                }
                if (Random.value < Mathf.Repeat(emitPerFrame, 1f))
                {
                    EmitObject();
                    numEmit++;
                }
                yield return 0;
                if (0 < emitLimit && emitLimit < numEmit)
                    break;
            }
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

        public virtual GameObject EmitObject()
        {
            var go = Instantiate(GetObject());
            return go;
        }
    }
}
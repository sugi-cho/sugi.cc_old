using UnityEngine;
using sugi.cc;

namespace sugi.example
{
    /// <summary>
    /// this is usage of CustomStruct string**Pair
    /// </summary>
    public class PropertySetter : MonoBehaviour
    {
        public StringTexturePair[] textureProperties;
        public StringVectorPair[] vectorProperties;
        public Material targetMat;

        void Start()
        {
            foreach (var texProp in textureProperties)
                targetMat.SetTexture(texProp.propName, texProp.value);
            foreach (var vecProp in vectorProperties)
                targetMat.SetVector(vecProp.propName, vecProp.value);
        }
    }
}
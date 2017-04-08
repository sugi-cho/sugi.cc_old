using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RendererBehaviour : MonoBehaviour
{
    public new Renderer renderer { get { if (_renderer == null) _renderer = GetComponentInChildren<Renderer>(); return _renderer; } }
    Renderer _renderer;
}

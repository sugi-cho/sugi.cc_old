using UnityEngine;
using System.Collections;
using sugi.cc;

public class SetTransformToProperty : MonoBehaviour
{
    public bool
        position,
        rotation,
        direction,
        scale;

    public Material[] targetMaterials;
    public ComputeShader[] targetComputes;
    public Renderer[] targetRenderers;
    public bool toGlobal;

    public string propertyName = "_Trans";

    // Update is called once per frame
    void Update()
    {
        SetProperty();
    }

    public void SetProperty()
    {
        if (position)
            SetPosition();
        if (rotation)
            SetRotation();
        if (direction)
            SetDirection();
        if (scale)
            SetScale();
    }
    void SetPosition()
    {
        var propPos = propertyName + "_Pos";
        foreach (var mat in targetMaterials)
            mat.SetVector(propPos, transform.position);
        foreach (var com in targetComputes)
            com.SetVector(propPos, transform.position);
        foreach (var r in targetRenderers)
            r.SetVector(propPos, transform.position);
    }
    void SetRotation()
    {
        var propRot = propertyName + "_Rot";
        var rot = new Vector4(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        foreach (var mat in targetMaterials)
            mat.SetVector(propRot, rot);
        foreach (var com in targetComputes)
            com.SetVector(propRot, rot);
        foreach (var r in targetRenderers)
            r.SetVector(propRot, rot);
    }
    void SetDirection()
    {
        var propDir = propertyName + "_Dir";
        foreach (var mat in targetMaterials)
            mat.SetVector(propDir, transform.forward);
        foreach (var com in targetComputes)
            com.SetVector(propDir, transform.forward);
        foreach (var r in targetRenderers)
            r.SetVector(propDir, transform.forward);
    }
    void SetScale()
    {
        var propScale = propertyName + "_Scale";
        foreach (var mat in targetMaterials)
            mat.SetVector(propScale, transform.localScale);
        foreach (var com in targetComputes)
            com.SetVector(propScale, transform.localScale);
        foreach (var r in targetRenderers)
            r.SetVector(propScale, transform.localScale);
    }
}

using UnityEngine;
using System.Collections;
using sugi.cc;
using System.Reflection;
using System.Collections.Generic;
using MidiJack;

public class Test : MonoBehaviour
{
    [SerializeField]
    Texture2D tex2d;

    void Start()
    {
        tex2d = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        this.Invoke(SomeMethod, 1f);
        this.CallMethodDelayed(5f, SomeMethod, GetInstanceID(), Time.time, transform.position);
    }

    void Update()
    {
        return;
        var ch = MidiChannel.All;
        Debug.Log(ch);
        var bn = (int)ch;
        Debug.Log(bn);
        Debug.Log((MidiChannel)bn);

        Debug.Log(RenderTexture.active.format);
        Graphics.CopyTexture(RenderTexture.active, tex2d);
    }

    void SomeMethod()
    {
        print("this is some method");
    }

    void SomeMethod(int i, float f, Vector3 pos)
    {
        Debug.Log(i + ":" + f + ":" + pos);
    }
}

using UnityEngine;
using System.Collections;
using System.Reflection;

public class TestAttribute : System.Attribute
{

    public TestAttribute(int i)
    {
        Debug.Log(i);

        var type = typeof(int);
    }
}

using UnityEngine;
using System.Collections;

public class TestAttribute : System.Attribute
{

    public TestAttribute(int i)
    {
        Debug.Log(i);
    }
}

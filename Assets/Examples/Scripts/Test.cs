using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        System.Action action = Test1;
        print(action.Method.Name);
        action += Test2;
        print(action.Method.Name);
        action();
        foreach (var d in action.GetInvocationList())
            print(d.Method.Name);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Test1() { print("1"); }
    void Test2() { print("2"); }
}

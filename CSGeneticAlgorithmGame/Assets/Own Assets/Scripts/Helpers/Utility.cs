using System;
using System.Collections;
using UnityEngine;

public static class Utility // class for random actions i might need to reuse
{
    public static void Wait(int ms, Action action) // wait for some seconds
    {
        new MonoBehaviour().StartCoroutine(doWait(ms / 1000f, action)); // start coroutine so it doesn't interrupt the program
    }
    static IEnumerator doWait(float time, Action callback) // actual wait method
    {
        yield return new WaitForSecondsRealtime(time); // wait for the specified time
        callback(); // do the callback
    }
}

using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
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
    public static string ToHex(this byte[] input) // converts a byte array to a hex string
    {
        StringBuilder builder = new StringBuilder(input.Length * 2); // create a string builder with double the length of the array (each byte takes 2 hex characters)
        foreach (byte b in input) // iterates through each byte
        {
            builder.Append(b.ToString("x2")); // append the byte converted to lowercase hex to the builder
        }
        return builder.ToString(); // return the builder's final string
    }
    public static string Hash(this string inputString, string hashAlg) // hash a string, has support for basic types
    {
        byte[] bytes = Encoding.UTF8.GetBytes(inputString); // gets utf bytes of input
        switch (hashAlg.ToLower()) // switches depending on the selected algorithm
        {
            case "md5": // if it's md5
                {
                    return new MD5CryptoServiceProvider().ComputeHash(bytes).ToHex(); // compute with md5 provider
                }
            case "sha1": // same with sha1
                {
                    return new SHA1CryptoServiceProvider().ComputeHash(bytes).ToHex();
                }
            case "sha256": // these two refer to the same algorithm hence the fallthrough
            case "sha2":
                {
                    return new SHA256CryptoServiceProvider().ComputeHash(bytes).ToHex();
                }
            default: // if any other algorithm is specified
                {
                    return ""; // return an empty string
                }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    readonly HashSet<string> keys = new HashSet<string>();

    public void AddKey(string keyId)
    {
        keys.Add(keyId);
    }

    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RandomizeGameSetting : MonoBehaviour
{
    public void Randomize()
    {
        DataPersistence.Instance.data.Map = GetRandomMap();
        DataPersistence.Instance.data.GameModeScriptName = GetRandomGameMode();
    }

    private string GetRandomMap()
    {
        FieldInfo[] fieldInfos = typeof(Maps).GetFields(BindingFlags.Static | BindingFlags.Public);
        var random = UnityEngine.Random.Range(0, fieldInfos.Length);
        return (string) fieldInfos[random].GetValue(null);
    }

    private string GetRandomGameMode()
    {
        FieldInfo[] fieldInfos = typeof(GameModes).GetFields(BindingFlags.Static | BindingFlags.Public);
        var random = UnityEngine.Random.Range(0, fieldInfos.Length);
        return (string)fieldInfos[random].GetValue(null);
    }
}

using Assets.Script.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UnityUtils
{
    public static bool IsNullOrDestroyed(this System.Object obj)
    {
        if (object.ReferenceEquals(obj, null)) return true;

        if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

        return false;
    }

    public static string GetTimeFromTimer(float timer)
    {
        var ss = Convert.ToInt32(timer % 60).ToString("00");
        var mm = (Math.Floor(timer / 60) % 60).ToString("00");
        return mm + ":" + ss;
    }

    internal static bool IsSameColorIgnoringAlpha(Color color1, Color color2)
    {
        return !(color1.r != color2.r || color1.g != color2.g || color1.b != color2.b);
    }

    internal static void SetLayerAllChildren(Transform root, int layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    internal static Collider2D[] RemoveTriggersFromColliders(Collider2D[] collidersOverlapping)
    {
        List<Collider2D> newList = new List<Collider2D>();
        foreach (Collider2D collider in collidersOverlapping)
        {
            if (!collider.isTrigger)
            {
                newList.Add(collider);
            }
        }
        return newList.ToArray();
    }

    internal static void WriteToDebugFile(string txt)
    {
        int playerId = 0;
        foreach (GameObject player in PlayerUtils.GetAllPlayer())
        {
            if (player.GetComponent<TankController>().netPlayerAvatar.IsOwner)
            {
                playerId = player.GetComponent<TankController>().netPlayerAvatar._playerInfo.Player;
            }
        }
        string path = "./debug" + playerId + ".txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(txt);
        writer.Close();
    }
}

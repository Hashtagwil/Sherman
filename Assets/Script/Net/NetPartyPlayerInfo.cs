using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct FixedPlayerName : INetworkSerializable
{
    ForceNetworkSerializeByMemcpy<FixedString32Bytes> m_Name; // using ForceNetworkSerializeByMemcpy to force compatibility between FixedString and NetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref m_Name);
    }

    public override string ToString()
    {
        return m_Name.Value.ToString();
    }

    public static implicit operator string(FixedPlayerName s) => s.ToString();
    public static implicit operator FixedPlayerName(string s) => new FixedPlayerName() { m_Name = new FixedString32Bytes(s) };
}

public class NetPartyPlayerInfo : INetworkSerializable
{
    [Header("Base")]
    public ulong playerId;
    public int player;
    public FixedPlayerName playerAlias;

    public int tankIndex;
    public Color colorBody;
    public Color colorTurret;

    public override string ToString()
    {
        return playerId.ToString();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref player);
        serializer.SerializeValue(ref playerAlias);
        serializer.SerializeValue(ref tankIndex);
        serializer.SerializeValue(ref colorBody);
        serializer.SerializeValue(ref colorTurret);
    }
}

using System;
using System.Buffers.Binary;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Utils
{
    public static void PrintProperties(object obj)
    {
        if (obj == null)
        {
            Debug.Log("null");
            return;
        }

        StringBuilder sb = new StringBuilder();
        Type type = obj.GetType();
        sb.AppendLine($"Properties of {type.Name}:");

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            try
            {
                object value = prop.GetValue(obj);
                sb.AppendLine($"  {prop.Name} = {value}");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"  {prop.Name} = <error: {ex.Message}>");
            }
        }
        Debug.Log(sb.ToString());
    }

    public static bool ValidateChunk(Span<byte> data, int offset, string token)
    {
        var chunk = new IffChunk(data.Slice(offset, 8), (uint)offset);
        if (chunk.Token != token)
        {
            Debug.LogError(chunk.Token);
        }
        return chunk.Token == token;
    }

    public static bool ValidateChunk(Span<byte> data, int offset, string token, out uint size)
    {
        var chunk = new IffChunk(data.Slice(offset, 8), (uint)offset);
        size = chunk.Size;
        if (chunk.Token != token)
        {
            Debug.LogError(chunk.Token);
        }
        return chunk.Token == token;
    }

    public static Vector3 ReadVector3(Span<byte> data)
    {
        var x = BitConverter.ToSingle(data.Slice(0, 4));
        var y = BitConverter.ToSingle(data.Slice(4, 4));
        var z = BitConverter.ToSingle(data.Slice(8, 4));

        return new Vector3(x, y, z);
    }

    public static Vector2 ReadVector2(Span<byte> data)
    {
        var x = BitConverter.ToSingle(data.Slice(0, 4));
        var y = BitConverter.ToSingle(data.Slice(4, 4));

        return new Vector2(x, y);
    }

    public static Vector2Int ReadVector2Int(Span<byte> data)
    {
        var x = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(0, 4));
        var y = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(4, 4));

        return new Vector2Int(x, y);
    }

    public static Quaternion ReadQuaternion(Span<byte> data)
    {
        var x = BitConverter.ToSingle(data.Slice(0, 4));
        var y = BitConverter.ToSingle(data.Slice(4, 4));
        var z = BitConverter.ToSingle(data.Slice(8, 4));
        var w = BitConverter.ToSingle(data.Slice(12, 4));

        return new Quaternion(x, y, z, w);
    }

    public static Color ReadCImVector(Span<byte> data)
    {
        byte b = data[0];
        byte g = data[1];
        byte r = data[2];
        byte a = data[3];

        return new Color32(r, g, b, a);
    }

    public static Color ReadColor(Span<byte> data)
    {
        byte r = data[0];
        byte g = data[1];
        byte b = data[2];
        byte a = data[3];

        return new Color32(r, g, b, a);
    }

    public static Bounds ReadCAaBox(Span<byte> data)
    {
        var min = ReadVector3(data.Slice(0, 12));
        var max = ReadVector3(data.Slice(12, 12));

        var bounds = new Bounds();
        bounds.min = min;
        bounds.max = max;
        return bounds;
    }

    public static Plane ReadC4Plane(Span<byte> data)
    {
        var normal = ReadVector3(data.Slice(0, 12));
        var distance = BitConverter.ToSingle(data.Slice(12, 4));

        return new Plane(normal, distance);
    }

    public static byte[] Sha256(byte[] fileBytes)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(fileBytes);
    }

    public static string Sha256Hex(byte[] fileBytes)
    {
        var hash = Sha256(fileBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

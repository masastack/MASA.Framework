// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

/// <summary>
/// Temporary use, later versions will be removed
/// </summary>
internal class XmlUtils
{
    public static string Serializer(object data)
    {
        MemoryStream ms = new MemoryStream();
        StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
        XmlSerializer xz = new XmlSerializer(data.GetType());
        xz.Serialize(sw, data);
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public static T Deserialize<T>(string xml)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToCharArray()));
        return (T)xmlSerializer.Deserialize(stream)!;
    }
}

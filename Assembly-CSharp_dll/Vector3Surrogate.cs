using System;
using System.Runtime.Serialization;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class Vector3Surrogate : ISerializationSurrogate
{
	// Token: 0x06000CD6 RID: 3286 RVA: 0x00034D58 File Offset: 0x00032F58
	public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
	{
		Vector3 vector = (Vector3)obj;
		info.AddValue("x", vector.x);
		info.AddValue("y", vector.y);
		info.AddValue("z", vector.z);
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00034DA0 File Offset: 0x00032FA0
	public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
	{
		Vector3 vector = (Vector3)obj;
		try
		{
			vector.x = (float)info.GetDecimal("x");
			vector.y = (float)info.GetDecimal("y");
			vector.z = (float)info.GetDecimal("z");
		}
		catch (Exception)
		{
			Debug.Log("Failed to load vector data, setting to starting pos");
			vector.x = 88.21f;
			vector.y = 16.41f;
			vector.z = -139.86f;
		}
		return vector;
	}
}

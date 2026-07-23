using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Token: 0x02000258 RID: 600
[Serializable]
public abstract class DataModule<T> where T : DataModule<T>
{
	// Token: 0x06000CD1 RID: 3281 RVA: 0x00034BE8 File Offset: 0x00032DE8
	public void Serialize(BinaryFormatter formatter, FileStream file, int currFormatID)
	{
		formatter.Serialize(file, currFormatID);
		formatter.Serialize(file, this);
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00034C00 File Offset: 0x00032E00
	public static T Deserialize(BinaryFormatter formatter, Stream file, int currFormatID)
	{
		int num = (int)formatter.Deserialize(file);
		if (num > currFormatID)
		{
			Debug.Log(string.Concat(new object[]
			{
				"File format newer than current version type=",
				typeof(T),
				" fileVer=",
				num,
				" currVer=",
				currFormatID
			}));
			throw new VersionMismatchException("File format newer than current version.");
		}
		if (num < currFormatID)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Unhandled version type=",
				typeof(T),
				" fileVer=",
				num,
				" currVer=",
				currFormatID
			}));
			throw new VersionMismatchException("File format unhandled.");
		}
		return formatter.Deserialize(file) as T;
	}
}

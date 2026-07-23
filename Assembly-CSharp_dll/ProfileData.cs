using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class ProfileData : Persistable
{
	// Token: 0x06000CED RID: 3309 RVA: 0x00035580 File Offset: 0x00033780
	private static BinaryFormatter CreateFormatter()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		SurrogateSelector surrogateSelector = new SurrogateSelector();
		surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
		binaryFormatter.SurrogateSelector = surrogateSelector;
		return binaryFormatter;
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x000355C0 File Offset: 0x000337C0
	public void Load(Stream stream)
	{
		BinaryFormatter binaryFormatter = ProfileData.CreateFormatter();
		int num = (int)binaryFormatter.Deserialize(stream);
		if (num > 3)
		{
			Debug.Log(string.Concat(new object[]
			{
				"File format newer than current version type=ProfileData fileVer=",
				num,
				" currVer=",
				3
			}));
			throw new VersionMismatchException("File format newer than current version.");
		}
		if (num < 1)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Unhandled version type=ProfileData fileVer=",
				num,
				" currVer=",
				3
			}));
			throw new VersionMismatchException("File format unhandled.");
		}
		this.options = DataModule<OptionsData>.Deserialize(binaryFormatter, stream, 2);
		this.achieve = DataModule<AchieveData>.Deserialize(binaryFormatter, stream, 2);
		try
		{
			this.continueGameName = (string)binaryFormatter.Deserialize(stream);
		}
		catch (EndOfStreamException)
		{
			this.continueGameName = null;
		}
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x000356AC File Offset: 0x000338AC
	public long Write(Stream stream)
	{
		throw new NotImplementedException("Write is not supported for legacy data.");
	}

	// Token: 0x04000C09 RID: 3081
	public const int CURR_FORMAT_ID = 3;

	// Token: 0x04000C0A RID: 3082
	public const int MIN_HANDLED_FORMAT_ID = 1;

	// Token: 0x04000C0B RID: 3083
	public int optionsFormatID = 2;

	// Token: 0x04000C0C RID: 3084
	public OptionsData options = new OptionsData();

	// Token: 0x04000C0D RID: 3085
	public AchieveData achieve = new AchieveData();

	// Token: 0x04000C0E RID: 3086
	public string continueGameName;

	// Token: 0x04000C0F RID: 3087
	private const string NAME = "slimerancher.prf";
}

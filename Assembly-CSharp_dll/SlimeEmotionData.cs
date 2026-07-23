using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

// Token: 0x02000477 RID: 1143
[Serializable]
public class SlimeEmotionData : Dictionary<SlimeEmotions.Emotion, float>
{
	// Token: 0x060017AB RID: 6059 RVA: 0x0005BEBC File Offset: 0x0005A0BC
	public SlimeEmotionData()
	{
	}

	// Token: 0x060017AC RID: 6060 RVA: 0x0005BEC4 File Offset: 0x0005A0C4
	public SlimeEmotionData(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x0005BED0 File Offset: 0x0005A0D0
	public SlimeEmotionData(SlimeEmotions emotions)
	{
		foreach (SlimeEmotions.EmotionState emotionState in emotions.GetAll())
		{
			base[emotionState.emotion] = emotionState.currVal;
		}
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x0005BF30 File Offset: 0x0005A130
	public void AverageIn(SlimeEmotions emotions, float weight)
	{
		float num = 1f - weight;
		foreach (SlimeEmotions.EmotionState emotionState in emotions.GetAll())
		{
			base[emotionState.emotion] = base[emotionState.emotion] * num + emotionState.currVal * weight;
		}
	}

	// Token: 0x060017AF RID: 6063 RVA: 0x0005BFA4 File Offset: 0x0005A1A4
	public override bool Equals(object o)
	{
		if (!(o is SlimeEmotionData))
		{
			return false;
		}
		SlimeEmotionData second = (SlimeEmotionData)o;
		return this.SequenceEqual(second, new SlimeEmotionData.EmotionComparer());
	}

	// Token: 0x060017B0 RID: 6064 RVA: 0x0005BFD0 File Offset: 0x0005A1D0
	public override int GetHashCode()
	{
		int num = 0;
		foreach (KeyValuePair<SlimeEmotions.Emotion, float> keyValuePair in this)
		{
			num ^= (keyValuePair.Key.GetHashCode() ^ keyValuePair.Value.GetHashCode());
		}
		return num;
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x0005C044 File Offset: 0x0005A244
	public override string ToString()
	{
		string text = "";
		foreach (KeyValuePair<SlimeEmotions.Emotion, float> keyValuePair in this)
		{
			text = string.Concat(new object[]
			{
				text,
				keyValuePair.Key,
				":",
				keyValuePair.Value,
				","
			});
		}
		return text;
	}

	// Token: 0x02000478 RID: 1144
	private class EmotionComparer : IEqualityComparer<KeyValuePair<SlimeEmotions.Emotion, float>>
	{
		// Token: 0x060017B2 RID: 6066 RVA: 0x0005C0D0 File Offset: 0x0005A2D0
		public bool Equals(KeyValuePair<SlimeEmotions.Emotion, float> x, KeyValuePair<SlimeEmotions.Emotion, float> y)
		{
			return x.Key == y.Key && Math.Abs(x.Value - y.Value) < 0.001f;
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x000350A2 File Offset: 0x000332A2
		public int GetHashCode(KeyValuePair<SlimeEmotions.Emotion, float> obj)
		{
			throw new NotImplementedException();
		}
	}
}

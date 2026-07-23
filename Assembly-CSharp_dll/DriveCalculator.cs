using System;
using UnityEngine;

// Token: 0x020003C2 RID: 962
public class DriveCalculator
{
	// Token: 0x0600141A RID: 5146 RVA: 0x0004DD0E File Offset: 0x0004BF0E
	public DriveCalculator(SlimeEmotions.Emotion emotion, float extraDrive, float minDrive)
	{
		this.emotion = emotion;
		this.extraDrive = extraDrive;
		this.minDrive = minDrive;
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0004DD2B File Offset: 0x0004BF2B
	public virtual float Drive(SlimeEmotions emotions, Identifiable.Id id)
	{
		return Mathf.Max(0f, Mathf.Max(this.minDrive, emotions.GetCurr(this.emotion)) + this.extraDrive);
	}

	// Token: 0x040012CC RID: 4812
	public SlimeEmotions.Emotion emotion;

	// Token: 0x040012CD RID: 4813
	public float extraDrive;

	// Token: 0x040012CE RID: 4814
	public float minDrive;
}

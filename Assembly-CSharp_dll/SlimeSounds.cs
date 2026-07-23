using System;
using UnityEngine;

// Token: 0x02000492 RID: 1170
[CreateAssetMenu(fileName = "SlimeSounds", menuName = "Slimes/Slime Sounds")]
public class SlimeSounds : ScriptableObject
{
	// Token: 0x0600184E RID: 6222 RVA: 0x0005E1E4 File Offset: 0x0005C3E4
	public bool SuppressIfFeral(SECTR_AudioCue cue)
	{
		return (cue != null && cue == this.voiceAlarmCue) || cue == this.voiceAweCue || cue == this.voiceFunCue || cue == this.voiceJumpCue || cue == this.voiceSplatCue;
	}

	// Token: 0x040017CD RID: 6093
	public SECTR_AudioCue jumpCue;

	// Token: 0x040017CE RID: 6094
	public SECTR_AudioCue bounceCue;

	// Token: 0x040017CF RID: 6095
	public SECTR_AudioCue chompCue;

	// Token: 0x040017D0 RID: 6096
	public SECTR_AudioCue attackCue;

	// Token: 0x040017D1 RID: 6097
	public SECTR_AudioCue gulpCue;

	// Token: 0x040017D2 RID: 6098
	public SECTR_AudioCue plortCue;

	// Token: 0x040017D3 RID: 6099
	public SECTR_AudioCue splatCue;

	// Token: 0x040017D4 RID: 6100
	public SECTR_AudioCue voiceAlarmCue;

	// Token: 0x040017D5 RID: 6101
	public SECTR_AudioCue voiceAweCue;

	// Token: 0x040017D6 RID: 6102
	public SECTR_AudioCue voiceDamageCue;

	// Token: 0x040017D7 RID: 6103
	public SECTR_AudioCue voiceFearCue;

	// Token: 0x040017D8 RID: 6104
	public SECTR_AudioCue voiceFunCue;

	// Token: 0x040017D9 RID: 6105
	public SECTR_AudioCue voiceJumpCue;

	// Token: 0x040017DA RID: 6106
	public SECTR_AudioCue voiceSplatCue;

	// Token: 0x040017DB RID: 6107
	public SECTR_AudioCue sneezeCue;

	// Token: 0x040017DC RID: 6108
	public SECTR_AudioCue rollCue;

	// Token: 0x040017DD RID: 6109
	public SECTR_AudioCue stompJumpCue;

	// Token: 0x040017DE RID: 6110
	public SECTR_AudioCue stompLandCue;

	// Token: 0x040017DF RID: 6111
	public SECTR_AudioCue unferalCue;

	// Token: 0x040017E0 RID: 6112
	public SECTR_AudioCue wiggleCue;

	// Token: 0x040017E1 RID: 6113
	public SECTR_AudioCue cloakCue;

	// Token: 0x040017E2 RID: 6114
	public SECTR_AudioCue decloakCue;

	// Token: 0x040017E3 RID: 6115
	public SECTR_AudioCue gatherCue;
}

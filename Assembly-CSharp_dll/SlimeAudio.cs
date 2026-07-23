using System;
using MonomiPark.SlimeRancher.DataModel;

// Token: 0x0200045C RID: 1116
public class SlimeAudio : SRBehaviour, ActorModel.Participant
{
	// Token: 0x06001703 RID: 5891 RVA: 0x00059610 File Offset: 0x00057810
	public void Awake()
	{
		this.source = base.GetComponent<SECTR_PointSource>();
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x00059620 File Offset: 0x00057820
	public void Play(SECTR_AudioCue cue)
	{
		if (cue != null && (this.slimeModel == null || !this.slimeModel.isFeral || !this.slimeSounds.SuppressIfFeral(cue)))
		{
			this.source.Cue = cue;
			this.source.Play();
		}
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel model)
	{
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x00059670 File Offset: 0x00057870
	public void SetModel(ActorModel model)
	{
		this.slimeModel = (model as SlimeModel);
	}

	// Token: 0x04001631 RID: 5681
	public SlimeSounds slimeSounds;

	// Token: 0x04001632 RID: 5682
	private SECTR_PointSource source;

	// Token: 0x04001633 RID: 5683
	private SlimeModel slimeModel;
}

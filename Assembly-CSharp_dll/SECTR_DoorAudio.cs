using System;
using UnityEngine;

// Token: 0x02000088 RID: 136
[AddComponentMenu("SECTR/Audio/SECTR Door Audio")]
public class SECTR_DoorAudio : MonoBehaviour
{
	// Token: 0x060002DB RID: 731 RVA: 0x000124D5 File Offset: 0x000106D5
	private void OnDisable()
	{
		this._Stop(true);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x000124E0 File Offset: 0x000106E0
	private void OnOpen()
	{
		this._Stop(false);
		this.instance = SECTR_AudioSystem.Play(this.OpenLoopCue, base.transform, Vector3.zero, true, null, false);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0001251C File Offset: 0x0001071C
	private void OnOpening()
	{
		this._Stop(false);
		this.instance = SECTR_AudioSystem.Play(this.OpeningCue, base.transform, Vector3.zero, false, null, false);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00012558 File Offset: 0x00010758
	private void OnClose()
	{
		this._Stop(false);
		this.instance = SECTR_AudioSystem.Play(this.ClosedLoopCue, base.transform, Vector3.zero, true, null, false);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00012594 File Offset: 0x00010794
	private void OnClosing()
	{
		this._Stop(false);
		this.instance = SECTR_AudioSystem.Play(this.ClosingCue, base.transform, Vector3.zero, false, null, false);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x000125D0 File Offset: 0x000107D0
	private void OnWaiting()
	{
		this._Stop(false);
		this.instance = SECTR_AudioSystem.Play(this.WaitingCue, base.transform, Vector3.zero, true, null, false);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0001260B File Offset: 0x0001080B
	private void _Stop(bool stopImmediately)
	{
		this.instance.Stop(stopImmediately);
	}

	// Token: 0x040002FF RID: 767
	private SECTR_AudioCueInstance instance;

	// Token: 0x04000300 RID: 768
	[SECTR_ToolTip("Sound to play while door is in Open state.", null, false)]
	public SECTR_AudioCue OpenLoopCue;

	// Token: 0x04000301 RID: 769
	[SECTR_ToolTip("Sound to play while door is in Closed state.", null, false)]
	public SECTR_AudioCue ClosedLoopCue;

	// Token: 0x04000302 RID: 770
	[SECTR_ToolTip("Sound to play when door starts to open.", null, false)]
	public SECTR_AudioCue OpeningCue;

	// Token: 0x04000303 RID: 771
	[SECTR_ToolTip("Sound to play while door starts to close.", null, false)]
	public SECTR_AudioCue ClosingCue;

	// Token: 0x04000304 RID: 772
	[SECTR_ToolTip("Sound to play while waiting for the door to start opening.", null, false)]
	public SECTR_AudioCue WaitingCue;
}

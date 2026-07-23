using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200050A RID: 1290
public class GlitchTerminalAnimator_Player : SRAnimator
{
	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06001B02 RID: 6914 RVA: 0x00068258 File Offset: 0x00066458
	// (remove) Token: 0x06001B03 RID: 6915 RVA: 0x00068290 File Offset: 0x00066490
	private event GlitchTerminalAnimator_Player.OnStateChanged onStateExit;

	// Token: 0x06001B04 RID: 6916 RVA: 0x000682C5 File Offset: 0x000664C5
	public IEnumerator WaitForStateExit(GlitchTerminalAnimator_PlayerState.Id id)
	{
		bool wasTriggered = false;
		GlitchTerminalAnimator_Player.OnStateChanged listener = delegate(GlitchTerminalAnimator_PlayerState.Id otherid)
		{
			wasTriggered |= (id == otherid);
		};
		this.onStateExit += listener;
		yield return new WaitUntil(() => wasTriggered);
		this.onStateExit -= listener;
		yield break;
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x000682DB File Offset: 0x000664DB
	public void OnStateExit(GlitchTerminalAnimator_PlayerState.Id id)
	{
		if (this.onStateExit != null)
		{
			this.onStateExit(id);
		}
	}

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06001B06 RID: 6918 RVA: 0x000682F4 File Offset: 0x000664F4
	// (remove) Token: 0x06001B07 RID: 6919 RVA: 0x0006832C File Offset: 0x0006652C
	private event GlitchTerminalAnimator_Player.OnAnimationEventListener onAnimationEvent;

	// Token: 0x06001B08 RID: 6920 RVA: 0x00068361 File Offset: 0x00066561
	public IEnumerator WaitForAnimationEvent(GlitchTerminalAnimator_Player.AnimationEvent eventId)
	{
		bool wasTriggered = false;
		GlitchTerminalAnimator_Player.OnAnimationEventListener listener = delegate(GlitchTerminalAnimator_Player.AnimationEvent otherid)
		{
			wasTriggered |= (eventId == otherid);
		};
		this.onAnimationEvent += listener;
		yield return new WaitUntil(() => wasTriggered);
		this.onAnimationEvent -= listener;
		yield break;
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x00068377 File Offset: 0x00066577
	public void OnAnimationEvent(GlitchTerminalAnimator_Player.AnimationEvent eventId)
	{
		if (this.onAnimationEvent != null)
		{
			this.onAnimationEvent(eventId);
		}
	}

	// Token: 0x04001A77 RID: 6775
	public const string TRIGGER_ENTER_SLIMULATION = "trigger_enter_slimulation";

	// Token: 0x04001A78 RID: 6776
	public const string TRIGGER_EXIT_SLIMULATION = "trigger_exit_slimulation";

	// Token: 0x0200050B RID: 1291
	// (Invoke) Token: 0x06001B0C RID: 6924
	private delegate void OnStateChanged(GlitchTerminalAnimator_PlayerState.Id id);

	// Token: 0x0200050C RID: 1292
	public enum AnimationEvent
	{
		// Token: 0x04001A7C RID: 6780
		ENTERING_FULLY_COVERED
	}

	// Token: 0x0200050D RID: 1293
	// (Invoke) Token: 0x06001B10 RID: 6928
	private delegate void OnAnimationEventListener(GlitchTerminalAnimator_Player.AnimationEvent eventId);
}

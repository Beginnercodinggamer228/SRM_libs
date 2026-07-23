using System;
using UnityEngine;

// Token: 0x02000514 RID: 1300
public class GlitchTerminalAudio : SRBehaviour
{
	// Token: 0x06001B27 RID: 6951 RVA: 0x00068550 File Offset: 0x00066750
	public void Awake()
	{
		GlitchMetadata metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.onStateIdle.cue = metadata.animationOnTerminalIdleCue;
		this.onStateIdle.gameObject.SetActive(false);
		this.animator.onStateEnter += delegate(GlitchTerminalAnimatorState.Id id)
		{
			switch (id)
			{
			case GlitchTerminalAnimatorState.Id.SLEEP:
				this.onStateIdle.gameObject.SetActive(false);
				return;
			case GlitchTerminalAnimatorState.Id.BOOT_UP:
				SECTR_AudioSystem.Play(metadata.animationOnTerminalBootupCue, this.onStateBootup.position, false);
				return;
			case GlitchTerminalAnimatorState.Id.IDLE:
				this.onStateIdle.gameObject.SetActive(true);
				return;
			default:
				return;
			}
		};
	}

	// Token: 0x04001A90 RID: 6800
	[Tooltip("Reference to the parent animator.")]
	public GlitchTerminalAnimator animator;

	// Token: 0x04001A91 RID: 6801
	[Tooltip("Transform of where to play the BOOT_UP state sound.")]
	public Transform onStateBootup;

	// Token: 0x04001A92 RID: 6802
	[Tooltip("Sound component playing the IDLE state sound.")]
	public PlaySoundOnEnable onStateIdle;
}

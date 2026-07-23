using System;

// Token: 0x02000638 RID: 1592
public class UIInputLocker : SRBehaviour
{
	// Token: 0x0600216B RID: 8555 RVA: 0x0007FE43 File Offset: 0x0007E043
	public void OnEnable()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.TimeDirector.Pause(true, this.lockEvenSpecialScenes);
		}
		if (!Levels.isSpecial() || this.lockEvenSpecialScenes)
		{
			SECTR_AudioSystem.PauseNonUISFX(true);
		}
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x0007FE7D File Offset: 0x0007E07D
	public void OnDisable()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.TimeDirector.Unpause(true, this.lockEvenSpecialScenes);
		}
		if (!Levels.isSpecial() || this.lockEvenSpecialScenes)
		{
			SECTR_AudioSystem.PauseNonUISFX(false);
		}
	}

	// Token: 0x040020C0 RID: 8384
	public bool lockEvenSpecialScenes;
}

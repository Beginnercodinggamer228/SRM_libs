using System;

// Token: 0x02000728 RID: 1832
public class LabAccessDoor : AccessDoor
{
	// Token: 0x06002641 RID: 9793 RVA: 0x0009279E File Offset: 0x0009099E
	public override void Awake()
	{
		base.Awake();
		this.pediaDir = SRSingleton<SceneContext>.Instance.PediaDirector;
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000927B6 File Offset: 0x000909B6
	public override void Update()
	{
		base.Update();
		if (this.firstUpdate)
		{
			this.MaybeRecountProgress();
			this.firstUpdate = false;
		}
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000927D4 File Offset: 0x000909D4
	public override bool MaybeRecountProgress()
	{
		if (base.MaybeRecountProgress())
		{
			this.pediaDir.UnlockScience();
			return true;
		}
		return false;
	}

	// Token: 0x04002594 RID: 9620
	private PediaDirector pediaDir;

	// Token: 0x04002595 RID: 9621
	private bool firstUpdate = true;
}

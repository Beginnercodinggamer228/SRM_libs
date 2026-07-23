using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000625 RID: 1573
public abstract class StorageSlotUI : MonoBehaviour
{
	// Token: 0x06002107 RID: 8455 RVA: 0x0007E547 File Offset: 0x0007C747
	public virtual void Awake()
	{
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
	}

	// Token: 0x06002108 RID: 8456 RVA: 0x0007E55C File Offset: 0x0007C75C
	public void Update()
	{
		Identifiable.Id currentId = this.GetCurrentId();
		Identifiable.Id id = currentId;
		Identifiable.Id? id2 = this.currentlyStoredId;
		if (!(id == id2.GetValueOrDefault() & id2 != null))
		{
			if (currentId == Identifiable.Id.NONE)
			{
				this.slotIcon.enabled = false;
				this.bar.currValue = 0f;
				this.bar.barColor = Color.black;
				this.frontFrameIcon.sprite = this.frontEmpty;
				this.backFrameIcon.sprite = this.backEmpty;
			}
			else
			{
				this.slotIcon.sprite = this.lookupDir.GetIcon(currentId);
				this.slotIcon.enabled = true;
				this.bar.barColor = this.lookupDir.GetColor(currentId);
				this.frontFrameIcon.sprite = this.frontFilled;
				this.backFrameIcon.sprite = this.backFilled;
			}
			this.currentlyStoredId = new Identifiable.Id?(currentId);
		}
		if (currentId != Identifiable.Id.NONE)
		{
			this.bar.currValue = (float)this.GetCurrentCount();
		}
	}

	// Token: 0x06002109 RID: 8457
	protected abstract Identifiable.Id GetCurrentId();

	// Token: 0x0600210A RID: 8458
	protected abstract int GetCurrentCount();

	// Token: 0x04002062 RID: 8290
	public Image slotIcon;

	// Token: 0x04002063 RID: 8291
	public Image frontFrameIcon;

	// Token: 0x04002064 RID: 8292
	public Image backFrameIcon;

	// Token: 0x04002065 RID: 8293
	public WorldStatusBar bar;

	// Token: 0x04002066 RID: 8294
	public Sprite backEmpty;

	// Token: 0x04002067 RID: 8295
	public Sprite backFilled;

	// Token: 0x04002068 RID: 8296
	public Sprite frontEmpty;

	// Token: 0x04002069 RID: 8297
	public Sprite frontFilled;

	// Token: 0x0400206A RID: 8298
	private LookupDirector lookupDir;

	// Token: 0x0400206B RID: 8299
	private Identifiable.Id? currentlyStoredId;
}

using System;
using MonomiPark.SlimeRancher.DataModel;

// Token: 0x02000713 RID: 1811
public class GlitchStorage : IdHandler<GlitchStorageModel>
{
	// Token: 0x1700025C RID: 604
	// (get) Token: 0x060025D2 RID: 9682 RVA: 0x00090FC9 File Offset: 0x0008F1C9
	public Identifiable.Id selected
	{
		get
		{
			return this.model.id;
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x060025D3 RID: 9683 RVA: 0x00090FD6 File Offset: 0x0008F1D6
	public int count
	{
		get
		{
			return this.model.count;
		}
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x00090FE3 File Offset: 0x0008F1E3
	public override void Awake()
	{
		base.Awake();
		base.GetRequiredComponentInChildren<WorldStatusBar>(false).maxValue = 300f;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x00090FFC File Offset: 0x0008F1FC
	protected override string IdPrefix()
	{
		return "glitchST";
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x00091003 File Offset: 0x0008F203
	protected override GameModel.Unregistrant Register(GameModel game)
	{
		return game.Glitch.storage.Register(this);
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x00003296 File Offset: 0x00001496
	protected override void InitModel(GlitchStorageModel model)
	{
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x00091016 File Offset: 0x0008F216
	protected override void SetModel(GlitchStorageModel model)
	{
		this.model = model;
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x00091020 File Offset: 0x0008F220
	public bool Add(Identifiable.Id id)
	{
		if (this.model.count > 0 && this.model.id != id)
		{
			return false;
		}
		if (this.model.count >= 300)
		{
			return false;
		}
		if (this.model.count == 0 && !SiloStorage.StorageType.NON_SLIMES.Contains(id))
		{
			return false;
		}
		this.model.count++;
		this.model.id = id;
		return true;
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x00091098 File Offset: 0x0008F298
	public bool Remove(out Identifiable.Id id)
	{
		id = this.model.id;
		if (this.model.count > 0)
		{
			this.model.count--;
			if (this.model.count == 0)
			{
				this.model.id = Identifiable.Id.NONE;
			}
			return true;
		}
		return false;
	}

	// Token: 0x0400253D RID: 9533
	private const SiloStorage.StorageType STORAGE_TYPE = SiloStorage.StorageType.NON_SLIMES;

	// Token: 0x0400253E RID: 9534
	private const int MAX_COUNT = 300;

	// Token: 0x0400253F RID: 9535
	private GlitchStorageModel model;
}

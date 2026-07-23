using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class DecorizerStorage : IdHandler, DecorizerModel.Participant
{
	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06001053 RID: 4179 RVA: 0x000415B0 File Offset: 0x0003F7B0
	// (set) Token: 0x06001054 RID: 4180 RVA: 0x000415BD File Offset: 0x0003F7BD
	public Identifiable.Id selected
	{
		get
		{
			return this.settings.selected;
		}
		set
		{
			this.settings.selected = value;
		}
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x000415CB File Offset: 0x0003F7CB
	protected override string IdPrefix()
	{
		return "decorizer";
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x000415D2 File Offset: 0x0003F7D2
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterDecorizer(this);
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(DecorizerModel model)
	{
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000415E4 File Offset: 0x0003F7E4
	public void SetModel(DecorizerModel model)
	{
		this.model = model;
		this.settings = model.GetSettings(base.id);
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000415FF File Offset: 0x0003F7FF
	public void OnDecorizerRemoved(Identifiable.Id id)
	{
		if (id == this.selected && this.model.GetCount(id) == 0)
		{
			this.selected = Identifiable.Id.NONE;
		}
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x0004161F File Offset: 0x0003F81F
	public bool Add(Identifiable.Id id)
	{
		if (this.model.Add(id))
		{
			if (this.selected == Identifiable.Id.NONE)
			{
				this.selected = id;
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00041641 File Offset: 0x0003F841
	public bool Remove(out Identifiable.Id id)
	{
		id = this.selected;
		if (this.model.Remove(this.selected))
		{
			return true;
		}
		id = Identifiable.Id.NONE;
		return false;
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x00041664 File Offset: 0x0003F864
	public int GetCount()
	{
		return this.model.GetCount(this.selected);
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00041678 File Offset: 0x0003F878
	public void Cleanup(IEnumerable<Identifiable.Id> ids)
	{
		base.GetRequiredComponentInParent<CellDirector>(false).Get(ids, DecorizerStorage.CLEANUP_RESULTS);
		for (int i = 0; i < DecorizerStorage.CLEANUP_RESULTS.Count; i++)
		{
			GameObject gameObject = DecorizerStorage.CLEANUP_RESULTS[i];
			if (this.Add(Identifiable.GetId(gameObject)))
			{
				Destroyer.DestroyActor(gameObject, "DecorizerStorage.Cleanup", false);
			}
		}
		DecorizerStorage.CLEANUP_RESULTS.Clear();
	}

	// Token: 0x04000F1C RID: 3868
	private static List<GameObject> CLEANUP_RESULTS = new List<GameObject>();

	// Token: 0x04000F1D RID: 3869
	private DecorizerModel model;

	// Token: 0x04000F1E RID: 3870
	private DecorizerModel.Settings settings;
}

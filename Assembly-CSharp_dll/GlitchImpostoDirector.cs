using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004DC RID: 1244
[RequireComponent(typeof(Region))]
public class GlitchImpostoDirector : SRBehaviour, GlitchImpostoDirectorModel.Participant
{
	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001A08 RID: 6664 RVA: 0x0006579E File Offset: 0x0006399E
	public string id
	{
		get
		{
			return base.name;
		}
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x000657A6 File Offset: 0x000639A6
	public void Awake()
	{
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		SRSingleton<SceneContext>.Instance.GameModel.Glitch.Register(this);
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x000657E2 File Offset: 0x000639E2
	public void Start()
	{
		this.region = base.GetComponent<Region>();
		this.region.onHibernationStateChanged += this.OnHibernationStateChanged;
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x00065807 File Offset: 0x00063A07
	public void InitModel(GlitchImpostoDirectorModel model)
	{
		model.hibernationTime = null;
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x00065815 File Offset: 0x00063A15
	public void SetModel(GlitchImpostoDirectorModel model)
	{
		this.model = model;
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x00065820 File Offset: 0x00063A20
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.Glitch.Unregister(this);
		}
		if (this.region != null)
		{
			this.region.onHibernationStateChanged -= this.OnHibernationStateChanged;
			this.region = null;
		}
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x0006587B File Offset: 0x00063A7B
	public void Register(GlitchImposto imposto)
	{
		this.registered.Add(imposto);
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x0006588C File Offset: 0x00063A8C
	public bool Deregister(GlitchImposto imposto)
	{
		return this.registered.RemoveAll((GlitchImposto d) => d == imposto) >= 1;
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x000658C4 File Offset: 0x00063AC4
	public void ResetImpostos()
	{
		this.registered.ForEach(delegate(GlitchImposto imposto)
		{
			imposto.Deactivate();
		});
		foreach (GlitchImposto glitchImposto in Randoms.SHARED.Pick<GlitchImposto>((from imposto in this.registered
		where imposto.IsReady()
		select imposto).ToList<GlitchImposto>(), Mathf.FloorToInt(this.availableCount.GetRandom()), (GlitchImposto imposto) => imposto.weight))
		{
			glitchImposto.Activate();
		}
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x0006599C File Offset: 0x00063B9C
	private void OnHibernationStateChanged(bool hibernating)
	{
		if (hibernating)
		{
			if (this.model.hibernationTime == null)
			{
				this.model.hibernationTime = new double?(this.timeDirector.WorldTime());
				return;
			}
		}
		else if (this.model.hibernationTime == null || this.timeDirector.TimeSince(this.model.hibernationTime.Value) >= (double)(this.metadata.impostoMinHibernationTime * 3600f))
		{
			this.ResetImpostos();
		}
	}

	// Token: 0x0400199B RID: 6555
	[Tooltip("Random range of number of impostos to enable this cell is unhibernated.")]
	public Vector2 availableCount;

	// Token: 0x0400199C RID: 6556
	private GlitchImpostoDirectorModel model;

	// Token: 0x0400199D RID: 6557
	private GlitchMetadata metadata;

	// Token: 0x0400199E RID: 6558
	private TimeDirector timeDirector;

	// Token: 0x0400199F RID: 6559
	private Region region;

	// Token: 0x040019A0 RID: 6560
	private List<GlitchImposto> registered = new List<GlitchImposto>();
}

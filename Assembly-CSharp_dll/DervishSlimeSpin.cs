using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B6 RID: 950
public class DervishSlimeSpin : SlimeHover, LiquidConsumer
{
	// Token: 0x060013CC RID: 5068 RVA: 0x0004CC78 File Offset: 0x0004AE78
	public override void Start()
	{
		base.Start();
		this.totemLinker = base.GetComponentInChildren<TotemLinker>();
		this.calmer = base.GetComponent<CalmedByWaterSpray>();
		this.isLargo = (base.GetComponent<Vacuumable>().size > Vacuumable.Size.NORMAL);
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.slimeAppearanceApplicator.OnAppearanceChanged += this.UpdateTornadoAppearance;
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.UpdateTornadoAppearance(this.slimeAppearanceApplicator.Appearance);
		}
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x0004CCFE File Offset: 0x0004AEFE
	public override float Relevancy(bool isGrounded)
	{
		if (this.calmer.IsCalmed())
		{
			return 0f;
		}
		return base.Relevancy(isGrounded);
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0004CD1C File Offset: 0x0004AF1C
	public override void Selected()
	{
		base.Selected();
		if (this.tornado == null)
		{
			this.tornado = this.SpawnTornado();
		}
		if (this.totemLinker != null)
		{
			this.totemLinker.DisableToteming();
		}
		if (this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) >= 0.95f)
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			Quaternion rotation = Quaternion.Euler(eulerAngles);
			while (DervishSlimeSpin.whirlwinds.Count > 0 && DervishSlimeSpin.whirlwinds.Peek() == null)
			{
				DervishSlimeSpin.whirlwinds.Dequeue();
			}
			if (DervishSlimeSpin.whirlwinds.Count > 6)
			{
				DervishSlimeSpin.whirlwinds.Dequeue().GetComponent<DestroyAfterTime>().SetDeathTime(0.0);
			}
			GameObject gameObject = SRBehaviour.InstantiateDynamic(this.fullWhirlwindPrefab, base.transform.position, rotation, false);
			gameObject.GetComponent<DestroyAfterTime>().FizzleParticlesOnDestroy();
			DervishSlimeSpin.whirlwinds.Enqueue(gameObject);
		}
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0004CE30 File Offset: 0x0004B030
	public override void Deselected()
	{
		base.Deselected();
		if (this.tornado != null)
		{
			Destroyer.Destroy(this.tornado, "DervishSlimeSpin.Deselected");
		}
		if (this.totemLinker != null)
		{
			this.totemLinker.EnableToteming();
		}
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0004CE6F File Offset: 0x0004B06F
	protected override float GetHoverAccel()
	{
		return 600f;
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0004CE76 File Offset: 0x0004B076
	protected override float GetHoverHeight()
	{
		if (!this.isLargo)
		{
			return 5f;
		}
		return 9f;
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0004CE8B File Offset: 0x0004B08B
	protected override float GetInvHoverHeight()
	{
		if (!this.isLargo)
		{
			return 0.2f;
		}
		return 0.11111111f;
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0004CEA0 File Offset: 0x0004B0A0
	private void UpdateTornadoAppearance(SlimeAppearance appearance)
	{
		this.tornadoPrefab = appearance.TornadoAppearance.tornadoPrefab;
		this.tornadoLargoPrefab = appearance.TornadoAppearance.largoTornadoPrefab;
		this.fullWhirlwindPrefab = appearance.TornadoAppearance.fullWhirlwindPrefab;
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0004CED5 File Offset: 0x0004B0D5
	private GameObject SpawnTornado()
	{
		GameObject gameObject = SRBehaviour.InstantiateDynamic(this.isLargo ? this.tornadoLargoPrefab : this.tornadoPrefab, base.transform.position, Quaternion.identity, false);
		gameObject.GetComponent<KeepAlignedUnderActor>().AlignWith(base.transform);
		return gameObject;
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0004CF14 File Offset: 0x0004B114
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsWater(liquidId))
		{
			this.plexer.ForceRethink();
		}
	}

	// Token: 0x0400128B RID: 4747
	[Tooltip("The mini-vortex we attach under ourselves")]
	public GameObject tornadoPrefab;

	// Token: 0x0400128C RID: 4748
	[Tooltip("The mini-vortex we attach under ourselves")]
	public GameObject tornadoLargoPrefab;

	// Token: 0x0400128D RID: 4749
	[Tooltip("The full self-moving whirlwind we spawn only when agitated")]
	public GameObject fullWhirlwindPrefab;

	// Token: 0x0400128E RID: 4750
	private const float STD_HOVER_HEIGHT = 5f;

	// Token: 0x0400128F RID: 4751
	private const float INV_STD_HOVER_HEIGHT = 0.2f;

	// Token: 0x04001290 RID: 4752
	private const float LARGO_HOVER_HEIGHT = 9f;

	// Token: 0x04001291 RID: 4753
	private const float INV_LARGO_HOVER_HEIGHT = 0.11111111f;

	// Token: 0x04001292 RID: 4754
	private const float WHIRLWIND_CUTOFF = 0.95f;

	// Token: 0x04001293 RID: 4755
	private const int MAX_WHIRLWINDS = 6;

	// Token: 0x04001294 RID: 4756
	private GameObject tornado;

	// Token: 0x04001295 RID: 4757
	private TotemLinker totemLinker;

	// Token: 0x04001296 RID: 4758
	private CalmedByWaterSpray calmer;

	// Token: 0x04001297 RID: 4759
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x04001298 RID: 4760
	private bool isLargo;

	// Token: 0x04001299 RID: 4761
	private static Queue<GameObject> whirlwinds = new Queue<GameObject>();
}

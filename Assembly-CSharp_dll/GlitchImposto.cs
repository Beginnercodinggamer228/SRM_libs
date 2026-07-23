using System;
using System.Collections;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020004D9 RID: 1241
public class GlitchImposto : IdHandler<GlitchImpostoModel>, LiquidConsumer
{
	// Token: 0x060019F1 RID: 6641 RVA: 0x000652FA File Offset: 0x000634FA
	protected override string IdPrefix()
	{
		return "imposto";
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x00065304 File Offset: 0x00063504
	public override void Awake()
	{
		base.Awake();
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.player = SRSingleton<SceneContext>.Instance.Player;
		this.impostoDirector = base.GetComponentInParent<GlitchImpostoDirector>();
		this.impostoDirector.Register(this);
		this.renderers = base.GetComponentsInChildren<Renderer>(true);
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x00065371 File Offset: 0x00063571
	protected override GameModel.Unregistrant Register(GameModel game)
	{
		return game.Glitch.impostos.Register(this);
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x00065384 File Offset: 0x00063584
	protected override void InitModel(GlitchImpostoModel model)
	{
		model.deactivateTime = null;
		model.cooldownTime = 0.0;
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x000653A1 File Offset: 0x000635A1
	protected override void SetModel(GlitchImpostoModel model)
	{
		this.model = model;
		if (this.model.deactivateTime != null && this.timeDirector.HasReached(this.model.deactivateTime.Value))
		{
			this.Deactivate();
		}
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000653DF File Offset: 0x000635DF
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.impostoDirector != null)
		{
			this.impostoDirector.Deregister(this);
			this.impostoDirector = null;
		}
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x0006540C File Offset: 0x0006360C
	public void Update()
	{
		GlitchImposto.Visibility maxVisibility = this.GetMaxVisibility();
		if (maxVisibility != this.visibility)
		{
			this.OnVisibilityChanged(this.visibility, maxVisibility);
			this.visibility = maxVisibility;
		}
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x00065440 File Offset: 0x00063640
	public void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = ((this.visibility == GlitchImposto.Visibility.IN_RANGE) ? Color.green : ((this.visibility == GlitchImposto.Visibility.OUT_OF_RANGE) ? Color.yellow : Color.red));
			Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 10f);
		}
		if (GizmosUtil.IsThisOrChildSelected(base.gameObject) && this.spawnNode != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(this.spawnNode.position, 0.5f);
		}
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x000654E7 File Offset: 0x000636E7
	public void Deactivate()
	{
		base.gameObject.SetActive(false);
		this.model.deactivateTime = new double?(0.0);
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x0006550E File Offset: 0x0006370E
	public void Activate()
	{
		this.visibility = GlitchImposto.Visibility.NONE;
		base.gameObject.SetActive(true);
		this.model.deactivateTime = null;
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x00065534 File Offset: 0x00063734
	public bool IsReady()
	{
		return this.timeDirector.HasReached(this.model.cooldownTime);
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x0006554C File Offset: 0x0006374C
	public void AddLiquid(Identifiable.Id id, float units)
	{
		if (id == Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID)
		{
			this.metadata.impostoExposure.OnExposed(base.gameObject, new Vector3?((this.spawnNode != null) ? this.spawnNode.position : base.transform.position), null, null, null, null);
			this.model.cooldownTime = this.timeDirector.HoursFromNow(this.metadata.impostoCooldownTime);
			this.Deactivate();
		}
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x000655D8 File Offset: 0x000637D8
	private void OnVisibilityChanged(GlitchImposto.Visibility previous, GlitchImposto.Visibility current)
	{
		if (previous == GlitchImposto.Visibility.IN_RANGE)
		{
			this.model.deactivateTime = new double?(this.timeDirector.HoursFromNow(this.metadata.impostoDeactivateTime * 0.016666668f));
			return;
		}
		if (previous == GlitchImposto.Visibility.NONE && this.model.deactivateTime != null && this.timeDirector.HasReached(this.model.deactivateTime.Value))
		{
			base.StartCoroutine(this.OnFailedExposedCoroutine());
			return;
		}
		if (current == GlitchImposto.Visibility.IN_RANGE)
		{
			this.model.deactivateTime = null;
		}
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x0006566B File Offset: 0x0006386B
	private IEnumerator OnFailedExposedCoroutine()
	{
		yield return new WaitForSeconds(this.metadata.impostoFailedExposedDelayTime);
		this.metadata.impostoExposure.OnFailedExposed(base.gameObject);
		this.Deactivate();
		yield break;
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x0006567C File Offset: 0x0006387C
	private GlitchImposto.Visibility GetMaxVisibility()
	{
		GlitchImposto.Visibility visibility = GlitchImposto.Visibility.NONE;
		int num = 0;
		while (num < this.renderers.Length && visibility < GlitchImposto.Visibility.IN_RANGE)
		{
			visibility = GlitchImposto.Max(visibility, this.renderers[num].isVisible ? (((this.player.transform.position - this.renderers[num].transform.position).sqrMagnitude <= this.metadata.impostoDetectionRange) ? GlitchImposto.Visibility.IN_RANGE : GlitchImposto.Visibility.OUT_OF_RANGE) : GlitchImposto.Visibility.NONE);
			num++;
		}
		return visibility;
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x000656FC File Offset: 0x000638FC
	private static GlitchImposto.Visibility Max(GlitchImposto.Visibility a, GlitchImposto.Visibility b)
	{
		if (a > b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x0400198B RID: 6539
	[Tooltip("Custom transform node used as the spawn position of the glitch slimes when exposed. (optional)")]
	public Transform spawnNode;

	// Token: 0x0400198C RID: 6540
	[Tooltip("Weight used when picking which imposto to be enabled by the GlitchImpostoDirector.")]
	public float weight = 1f;

	// Token: 0x0400198D RID: 6541
	private GlitchImpostoModel model;

	// Token: 0x0400198E RID: 6542
	private GlitchMetadata metadata;

	// Token: 0x0400198F RID: 6543
	private TimeDirector timeDirector;

	// Token: 0x04001990 RID: 6544
	private GameObject player;

	// Token: 0x04001991 RID: 6545
	private GlitchImpostoDirector impostoDirector;

	// Token: 0x04001992 RID: 6546
	private Renderer[] renderers;

	// Token: 0x04001993 RID: 6547
	private GlitchImposto.Visibility visibility;

	// Token: 0x020004DA RID: 1242
	private enum Visibility
	{
		// Token: 0x04001995 RID: 6549
		NONE,
		// Token: 0x04001996 RID: 6550
		OUT_OF_RANGE,
		// Token: 0x04001997 RID: 6551
		IN_RANGE
	}
}

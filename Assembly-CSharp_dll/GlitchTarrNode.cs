using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000715 RID: 1813
public class GlitchTarrNode : IdHandler<GlitchTarrNodeModel>
{
	// Token: 0x1700025E RID: 606
	// (get) Token: 0x060025E0 RID: 9696 RVA: 0x00091125 File Offset: 0x0008F325
	public GlitchTarrNode.Group activationGroup
	{
		get
		{
			return this.cellDirector.tarrActivationGroup;
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x060025E1 RID: 9697 RVA: 0x00091132 File Offset: 0x0008F332
	// (set) Token: 0x060025E2 RID: 9698 RVA: 0x0009113A File Offset: 0x0008F33A
	public Vector3 scale { get; private set; }

	// Token: 0x060025E3 RID: 9699 RVA: 0x00091144 File Offset: 0x0008F344
	public override void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.cellDirector = base.GetRequiredComponentInParent<GlitchCellDirector>(false);
		this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
		this.scale = base.transform.localScale;
		base.Awake();
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x0009119A File Offset: 0x0008F39A
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.timeDirector != null)
		{
			this.timeDirector.RemovePassedTimeDelegate(new Action(this.OnActivationStateChanged));
			this.timeDirector = null;
		}
	}

	// Token: 0x060025E5 RID: 9701 RVA: 0x000911CE File Offset: 0x0008F3CE
	protected override string IdPrefix()
	{
		return "glitchTN";
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x000911D5 File Offset: 0x0008F3D5
	protected override GameModel.Unregistrant Register(GameModel game)
	{
		return game.Glitch.nodes.Register(this);
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000911E8 File Offset: 0x0008F3E8
	protected override void InitModel(GlitchTarrNodeModel model)
	{
		model.activationTime = 0.0;
	}

	// Token: 0x060025E8 RID: 9704 RVA: 0x000911F9 File Offset: 0x0008F3F9
	protected override void SetModel(GlitchTarrNodeModel model)
	{
		this.model = model;
		this.ResetNode(this.model.activationTime);
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x00091214 File Offset: 0x0008F414
	public void ResetNode(double activationTime)
	{
		this.model.activationTime = activationTime;
		this.OnActivationStateChanged();
		if (!this.timeDirector.HasReached(activationTime))
		{
			this.timeDirector.RemovePassedTimeDelegate(new Action(this.OnActivationStateChanged));
			this.timeDirector.AddPassedTimeDelegate(activationTime, new Action(this.OnActivationStateChanged));
		}
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x00091270 File Offset: 0x0008F470
	public GlitchTarrNode.State GetState()
	{
		if (!this.timeDirector.HasReached(this.model.activationTime))
		{
			return GlitchTarrNode.State.INACTIVE;
		}
		if (this.tween != null && this.tween.IsActive() && this.tween.IsPlaying())
		{
			return GlitchTarrNode.State.ACTIVATING;
		}
		return GlitchTarrNode.State.ACTIVE;
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000912BC File Offset: 0x0008F4BC
	private void OnActivationStateChanged()
	{
		Tween tween = this.tween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.tween = null;
		bool flag = this.timeDirector.HasReached(this.model.activationTime);
		base.gameObject.SetActive(flag);
		if (flag)
		{
			this.tween = base.transform.DOScale(this.scale, this.metadata.tarrNodeScaleInSpeed).From(this.scale * 0.2f, true).SetEase(Ease.Linear).SetSpeedBased(true);
		}
	}

	// Token: 0x04002541 RID: 9537
	[Tooltip("Tarr node activation group minor index.")]
	[Range(0f, 10f)]
	public int activationIndex;

	// Token: 0x04002542 RID: 9538
	private TimeDirector timeDirector;

	// Token: 0x04002543 RID: 9539
	private GlitchCellDirector cellDirector;

	// Token: 0x04002544 RID: 9540
	private GlitchTarrNodeModel model;

	// Token: 0x04002545 RID: 9541
	private GlitchMetadata metadata;

	// Token: 0x04002547 RID: 9543
	private Tween tween;

	// Token: 0x02000716 RID: 1814
	public enum State
	{
		// Token: 0x04002549 RID: 9545
		INACTIVE,
		// Token: 0x0400254A RID: 9546
		ACTIVATING,
		// Token: 0x0400254B RID: 9547
		ACTIVE
	}

	// Token: 0x02000717 RID: 1815
	public enum Group
	{
		// Token: 0x0400254D RID: 9549
		A,
		// Token: 0x0400254E RID: 9550
		B
	}
}

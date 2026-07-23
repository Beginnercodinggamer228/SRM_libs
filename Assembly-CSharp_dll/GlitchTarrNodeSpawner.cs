using System;

// Token: 0x0200071B RID: 1819
public class GlitchTarrNodeSpawner : DirectedSlimeSpawner
{
	// Token: 0x060025FB RID: 9723 RVA: 0x00091584 File Offset: 0x0008F784
	public override void Awake()
	{
		base.Awake();
		this.config = SRSingleton<SceneContext>.Instance.GameModeConfig;
		this.node = base.GetRequiredComponent<GlitchTarrNode>();
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000915A8 File Offset: 0x0008F7A8
	public override bool CanSpawn(float? forHour = null)
	{
		return this.node.GetState() == GlitchTarrNode.State.ACTIVE && !this.config.GetModeSettings().preventHostiles && base.CanSpawn(forHour);
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000915D3 File Offset: 0x0008F7D3
	protected override void Register(CellDirector director)
	{
		(director as GlitchCellDirector).Register(this);
	}

	// Token: 0x04002558 RID: 9560
	private GlitchTarrNode node;

	// Token: 0x04002559 RID: 9561
	private GameModeConfig config;
}

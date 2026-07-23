using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000578 RID: 1400
public class EnergyMeter : SRBehaviour
{
	// Token: 0x06001D23 RID: 7459 RVA: 0x0006EB2C File Offset: 0x0006CD2C
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.model = SRSingleton<SceneContext>.Instance.GameModel.GetPlayerModel();
		this.state = SRSingleton<SceneContext>.Instance.PlayerState;
		this.statusBar = base.GetComponent<StatusBar>();
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x0006EB7C File Offset: 0x0006CD7C
	public void Update()
	{
		this.statusBar.currValue = (float)this.state.GetCurrEnergy();
		this.statusBar.maxValue = (float)this.state.GetMaxEnergy();
		if (this.onEnergyRechargingFX)
		{
			this.onEnergyRechargingFX.SetActive(this.timeDirector.HasReached(this.model.energyRecoverAfter) && this.model.currEnergy < (float)this.model.maxEnergy);
		}
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x0006EC04 File Offset: 0x0006CE04
	public GameObject Play(GameObject fxPrefab)
	{
		Transform transform = this.statusBar.statusImage.transform;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fxPrefab, transform);
		SRBehaviour.PlayFX(gameObject);
		return gameObject;
	}

	// Token: 0x04001C34 RID: 7220
	[Tooltip("GameObject containing an FX that is active while the energy meeting is recharging. (optional)")]
	public GameObject onEnergyRechargingFX;

	// Token: 0x04001C35 RID: 7221
	[Tooltip("FX activated when the dash pad is active.")]
	public GameObject dashPadFX;

	// Token: 0x04001C36 RID: 7222
	private TimeDirector timeDirector;

	// Token: 0x04001C37 RID: 7223
	private PlayerModel model;

	// Token: 0x04001C38 RID: 7224
	private PlayerState state;

	// Token: 0x04001C39 RID: 7225
	private StatusBar statusBar;
}

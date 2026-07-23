using System;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class CaveTrigger : WeatherBlockingTrigger
{
	// Token: 0x06002446 RID: 9286 RVA: 0x0008BF07 File Offset: 0x0008A107
	public void Awake()
	{
		this.ambianceDir = SRSingleton<SceneContext>.Instance.AmbianceDirector;
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x0008BF1C File Offset: 0x0008A11C
	public void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		CaveTrigger.Listener interfaceComponent = col.gameObject.GetInterfaceComponent<CaveTrigger.Listener>();
		if (interfaceComponent != null)
		{
			interfaceComponent.OnCaveEnter(base.gameObject, this.affectsLighting, this.caveZone);
			if (interfaceComponent is PlayerCaveLighting)
			{
				this.playerListener = (PlayerCaveLighting)interfaceComponent;
			}
		}
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x0008BF70 File Offset: 0x0008A170
	public void OnTriggerExit(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		CaveTrigger.Listener interfaceComponent = col.gameObject.GetInterfaceComponent<CaveTrigger.Listener>();
		if (interfaceComponent != null)
		{
			interfaceComponent.OnCaveExit(base.gameObject, this.affectsLighting, this.caveZone);
			if (interfaceComponent == this.playerListener)
			{
				this.playerListener = null;
			}
		}
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x0008BFBD File Offset: 0x0008A1BD
	public void OnDisable()
	{
		if (this.playerListener != null)
		{
			this.playerListener.OnCaveExit(base.gameObject, this.affectsLighting, this.caveZone);
			this.playerListener = null;
		}
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x0008BFF4 File Offset: 0x0008A1F4
	public void Update()
	{
		if (this.playerListener != null && this.triggerness < 1f)
		{
			this.triggerness = Mathf.Min(1f, this.triggerness + Time.deltaTime / this.ambianceDir.zoneSettingTransitionTime);
		}
		else if (this.playerListener == null && this.triggerness > 0f)
		{
			this.triggerness = Mathf.Max(0f, this.triggerness - Time.deltaTime / this.ambianceDir.zoneSettingTransitionTime);
		}
		for (int i = 0; i < this.lights.Length; i++)
		{
			this.lights[i].SetTriggerness(this, this.triggerness);
		}
	}

	// Token: 0x04002353 RID: 9043
	public CaveLightController[] lights = new CaveLightController[0];

	// Token: 0x04002354 RID: 9044
	public bool affectsLighting = true;

	// Token: 0x04002355 RID: 9045
	public AmbianceDirector.Zone caveZone = AmbianceDirector.Zone.CAVE;

	// Token: 0x04002356 RID: 9046
	private PlayerCaveLighting playerListener;

	// Token: 0x04002357 RID: 9047
	private float triggerness;

	// Token: 0x04002358 RID: 9048
	private AmbianceDirector ambianceDir;

	// Token: 0x020006D1 RID: 1745
	public interface Listener
	{
		// Token: 0x0600244C RID: 9292
		void OnCaveEnter(GameObject gameObject, bool affectLighting, AmbianceDirector.Zone caveZone);

		// Token: 0x0600244D RID: 9293
		void OnCaveExit(GameObject gameObject, bool affectLighting, AmbianceDirector.Zone caveZone);
	}
}

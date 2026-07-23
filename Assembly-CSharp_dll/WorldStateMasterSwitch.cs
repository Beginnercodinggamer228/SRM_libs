using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020007CD RID: 1997
public class WorldStateMasterSwitch : IdHandler, TechActivator, MasterSwitchModel.Participant
{
	// Token: 0x060029C6 RID: 10694 RVA: 0x0009CFA2 File Offset: 0x0009B1A2
	private bool IsActivationBlocked()
	{
		return Time.time < this.blockSwitchActivationUntil;
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x0009CFB4 File Offset: 0x0009B1B4
	public void Awake()
	{
		this.switchHandler = new SwitchHandler(base.GetComponent<Animator>(), base.gameObject);
		SRSingleton<SceneContext>.Instance.GameModel.RegisterSwitch(base.id, base.gameObject);
		WorldStateSlaveSwitch[] array = this.slaves;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RegisterMaster(this);
		}
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x0009D011 File Offset: 0x0009B211
	public void OnEnable()
	{
		if (this.model != null)
		{
			this.switchHandler.SetState(this.model.state, true);
		}
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x0009D032 File Offset: 0x0009B232
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterSwitch(base.id);
		}
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x0009D056 File Offset: 0x0009B256
	public void InitModel(MasterSwitchModel model)
	{
		model.state = this.initState;
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x0009D064 File Offset: 0x0009B264
	public void SetModel(MasterSwitchModel model)
	{
		this.model = model;
		this.switchHandler.SetState(model.state, true);
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x0009D07F File Offset: 0x0009B27F
	protected override string IdPrefix()
	{
		return "switch";
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x0009D086 File Offset: 0x0009B286
	public void Update()
	{
		if (this.firstUpdate)
		{
			this.SetStateForAll(this.model.state, true);
			this.firstUpdate = false;
		}
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x0009D0AC File Offset: 0x0009B2AC
	public void Activate()
	{
		if (this.IsActivationBlocked())
		{
			return;
		}
		this.blockSwitchActivationUntil = Time.time + 2f;
		SwitchHandler.State state = (this.model.state == SwitchHandler.State.UP) ? SwitchHandler.State.DOWN : SwitchHandler.State.UP;
		this.SetStateForAll(state, false);
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x0009D0F0 File Offset: 0x0009B2F0
	private void SetStateForAll(SwitchHandler.State state, bool immediate)
	{
		this.model.state = state;
		this.switchHandler.SetState(state, immediate);
		WorldStateSlaveSwitch[] array = this.slaves;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetState(state, immediate);
		}
		SwitchHandler.Switchable[] array2 = this.switchables;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetState(state, immediate);
		}
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x040028FB RID: 10491
	public SwitchHandler.Switchable[] switchables;

	// Token: 0x040028FC RID: 10492
	public SwitchHandler.State initState;

	// Token: 0x040028FD RID: 10493
	public WorldStateSlaveSwitch[] slaves;

	// Token: 0x040028FE RID: 10494
	private bool firstUpdate = true;

	// Token: 0x040028FF RID: 10495
	private SwitchHandler switchHandler;

	// Token: 0x04002900 RID: 10496
	private float blockSwitchActivationUntil;

	// Token: 0x04002901 RID: 10497
	private MasterSwitchModel model;

	// Token: 0x04002902 RID: 10498
	private const float ACTIVATION_THROTTLE = 2f;
}

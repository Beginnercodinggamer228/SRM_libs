using System;
using UnityEngine;

// Token: 0x020007CE RID: 1998
public class WorldStateSlaveSwitch : MonoBehaviour, TechActivator
{
	// Token: 0x060029D2 RID: 10706 RVA: 0x0009D163 File Offset: 0x0009B363
	public void Awake()
	{
		this.switchHandler = new SwitchHandler(base.GetComponent<Animator>(), base.gameObject);
	}

	// Token: 0x060029D3 RID: 10707 RVA: 0x00003296 File Offset: 0x00001496
	public void Start()
	{
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x0009D17C File Offset: 0x0009B37C
	public void OnEnable()
	{
		this.SetState(this.currState, true);
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x0009D18B File Offset: 0x0009B38B
	public void Activate()
	{
		this.master.Activate();
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x0009D198 File Offset: 0x0009B398
	public void RegisterMaster(WorldStateMasterSwitch master)
	{
		this.master = master;
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x0009D1A1 File Offset: 0x0009B3A1
	internal void SetState(SwitchHandler.State state, bool immediate)
	{
		this.currState = state;
		if (base.isActiveAndEnabled)
		{
			this.switchHandler.SetState(state, immediate);
		}
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x04002903 RID: 10499
	private WorldStateMasterSwitch master;

	// Token: 0x04002904 RID: 10500
	private SwitchHandler switchHandler;

	// Token: 0x04002905 RID: 10501
	private SwitchHandler.State currState;
}

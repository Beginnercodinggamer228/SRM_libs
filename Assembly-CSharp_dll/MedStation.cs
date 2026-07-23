using System;
using UnityEngine;

// Token: 0x02000321 RID: 801
public class MedStation : MonoBehaviour
{
	// Token: 0x060010F6 RID: 4342 RVA: 0x00043DCC File Offset: 0x00041FCC
	public void Awake()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.waiter = base.GetComponentInParent<WaitForChargeup>();
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x00043DEA File Offset: 0x00041FEA
	public void OnTriggerEnter(Collider col)
	{
		if (PhysicsUtil.IsPlayerMainCollider(col))
		{
			this.counts++;
		}
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x00043E02 File Offset: 0x00042002
	public void OnTriggerExit(Collider col)
	{
		if (PhysicsUtil.IsPlayerMainCollider(col))
		{
			this.counts--;
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00043E1C File Offset: 0x0004201C
	public void Update()
	{
		if (this.waiter.IsWaiting())
		{
			return;
		}
		bool flag = false;
		if (this.counts > 0)
		{
			int num = Mathf.Max(this.playerState.GetMaxHealth() - this.playerState.GetCurrHealth(), this.playerState.GetCurrRad());
			int num2 = Mathf.CeilToInt((float)this.playerState.GetCurrEnergy() * this.healthPerEnergy);
			if (num > 0 && num2 > 0)
			{
				int num3 = Math.Min(Math.Min(num, num2), Mathf.CeilToInt(Time.deltaTime * this.healthPerSecond));
				if (num3 > 0)
				{
					flag = true;
					this.playerState.SpendEnergy((float)num3 / this.healthPerEnergy);
					this.playerState.Heal(num3);
					this.playerState.RemoveRads((float)num3);
				}
			}
		}
		if (flag != this.medFX.activeSelf)
		{
			this.medFX.SetActive(flag);
		}
	}

	// Token: 0x04000FE9 RID: 4073
	public GameObject medFX;

	// Token: 0x04000FEA RID: 4074
	public float healthPerEnergy = 1f;

	// Token: 0x04000FEB RID: 4075
	public float healthPerSecond = 100f;

	// Token: 0x04000FEC RID: 4076
	private int counts;

	// Token: 0x04000FED RID: 4077
	private PlayerState playerState;

	// Token: 0x04000FEE RID: 4078
	private WaitForChargeup waiter;
}

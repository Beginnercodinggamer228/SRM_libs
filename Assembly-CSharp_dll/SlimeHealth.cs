using System;
using UnityEngine;

// Token: 0x0200048A RID: 1162
public class SlimeHealth : SRBehaviour, Damageable
{
	// Token: 0x06001827 RID: 6183 RVA: 0x0005DA36 File Offset: 0x0005BC36
	public virtual void Start()
	{
		this.currHealth = this.maxHealth;
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x0005DA44 File Offset: 0x0005BC44
	public int GetCurrHealth()
	{
		return this.currHealth;
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x0005DA4C File Offset: 0x0005BC4C
	public bool Damage(int healthLoss, GameObject source)
	{
		this.currHealth -= healthLoss;
		if (this.onDamage != null)
		{
			this.onDamage(source);
		}
		if (this.currHealth <= 0)
		{
			this.currHealth = 0;
			return true;
		}
		SlimeFaceAnimator component = base.GetComponent<SlimeFaceAnimator>();
		if (component != null)
		{
			component.SetTrigger("triggerWince");
		}
		return false;
	}

	// Token: 0x0400178D RID: 6029
	public int maxHealth = 100;

	// Token: 0x0400178E RID: 6030
	private int currHealth;

	// Token: 0x0400178F RID: 6031
	public SlimeHealth.OnDamage onDamage;

	// Token: 0x0200048B RID: 1163
	// (Invoke) Token: 0x0600182C RID: 6188
	public delegate void OnDamage(GameObject damageSource);
}

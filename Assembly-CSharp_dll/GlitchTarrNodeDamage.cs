using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000718 RID: 1816
public class GlitchTarrNodeDamage : DamagePlayerOnTouch_Trigger
{
	// Token: 0x060025ED RID: 9709 RVA: 0x00091354 File Offset: 0x0008F554
	public override void OnEnable()
	{
		base.OnEnable();
		base.StartCoroutine(this.WaitForFixedUpdate());
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x00091369 File Offset: 0x0008F569
	public override void RegistryUpdate()
	{
		if (this.hasCheckedFirstCollision)
		{
			base.RegistryUpdate();
		}
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x00091379 File Offset: 0x0008F579
	private IEnumerator WaitForFixedUpdate()
	{
		this.hasCheckedFirstCollision = false;
		yield return new WaitForFixedUpdate();
		this.hasCheckedFirstCollision = true;
		if (this.damageGameObject != null)
		{
			this.nextTime = SRSingleton<SceneContext>.Instance.TimeDirector.HoursFromNow(SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.tarrNodeSpawnDamagePreventionTime * 0.016666668f);
		}
		yield break;
	}

	// Token: 0x0400254F RID: 9551
	private bool hasCheckedFirstCollision;
}

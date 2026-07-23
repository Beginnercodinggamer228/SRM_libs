using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class CFX_ShurikenThreadFix : MonoBehaviour
{
	// Token: 0x060001E4 RID: 484 RVA: 0x0000E564 File Offset: 0x0000C764
	private void OnEnable()
	{
		this.systems = base.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = this.systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].emission.enabled = false;
		}
		base.StartCoroutine("WaitFrame");
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000E5AF File Offset: 0x0000C7AF
	private IEnumerator WaitFrame()
	{
		yield return null;
		foreach (ParticleSystem particleSystem in this.systems)
		{
			particleSystem.emission.enabled = true;
			particleSystem.Play(true);
		}
		yield break;
	}

	// Token: 0x04000247 RID: 583
	private ParticleSystem[] systems;
}

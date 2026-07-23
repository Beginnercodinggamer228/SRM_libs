using System;
using UnityEngine;

// Token: 0x020006DC RID: 1756
public class DeactivateBasedOnGadgetMode : MonoBehaviour
{
	// Token: 0x060024A5 RID: 9381 RVA: 0x0008D3EA File Offset: 0x0008B5EA
	public void Awake()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x0008D3FC File Offset: 0x0008B5FC
	public void Update()
	{
		this.toDeactivate.SetActive(this.playerState.InGadgetMode ^ this.activateOnModeOff);
	}

	// Token: 0x04002399 RID: 9113
	public GameObject toDeactivate;

	// Token: 0x0400239A RID: 9114
	public bool activateOnModeOff;

	// Token: 0x0400239B RID: 9115
	private PlayerState playerState;
}

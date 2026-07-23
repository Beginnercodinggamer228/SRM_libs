using System;
using UnityEngine;

// Token: 0x02000388 RID: 904
public class SetInputMode : MonoBehaviour
{
	// Token: 0x060012D4 RID: 4820 RVA: 0x00049CBD File Offset: 0x00047EBD
	public void Awake()
	{
		this.input = SRInput.Instance;
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x00049CCA File Offset: 0x00047ECA
	public void OnEnable()
	{
		this.input.SetInputMode(this.mode, base.gameObject.GetInstanceID());
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x00049CE8 File Offset: 0x00047EE8
	public void OnDisable()
	{
		this.input.ClearInputMode(base.gameObject.GetInstanceID());
	}

	// Token: 0x040011CD RID: 4557
	[Tooltip("InputMode to set.")]
	public SRInput.InputMode mode;

	// Token: 0x040011CE RID: 4558
	private SRInput input;
}

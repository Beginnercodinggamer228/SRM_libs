using System;
using UnityEngine;

// Token: 0x020000B9 RID: 185
[ExecuteInEditMode]
[RequireComponent(typeof(SECTR_Member))]
[AddComponentMenu("")]
public class SECTR_Culler : MonoBehaviour
{
	// Token: 0x0600043E RID: 1086 RVA: 0x00019E78 File Offset: 0x00018078
	private void OnEnable()
	{
		this.cachedMember = base.GetComponent<SECTR_Member>();
		this.cachedMember.ChildCulling = (this.CullEachChild ? SECTR_Member.ChildCullModes.Individual : SECTR_Member.ChildCullModes.Group);
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00003296 File Offset: 0x00001496
	private void OnDisable()
	{
	}

	// Token: 0x0400042D RID: 1069
	private SECTR_Member cachedMember;

	// Token: 0x0400042E RID: 1070
	[SECTR_ToolTip("Overrides the culling information on Member.")]
	public bool CullEachChild;
}

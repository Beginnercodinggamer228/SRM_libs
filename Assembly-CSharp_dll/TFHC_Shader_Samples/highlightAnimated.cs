using System;
using UnityEngine;

namespace TFHC_Shader_Samples
{
	// Token: 0x02000C2A RID: 3114
	public class highlightAnimated : MonoBehaviour
	{
		// Token: 0x060057F8 RID: 22520 RVA: 0x00109737 File Offset: 0x00107937
		private void Start()
		{
			this.mat = base.GetComponent<Renderer>().material;
		}

		// Token: 0x060057F9 RID: 22521 RVA: 0x0010974A File Offset: 0x0010794A
		private void OnMouseEnter()
		{
			this.switchhighlighted(true);
		}

		// Token: 0x060057FA RID: 22522 RVA: 0x00109753 File Offset: 0x00107953
		private void OnMouseExit()
		{
			this.switchhighlighted(false);
		}

		// Token: 0x060057FB RID: 22523 RVA: 0x0010975C File Offset: 0x0010795C
		private void switchhighlighted(bool highlighted)
		{
			this.mat.SetFloat("_Highlighted", highlighted ? 1f : 0f);
		}

		// Token: 0x0400437C RID: 17276
		private Material mat;
	}
}

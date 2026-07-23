using System;
using UnityEngine;

namespace TFHC_ForceShield_Shader_Sample
{
	// Token: 0x02000C2B RID: 3115
	public class ForceShieldDestroyBall : MonoBehaviour
	{
		// Token: 0x060057FD RID: 22525 RVA: 0x0010977D File Offset: 0x0010797D
		private void Start()
		{
			UnityEngine.Object.Destroy(base.gameObject, this.lifetime);
		}

		// Token: 0x0400437D RID: 17277
		public float lifetime = 5f;
	}
}

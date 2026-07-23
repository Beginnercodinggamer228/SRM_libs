using System;
using UnityEngine;
using UnityEngine.Sprites;

// Token: 0x0200000E RID: 14
[ExecuteInEditMode]
public class SpriteMaskController : MonoBehaviour
{
	// Token: 0x06000043 RID: 67 RVA: 0x0000308E File Offset: 0x0000128E
	private void OnEnable()
	{
		this.m_spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.m_uvs = DataUtility.GetInnerUV(this.m_spriteRenderer.sprite);
		this.m_spriteRenderer.sharedMaterial.SetVector("_CustomUVS", this.m_uvs);
	}

	// Token: 0x0400002A RID: 42
	private SpriteRenderer m_spriteRenderer;

	// Token: 0x0400002B RID: 43
	private Vector4 m_uvs;
}

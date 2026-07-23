using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
[ExecuteInEditMode]
public class Colorizer : MonoBehaviour
{
	// Token: 0x06000182 RID: 386 RVA: 0x00003296 File Offset: 0x00001496
	private void Start()
	{
	}

	// Token: 0x06000183 RID: 387 RVA: 0x0000B519 File Offset: 0x00009719
	private void Update()
	{
		if (this.oldColor != this.TintColor)
		{
			this.ChangeColor(base.gameObject, this.TintColor);
		}
		this.oldColor = this.TintColor;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000B54C File Offset: 0x0000974C
	private void ChangeColor(GameObject effect, Color color)
	{
		foreach (Renderer renderer in effect.GetComponentsInChildren<Renderer>())
		{
			Material material = this.UseInstanceWhenNotEditorMode ? renderer.material : renderer.sharedMaterial;
			if (!(material == null) && material.HasProperty("_TintColor"))
			{
				Color color2 = material.GetColor("_TintColor");
				color.a = color2.a;
				material.SetColor("_TintColor", color);
			}
		}
		Light componentInChildren = effect.GetComponentInChildren<Light>();
		if (componentInChildren != null)
		{
			componentInChildren.color = color;
		}
	}

	// Token: 0x040001B3 RID: 435
	public Color TintColor;

	// Token: 0x040001B4 RID: 436
	public bool UseInstanceWhenNotEditorMode = true;

	// Token: 0x040001B5 RID: 437
	private Color oldColor;
}

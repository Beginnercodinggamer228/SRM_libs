using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006AF RID: 1711
public class UpdateMaterialUnscaledTime : MonoBehaviour
{
	// Token: 0x060023A6 RID: 9126 RVA: 0x0008A188 File Offset: 0x00088388
	public void Awake()
	{
		this.unscaledTimeVarId = Shader.PropertyToID("_UnscaledTime");
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length];
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				bool flag = false;
				for (int k = 0; k < this.mats.Length; k++)
				{
					if (sharedMaterials[j] == this.mats[k])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Material material = new Material(sharedMaterials[j]);
					this.adjustedMats.Add(material);
					array[j] = material;
				}
				else
				{
					array[j] = sharedMaterials[j];
				}
			}
			renderer.materials = array;
		}
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x0008A250 File Offset: 0x00088450
	public void Update()
	{
		foreach (Material material in this.adjustedMats)
		{
			material.SetFloat(this.unscaledTimeVarId, Time.unscaledTime);
		}
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x0008A2AC File Offset: 0x000884AC
	public void OnDestroy()
	{
		foreach (Material instance in this.adjustedMats)
		{
			Destroyer.Destroy(instance, "UpdateMaterialUnscaledTime.OnDestroy");
		}
		this.adjustedMats.Clear();
	}

	// Token: 0x040022D1 RID: 8913
	public Material[] mats;

	// Token: 0x040022D2 RID: 8914
	private int unscaledTimeVarId;

	// Token: 0x040022D3 RID: 8915
	private List<Material> adjustedMats = new List<Material>();
}

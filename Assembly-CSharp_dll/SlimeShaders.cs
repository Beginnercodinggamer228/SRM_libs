using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class SlimeShaders : SRBehaviour
{
	// Token: 0x0600184A RID: 6218 RVA: 0x0005E140 File Offset: 0x0005C340
	public void Awake()
	{
		this.unscaledTimePropertyId = Shader.PropertyToID("UnscaledTime");
		foreach (Material material in this.defaultCloakableMaterials)
		{
			this.cloakableShaders.Add(material.shader);
		}
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x0005E188 File Offset: 0x0005C388
	public void Update()
	{
		Shader.SetGlobalFloat(this.unscaledTimePropertyId, Time.unscaledTime);
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x0005E19C File Offset: 0x0005C39C
	public void RegisterAdditionalMaterials(Material[] materials)
	{
		foreach (Material material in materials)
		{
			this.cloakableShaders.Add(material.shader);
		}
	}

	// Token: 0x040017C8 RID: 6088
	public Material cloakMaterial;

	// Token: 0x040017C9 RID: 6089
	[Tooltip("The default whitelist of materials that can be cloaked.")]
	public Material[] defaultCloakableMaterials;

	// Token: 0x040017CA RID: 6090
	public HashSet<Shader> cloakableShaders = new HashSet<Shader>();

	// Token: 0x040017CB RID: 6091
	private int AlphaPropertyId;

	// Token: 0x040017CC RID: 6092
	private int unscaledTimePropertyId;
}

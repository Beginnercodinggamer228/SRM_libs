using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005A9 RID: 1449
public class LangReplaceParticleMat : MonoBehaviour
{
	// Token: 0x06001E0C RID: 7692 RVA: 0x00072308 File Offset: 0x00070508
	public void Awake()
	{
		this.partSys = base.GetComponent<ParticleSystemRenderer>();
		this.msgDir = SRSingleton<GameContext>.Instance.MessageDirector;
		foreach (LangReplaceParticleMat.Entry entry in this.replacements)
		{
			this.replacementDict[entry.lang] = entry.mat;
		}
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x00072364 File Offset: 0x00070564
	public void OnEnable()
	{
		string key = this.msgDir.GetCultureLang().ToString();
		if (this.replacementDict.ContainsKey(key))
		{
			this.partSys.sharedMaterial = this.replacementDict[key];
		}
	}

	// Token: 0x04001D2B RID: 7467
	public LangReplaceParticleMat.Entry[] replacements;

	// Token: 0x04001D2C RID: 7468
	private ParticleSystemRenderer partSys;

	// Token: 0x04001D2D RID: 7469
	private MessageDirector msgDir;

	// Token: 0x04001D2E RID: 7470
	private Dictionary<string, Material> replacementDict = new Dictionary<string, Material>();

	// Token: 0x020005AA RID: 1450
	[Serializable]
	public class Entry
	{
		// Token: 0x04001D2F RID: 7471
		public string lang;

		// Token: 0x04001D30 RID: 7472
		public Material mat;
	}
}

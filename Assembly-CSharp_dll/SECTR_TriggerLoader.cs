using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B8 RID: 184
[AddComponentMenu("SECTR/Stream/SECTR Trigger Loader")]
public class SECTR_TriggerLoader : SECTR_Loader
{
	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000437 RID: 1079 RVA: 0x00019CE4 File Offset: 0x00017EE4
	public override bool Loaded
	{
		get
		{
			bool flag = true;
			int count = this.Sectors.Count;
			int num = 0;
			while (num < count && flag)
			{
				SECTR_Sector sectr_Sector = this.Sectors[num];
				if (sectr_Sector.Frozen)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component && !component.IsLoaded())
					{
						flag = false;
						break;
					}
				}
				num++;
			}
			return flag;
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00019D43 File Offset: 0x00017F43
	private void OnDisable()
	{
		this._RefChunks();
		this.loadedCount = 0;
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00019D52 File Offset: 0x00017F52
	private void OnTriggerEnter(Collider other)
	{
		if (this.loadedCount == 0)
		{
			this._RefChunks();
		}
		this.loadedCount++;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00019D70 File Offset: 0x00017F70
	private void OnTriggerExit(Collider other)
	{
		if (this.loadedCount > 0)
		{
			this.loadedCount--;
			if (this.loadedCount == 0 && this.UnloadOnExit)
			{
				this._UnrefChunks();
			}
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00019DA0 File Offset: 0x00017FA0
	private void _RefChunks()
	{
		if (!this.chunksReferenced)
		{
			int count = this.Sectors.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = this.Sectors[i];
				if (sectr_Sector)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component)
					{
						component.AddReference();
					}
				}
			}
			this.chunksReferenced = true;
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00019E00 File Offset: 0x00018000
	private void _UnrefChunks()
	{
		if (this.chunksReferenced)
		{
			int count = this.Sectors.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = this.Sectors[i];
				if (sectr_Sector)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component)
					{
						component.RemoveReference();
					}
				}
			}
			this.chunksReferenced = false;
		}
	}

	// Token: 0x04000429 RID: 1065
	protected int loadedCount;

	// Token: 0x0400042A RID: 1066
	protected bool chunksReferenced;

	// Token: 0x0400042B RID: 1067
	[SECTR_ToolTip("List of Sectors to load when entering this trigger.")]
	public List<SECTR_Sector> Sectors = new List<SECTR_Sector>();

	// Token: 0x0400042C RID: 1068
	[SECTR_ToolTip("Should the Sectors be unloaded when trigger is exited.")]
	public bool UnloadOnExit = true;
}

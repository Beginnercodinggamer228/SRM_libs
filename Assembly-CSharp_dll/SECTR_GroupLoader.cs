using System;
using System.Collections.Generic;

// Token: 0x020000AC RID: 172
public class SECTR_GroupLoader : SECTR_Loader
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060003F5 RID: 1013 RVA: 0x0001836C File Offset: 0x0001656C
	public override bool Loaded
	{
		get
		{
			bool result = true;
			int count = this.Sectors.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = this.Sectors[i];
				if (sectr_Sector && sectr_Sector.Frozen)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component && !component.IsLoaded())
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x000183D0 File Offset: 0x000165D0
	private void OnEnable()
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
					component.ReferenceChange += this.ChunkChanged;
				}
			}
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0001842C File Offset: 0x0001662C
	private void OnDisable()
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
					component.ReferenceChange -= this.ChunkChanged;
				}
			}
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00018488 File Offset: 0x00016688
	private void ChunkChanged(SECTR_Chunk source, bool loaded)
	{
		int count = this.Sectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sectr_Sector = this.Sectors[i];
			if (sectr_Sector)
			{
				SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
				if (component && component != source)
				{
					component.ReferenceChange -= this.ChunkChanged;
					if (loaded)
					{
						component.AddReference();
					}
					else
					{
						component.RemoveReference();
					}
					component.ReferenceChange += this.ChunkChanged;
				}
			}
		}
	}

	// Token: 0x040003E9 RID: 1001
	[SECTR_ToolTip("The Sectors to load and unload together.")]
	public List<SECTR_Sector> Sectors = new List<SECTR_Sector>();
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class AshSource : MonoBehaviour
{
	// Token: 0x0600241F RID: 9247 RVA: 0x0008B902 File Offset: 0x00089B02
	public virtual void Awake()
	{
		AshSource.allAshes.Add(this);
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x0008B90F File Offset: 0x00089B0F
	public virtual void OnDestroy()
	{
		AshSource.allAshes.Remove(this);
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public virtual bool Available()
	{
		return true;
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void ConsumeAsh()
	{
	}

	// Token: 0x04002339 RID: 9017
	public static List<AshSource> allAshes = new List<AshSource>();
}

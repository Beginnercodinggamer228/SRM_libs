using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class IdDirector : SRBehaviour, ISerializationCallbackReceiver
{
	// Token: 0x06000B28 RID: 2856 RVA: 0x0002F084 File Offset: 0x0002D284
	public string GetPersistenceIdentifier(IdHandler instance)
	{
		string result;
		if (this.persistenceDict.TryGetValue(instance, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0002F0A4 File Offset: 0x0002D2A4
	public void OnBeforeSerialize()
	{
		this.persistenceKeys = new List<IdHandler>(this.persistenceDict.Keys);
		this.persistenceValues = new List<string>(this.persistenceDict.Values);
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0002F0D4 File Offset: 0x0002D2D4
	public void OnAfterDeserialize()
	{
		this.persistenceDict = new Dictionary<IdHandler, string>();
		for (int i = 0; i < this.persistenceKeys.Count; i++)
		{
			IdHandler idHandler = this.persistenceKeys[i];
			if (idHandler != null)
			{
				this.persistenceDict[idHandler] = this.persistenceValues[i];
			}
		}
		this.persistenceKeys = null;
		this.persistenceValues = null;
	}

	// Token: 0x0400090A RID: 2314
	private Dictionary<IdHandler, string> persistenceDict = new Dictionary<IdHandler, string>();

	// Token: 0x0400090B RID: 2315
	[SerializeField]
	[HideInInspector]
	private List<IdHandler> persistenceKeys = new List<IdHandler>();

	// Token: 0x0400090C RID: 2316
	[SerializeField]
	[HideInInspector]
	private List<string> persistenceValues = new List<string>();
}

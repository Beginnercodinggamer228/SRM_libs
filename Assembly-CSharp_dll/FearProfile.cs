using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D0 RID: 976
[CreateAssetMenu(menuName = "Slimes/Behaviors/FearProfile")]
public class FearProfile : ScriptableObject
{
	// Token: 0x06001443 RID: 5187 RVA: 0x0004E5D0 File Offset: 0x0004C7D0
	private void OnEnable()
	{
		this.InitializeFearProfilesLookup();
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0004E5D8 File Offset: 0x0004C7D8
	private void InitializeFearProfilesLookup()
	{
		foreach (FearProfile.ThreatEntry threatEntry in this.threats)
		{
			this.threatsLookup.Add(threatEntry.id, threatEntry);
		}
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x0004E638 File Offset: 0x0004C838
	public float GetSearchRadius(Identifiable.Id id, float currentFearDrive)
	{
		return this.GetThreatEntry(id).GetSearchRadius(currentFearDrive);
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0004E658 File Offset: 0x0004C858
	public float DistToFearAdjust(Identifiable.Id id, float dist)
	{
		return this.GetThreatEntry(id).DistToFearAdjust(dist);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x0004E675 File Offset: 0x0004C875
	public IEnumerable<Identifiable.Id> GetThreateningIdentifiables()
	{
		return this.threatsLookup.Keys;
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0004E682 File Offset: 0x0004C882
	private FearProfile.ThreatEntry GetThreatEntry(Identifiable.Id id)
	{
		return this.threatsLookup[id];
	}

	// Token: 0x04001308 RID: 4872
	public List<FearProfile.ThreatEntry> threats;

	// Token: 0x04001309 RID: 4873
	private Dictionary<Identifiable.Id, FearProfile.ThreatEntry> threatsLookup = new Dictionary<Identifiable.Id, FearProfile.ThreatEntry>(Identifiable.idComparer);

	// Token: 0x020003D1 RID: 977
	[Serializable]
	public struct ThreatEntry
	{
		// Token: 0x0600144A RID: 5194 RVA: 0x0004E6A8 File Offset: 0x0004C8A8
		public float GetSearchRadius(float currentFearDrive)
		{
			return this.maxSearchRadius * currentFearDrive + this.minSearchRadius * (1f - currentFearDrive);
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x0004E6C1 File Offset: 0x0004C8C1
		public float DistToFearAdjust(float dist)
		{
			return this.minThreatFearPerSec + (this.maxSearchRadius - dist) / this.maxSearchRadius * (this.maxThreatFearPerSec - this.minThreatFearPerSec);
		}

		// Token: 0x0400130A RID: 4874
		public Identifiable.Id id;

		// Token: 0x0400130B RID: 4875
		public float minSearchRadius;

		// Token: 0x0400130C RID: 4876
		public float maxSearchRadius;

		// Token: 0x0400130D RID: 4877
		public float minThreatFearPerSec;

		// Token: 0x0400130E RID: 4878
		public float maxThreatFearPerSec;
	}
}

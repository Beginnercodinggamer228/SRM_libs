using System;
using System.Collections.Generic;

// Token: 0x020004B9 RID: 1209
public class TrackSlimeTypes : SRBehaviour
{
	// Token: 0x06001959 RID: 6489 RVA: 0x00062BFC File Offset: 0x00060DFC
	public void Awake()
	{
		this.trackingContainer = base.GetComponent<TrackContainedIdentifiables>();
		TrackContainedIdentifiables trackContainedIdentifiables = this.trackingContainer;
		trackContainedIdentifiables.OnIdentifiableEntered = (TrackContainedIdentifiables.IdentifiableEntered)Delegate.Combine(trackContainedIdentifiables.OnIdentifiableEntered, new TrackContainedIdentifiables.IdentifiableEntered(this.OnIdentifiableEntered));
		TrackContainedIdentifiables trackContainedIdentifiables2 = this.trackingContainer;
		trackContainedIdentifiables2.OnNewIdentifiableTypeEntered = (TrackContainedIdentifiables.NewIdentifiableTypeEntered)Delegate.Combine(trackContainedIdentifiables2.OnNewIdentifiableTypeEntered, new TrackContainedIdentifiables.NewIdentifiableTypeEntered(this.OnNewIdentifiableTypeEntered));
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x00062C64 File Offset: 0x00060E64
	public void OnDestroy()
	{
		TrackContainedIdentifiables trackContainedIdentifiables = this.trackingContainer;
		trackContainedIdentifiables.OnIdentifiableEntered = (TrackContainedIdentifiables.IdentifiableEntered)Delegate.Remove(trackContainedIdentifiables.OnIdentifiableEntered, new TrackContainedIdentifiables.IdentifiableEntered(this.OnIdentifiableEntered));
		TrackContainedIdentifiables trackContainedIdentifiables2 = this.trackingContainer;
		trackContainedIdentifiables2.OnNewIdentifiableTypeEntered = (TrackContainedIdentifiables.NewIdentifiableTypeEntered)Delegate.Remove(trackContainedIdentifiables2.OnNewIdentifiableTypeEntered, new TrackContainedIdentifiables.NewIdentifiableTypeEntered(this.OnNewIdentifiableTypeEntered));
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x00062CC0 File Offset: 0x00060EC0
	public void OnIdentifiableEntered(TrackContainedIdentifiables container, Identifiable ident)
	{
		HashSet<Identifiable.Id> identifiablesForMode = this.GetIdentifiablesForMode();
		if (this.trackPlayerEnteredSlimesStat && ident.id == Identifiable.Id.PLAYER)
		{
			this.workingList.Clear();
			container.GetTrackedItemsOfClass(identifiablesForMode, this.workingList);
			SRSingleton<SceneContext>.Instance.AchievementsDirector.MaybeUpdateMaxStat(AchievementsDirector.IntStat.ENTERED_CORRAL_SLIMES, this.workingList.Count);
			this.workingList.Clear();
		}
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x00062D28 File Offset: 0x00060F28
	public void OnNewIdentifiableTypeEntered(TrackContainedIdentifiables container, Identifiable ident)
	{
		this.workingSet.Clear();
		this.workingSet.UnionWith(this.GetIdentifiablesForMode());
		container.GetTrackedIdentifiableTypes(this.workingSet);
		SRSingleton<SceneContext>.Instance.AchievementsDirector.MaybeUpdateMaxStat(this.stat, this.workingSet.Count);
		this.workingSet.Clear();
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x00062D88 File Offset: 0x00060F88
	private HashSet<Identifiable.Id> GetIdentifiablesForMode()
	{
		TrackSlimeTypes.Mode mode = this.mode;
		if (mode == TrackSlimeTypes.Mode.SLIMES)
		{
			return Identifiable.EATERS_CLASS;
		}
		if (mode == TrackSlimeTypes.Mode.LARGOS)
		{
			return Identifiable.LARGO_CLASS;
		}
		return new HashSet<Identifiable.Id>();
	}

	// Token: 0x04001905 RID: 6405
	public TrackSlimeTypes.Mode mode;

	// Token: 0x04001906 RID: 6406
	public AchievementsDirector.IntStat stat;

	// Token: 0x04001907 RID: 6407
	public bool trackPlayerEnteredSlimesStat;

	// Token: 0x04001908 RID: 6408
	private HashSet<Identifiable.Id> workingSet = new HashSet<Identifiable.Id>();

	// Token: 0x04001909 RID: 6409
	private List<Identifiable> workingList = new List<Identifiable>();

	// Token: 0x0400190A RID: 6410
	private TrackContainedIdentifiables trackingContainer;

	// Token: 0x020004BA RID: 1210
	public enum Mode
	{
		// Token: 0x0400190C RID: 6412
		SLIMES,
		// Token: 0x0400190D RID: 6413
		LARGOS
	}
}

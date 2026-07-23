using System;

// Token: 0x02000161 RID: 353
public abstract class DroneProgramDestination : DroneProgram
{
	// Token: 0x06000796 RID: 1942
	public abstract int GetAvailableSpace(Identifiable.Id id);

	// Token: 0x06000797 RID: 1943
	public abstract DroneProgramDestination.FastForward_Response FastForward(Identifiable.Id id, bool overflow, double endTime, int maxFastForward);

	// Token: 0x06000798 RID: 1944 RVA: 0x00025E93 File Offset: 0x00024093
	public virtual bool HasAvailableSpace(Identifiable.Id id)
	{
		return this.GetAvailableSpace(id) > 0;
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x00025E9F File Offset: 0x0002409F
	public sealed override bool Relevancy()
	{
		throw new InvalidOperationException();
	}

	// Token: 0x0600079A RID: 1946
	public abstract bool Relevancy(bool overflow);

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x0600079B RID: 1947 RVA: 0x00025EA6 File Offset: 0x000240A6
	protected override DroneAnimator.Id animation
	{
		get
		{
			return DroneAnimator.Id.DEPOSIT;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x0600079C RID: 1948 RVA: 0x00025EA9 File Offset: 0x000240A9
	protected override DroneAnimatorState.Id animationStateBegin
	{
		get
		{
			return DroneAnimatorState.Id.DEPOSIT_BEGIN;
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x0600079D RID: 1949 RVA: 0x00025EAD File Offset: 0x000240AD
	protected override DroneAnimatorState.Id animationStateEnd
	{
		get
		{
			return DroneAnimatorState.Id.DEPOSIT_END;
		}
	}

	// Token: 0x040006F5 RID: 1781
	public Predicate<Identifiable.Id> predicate = (Identifiable.Id id) => false;

	// Token: 0x02000162 RID: 354
	public class FastForward_Response
	{
		// Token: 0x040006F6 RID: 1782
		public int deposits;

		// Token: 0x040006F7 RID: 1783
		public int currency;
	}
}

using System;
using UnityEngine;

// Token: 0x02000759 RID: 1881
public class PuzzleDoorLock : PuzzleSlotLockable
{
	// Token: 0x06002747 RID: 10055 RVA: 0x00095497 File Offset: 0x00093697
	public override void NotifySlotChanged(bool immediate = false)
	{
		if (base.ShouldUnlock() && this.door.CurrState == AccessDoor.State.LOCKED)
		{
			this.door.CurrState = AccessDoor.State.CLOSED;
		}
		base.NotifySlotChanged(immediate);
	}

	// Token: 0x0400270D RID: 9997
	[Tooltip("The door we control the lock on.")]
	public AccessDoor door;
}

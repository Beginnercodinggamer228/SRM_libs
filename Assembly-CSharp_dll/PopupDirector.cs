using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class PopupDirector : MonoBehaviour
{
	// Token: 0x06000F6C RID: 3948 RVA: 0x0003D3A8 File Offset: 0x0003B5A8
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0003D3BA File Offset: 0x0003B5BA
	public void InitForLevel()
	{
		this.popupQueue.Clear();
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x0003D3C7 File Offset: 0x0003B5C7
	public bool IsQueued(PopupDirector.PopupCreator creator)
	{
		return this.popupQueue.Contains(creator);
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x0003D3D5 File Offset: 0x0003B5D5
	public void QueueForPopup(PopupDirector.PopupCreator creator)
	{
		this.popupQueue.Enqueue(creator);
		this.MaybePopupNext();
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x0003D3EC File Offset: 0x0003B5EC
	public void MaybePopupNext()
	{
		if (SRSingleton<SceneContext>.Instance != null && this.popupQueue.Count > 0 && this.currPopup == null && this.suppressors <= 0)
		{
			PopupDirector.PopupCreator popupCreator = this.popupQueue.Dequeue();
			if (popupCreator.ShouldClear())
			{
				this.MaybePopupNext();
				return;
			}
			popupCreator.Create();
		}
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0003D446 File Offset: 0x0003B646
	public void CheckShouldClear()
	{
		if (this.currPopup != null && this.currPopup.ShouldClear())
		{
			Destroyer.Destroy(((Component)this.currPopup).gameObject, "PopupDirector.CheckShouldClear");
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0003D477 File Offset: 0x0003B677
	public void PopupActivated(PopupDirector.Popup popup)
	{
		if (this.currPopup != null)
		{
			Log.Warning("Popup arrived with already-active popup.", Array.Empty<object>());
		}
		this.currPopup = popup;
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x0003D498 File Offset: 0x0003B698
	public void PopupDeactivated(PopupDirector.Popup popup)
	{
		if (this.currPopup == popup && !this.quitting)
		{
			this.currPopup = null;
			this.timeDir.OnUnpause(new TimeDirector.OnUnpauseDelegate(this.OnUnpause));
			return;
		}
		Log.Warning("Popup deactivated, but wasn't current popup.", Array.Empty<object>());
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x0003D4E4 File Offset: 0x0003B6E4
	public void RegisterSuppressor()
	{
		this.suppressors++;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0003D4F4 File Offset: 0x0003B6F4
	public void UnregisterSuppressor()
	{
		this.suppressors--;
		if (this.suppressors <= 0)
		{
			this.MaybePopupNext();
		}
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0003D513 File Offset: 0x0003B713
	public void OnUnpause()
	{
		this.MaybePopupNext();
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0003D51B File Offset: 0x0003B71B
	public void OnApplicationQuit()
	{
		this.quitting = true;
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x0003D524 File Offset: 0x0003B724
	public void OnDestroy()
	{
		this.timeDir.ClearOnUnpause(new TimeDirector.OnUnpauseDelegate(this.OnUnpause));
	}

	// Token: 0x04000E4D RID: 3661
	private Queue<PopupDirector.PopupCreator> popupQueue = new Queue<PopupDirector.PopupCreator>();

	// Token: 0x04000E4E RID: 3662
	private PopupDirector.Popup currPopup;

	// Token: 0x04000E4F RID: 3663
	private bool quitting;

	// Token: 0x04000E50 RID: 3664
	private TimeDirector timeDir;

	// Token: 0x04000E51 RID: 3665
	private int suppressors;

	// Token: 0x020002D5 RID: 725
	public interface Popup
	{
		// Token: 0x06000F7A RID: 3962
		bool ShouldClear();
	}

	// Token: 0x020002D6 RID: 726
	public abstract class PopupCreator
	{
		// Token: 0x06000F7B RID: 3963
		public abstract override bool Equals(object other);

		// Token: 0x06000F7C RID: 3964
		public abstract override int GetHashCode();

		// Token: 0x06000F7D RID: 3965
		public abstract void Create();

		// Token: 0x06000F7E RID: 3966
		public abstract bool ShouldClear();
	}
}

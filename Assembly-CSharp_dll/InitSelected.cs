using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200059E RID: 1438
[RequireComponent(typeof(Selectable))]
public class InitSelected : MonoBehaviour
{
	// Token: 0x06001DE0 RID: 7648 RVA: 0x00071CB7 File Offset: 0x0006FEB7
	public void Awake()
	{
		this.selectable = base.GetComponent<Selectable>();
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x00071CC5 File Offset: 0x0006FEC5
	public void OnEnable()
	{
		this.selectable.Select();
		this.selectable.OnSelect(null);
		InitSelected.Current = this;
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x00071CE4 File Offset: 0x0006FEE4
	public void OnDisable()
	{
		this.selectable.OnDeselect(null);
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == base.gameObject)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		if (InitSelected.Current == this)
		{
			InitSelected.Current = null;
		}
	}

	// Token: 0x04001D07 RID: 7431
	private Selectable selectable;

	// Token: 0x04001D08 RID: 7432
	public static InitSelected Current;
}

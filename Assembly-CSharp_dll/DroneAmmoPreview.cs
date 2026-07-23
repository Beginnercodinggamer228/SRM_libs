using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000128 RID: 296
[RequireComponent(typeof(Drone))]
public class DroneAmmoPreview : MonoBehaviour
{
	// Token: 0x0600067A RID: 1658 RVA: 0x00022D8A File Offset: 0x00020F8A
	public void Start()
	{
		this.lookup = SRSingleton<GameContext>.Instance.LookupDirector;
		this.drone = base.GetComponent<Drone>();
		this.root.SetActive(false);
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00022DB4 File Offset: 0x00020FB4
	public void LateUpdate()
	{
		Identifiable.Id slotName = this.drone.ammo.GetSlotName();
		if (slotName == this.previous)
		{
			return;
		}
		this.root.SetActive(slotName > Identifiable.Id.NONE);
		Sprite sprite = this.root.activeSelf ? this.lookup.GetIcon(slotName) : null;
		Image[] array = this.images;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = sprite;
		}
		this.previous = slotName;
	}

	// Token: 0x0400060A RID: 1546
	[Tooltip("Root game object to enable/disable based off ammo state.")]
	public GameObject root;

	// Token: 0x0400060B RID: 1547
	[Tooltip("Image to update with the ammo preview.")]
	public Image[] images;

	// Token: 0x0400060C RID: 1548
	private Drone drone;

	// Token: 0x0400060D RID: 1549
	private Identifiable.Id previous;

	// Token: 0x0400060E RID: 1550
	private LookupDirector lookup;
}

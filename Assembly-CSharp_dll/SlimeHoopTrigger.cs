using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000782 RID: 1922
public class SlimeHoopTrigger : MonoBehaviour
{
	// Token: 0x0600282C RID: 10284 RVA: 0x00098690 File Offset: 0x00096890
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger)
		{
			Identifiable componentInParent = col.GetComponentInParent<Identifiable>();
			if (componentInParent != null && Identifiable.IsSlime(componentInParent.id) && col.transform.position.y > base.transform.position.y && col.GetComponent<Rigidbody>().velocity.y < 10f)
			{
				this.passingDownwards.Add(col);
			}
		}
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x00098708 File Offset: 0x00096908
	public void OnTriggerExit(Collider col)
	{
		if (!col.isTrigger)
		{
			Identifiable componentInParent = col.GetComponentInParent<Identifiable>();
			if (componentInParent != null && Identifiable.IsSlime(componentInParent.id) && this.passingDownwards.Contains(col))
			{
				this.passingDownwards.Remove(col);
				if (col.transform.position.y < base.transform.position.y)
				{
					this.hoop.AddScore();
				}
			}
		}
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x00098784 File Offset: 0x00096984
	public void Update()
	{
		for (int i = this.passingDownwards.Count - 1; i >= 0; i--)
		{
			if (this.passingDownwards[i] == null)
			{
				this.passingDownwards.RemoveAt(i);
			}
		}
	}

	// Token: 0x040027CC RID: 10188
	public SlimeHoop hoop;

	// Token: 0x040027CD RID: 10189
	public List<Collider> passingDownwards = new List<Collider>();
}

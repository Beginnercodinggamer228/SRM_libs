using System;
using UnityEngine;

// Token: 0x02000421 RID: 1057
public class PollenSource : SRBehaviour
{
	// Token: 0x06001604 RID: 5636 RVA: 0x00055610 File Offset: 0x00053810
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger)
		{
			Identifiable component = col.gameObject.GetComponent<Identifiable>();
			if (component != null && !Identifiable.IsAllergyFree(component.id))
			{
				SlimeEmotions component2 = col.gameObject.GetComponent<SlimeEmotions>();
				if (component2 != null)
				{
					component2.AddPollenSource();
				}
				this.CauseSneeze(col.gameObject);
			}
		}
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x00055670 File Offset: 0x00053870
	public void OnTriggerExit(Collider col)
	{
		if (!col.isTrigger)
		{
			Identifiable component = col.gameObject.GetComponent<Identifiable>();
			if (component != null && !Identifiable.IsAllergyFree(component.id))
			{
				SlimeEmotions component2 = col.gameObject.GetComponent<SlimeEmotions>();
				if (component2 != null)
				{
					component2.RemovePollenSource();
				}
			}
		}
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x000556C4 File Offset: 0x000538C4
	private void CauseSneeze(GameObject gameObject)
	{
		SlimeFaceAnimator component = gameObject.GetComponent<SlimeFaceAnimator>();
		if (component != null)
		{
			component.SetTrigger("triggerSneeze");
		}
	}
}

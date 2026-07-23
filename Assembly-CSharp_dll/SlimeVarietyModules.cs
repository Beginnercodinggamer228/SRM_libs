using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000498 RID: 1176
public class SlimeVarietyModules : SRBehaviour
{
	// Token: 0x06001886 RID: 6278 RVA: 0x0005F219 File Offset: 0x0005D419
	public void Assemble()
	{
		if (this.addedComponents.Count > 0)
		{
			Log.Error("Why are we assembling an already assembled slime? Skipping: " + base.gameObject.name, Array.Empty<object>());
			return;
		}
		this.MergeGeneralComponents();
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x0005F250 File Offset: 0x0005D450
	private void MergeGeneralComponents()
	{
		foreach (GameObject gameObject in this.slimeModules)
		{
			if (gameObject != null)
			{
				foreach (Component component in gameObject.GetComponents<Component>())
				{
					if (component is Collider || base.GetComponent(component.GetType()) == null)
					{
						this.addedComponents.Add(base.gameObject.AddComponent(component.GetType()).GetCopyOf(component));
					}
				}
				int childCount = gameObject.transform.childCount;
				for (int k = 0; k < childCount; k++)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject.transform.GetChild(k).gameObject);
					Vector3 localPosition = gameObject2.transform.localPosition;
					Quaternion localRotation = gameObject2.transform.localRotation;
					gameObject2.transform.parent = base.transform;
					gameObject2.transform.localPosition = localPosition;
					gameObject2.transform.localRotation = localRotation;
				}
			}
		}
		if (this.baseModule != null)
		{
			bool flag = base.GetComponent<RejectBaseNontriggerColliders>() != null;
			foreach (Component component2 in this.baseModule.GetComponents<Component>())
			{
				if ((component2 is Collider && (((Collider)component2).isTrigger || !flag)) || base.GetComponent(component2.GetType()) == null)
				{
					this.addedComponents.Add(base.gameObject.AddComponent(component2.GetType()).GetCopyOf(component2));
				}
			}
			int childCount2 = this.baseModule.transform.childCount;
			for (int l = 0; l < childCount2; l++)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.baseModule.transform.GetChild(l).gameObject);
				Vector3 localPosition2 = gameObject3.transform.localPosition;
				Quaternion localRotation2 = gameObject3.transform.localRotation;
				gameObject3.transform.parent = base.transform;
				gameObject3.transform.localPosition = localPosition2;
				gameObject3.transform.localRotation = localRotation2;
			}
		}
		base.GetComponent<SlimeSubbehaviourPlexer>().CollectSubbehaviours();
	}

	// Token: 0x04001813 RID: 6163
	public GameObject baseModule;

	// Token: 0x04001814 RID: 6164
	public GameObject[] slimeModules;

	// Token: 0x04001815 RID: 6165
	private List<Component> addedComponents = new List<Component>();
}

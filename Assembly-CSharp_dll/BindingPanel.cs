using System;
using System.Collections.Generic;
using InControl;
using TMPro;
using UnityEngine;

// Token: 0x02000547 RID: 1351
public class BindingPanel
{
	// Token: 0x06001C32 RID: 7218 RVA: 0x0006B9A4 File Offset: 0x00069BA4
	public static GameObject CreateBindingLine(string label, PlayerAction action, GameObject bindingLineObj, MessageBundle uiBundle, Dictionary<BindingLineUI, string> labelKeyDict, BindingLineUI.DisableDelegate disableDelegate)
	{
		bindingLineObj.transform.Find("ActionText").gameObject.GetComponent<TMP_Text>().text = uiBundle.Xlate(label);
		BindingLineUI component = bindingLineObj.GetComponent<BindingLineUI>();
		component.action = action;
		if (disableDelegate != null)
		{
			component.disableDelegate = disableDelegate;
		}
		labelKeyDict[component] = label;
		return bindingLineObj;
	}
}

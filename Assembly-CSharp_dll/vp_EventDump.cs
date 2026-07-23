using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x020007FF RID: 2047
public class vp_EventDump
{
	// Token: 0x06002B0D RID: 11021 RVA: 0x000A2390 File Offset: 0x000A0590
	public static string Dump(vp_EventHandler handler, string[] eventTypes)
	{
		string text = "";
		foreach (string a in eventTypes)
		{
			if (!(a == "vp_Message"))
			{
				if (!(a == "vp_Attempt"))
				{
					if (!(a == "vp_Value"))
					{
						if (a == "vp_Activity")
						{
							text += vp_EventDump.DumpEventsOfType("vp_Activity", (eventTypes.Length > 1) ? "ACTIVITIES:\n\n" : "", handler);
						}
					}
					else
					{
						text += vp_EventDump.DumpEventsOfType("vp_Value", (eventTypes.Length > 1) ? "VALUES:\n\n" : "", handler);
					}
				}
				else
				{
					text += vp_EventDump.DumpEventsOfType("vp_Attempt", (eventTypes.Length > 1) ? "ATTEMPTS:\n\n" : "", handler);
				}
			}
			else
			{
				text += vp_EventDump.DumpEventsOfType("vp_Message", (eventTypes.Length > 1) ? "MESSAGES:\n\n" : "", handler);
			}
		}
		return text;
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x000A248D File Offset: 0x000A068D
	private static string DumpEventsOfType(string type, string caption, vp_EventHandler handler)
	{
		return "Dumping Disabled";
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x000A2494 File Offset: 0x000A0694
	private static string DumpEventListeners(object e, string[] invokers)
	{
		Type type = e.GetType();
		string text = "";
		foreach (string text2 in invokers)
		{
			FieldInfo field = type.GetField(text2);
			if (field == null)
			{
				return "";
			}
			Delegate @delegate = (Delegate)field.GetValue(e);
			string[] array = null;
			if (@delegate != null)
			{
				array = vp_EventDump.GetMethodNames(@delegate.GetInvocationList());
			}
			text += "\t\t\t\t";
			if (type.ToString().Contains("vp_Value"))
			{
				if (!(text2 == "Get"))
				{
					if (!(text2 == "Set"))
					{
						text += "Unsupported listener: ";
					}
					else
					{
						text += "Set";
					}
				}
				else
				{
					text += "Get";
				}
			}
			else if (type.ToString().Contains("vp_Attempt"))
			{
				text += "Try";
			}
			else if (type.ToString().Contains("vp_Message"))
			{
				text += "Send";
			}
			else if (type.ToString().Contains("vp_Activity"))
			{
				if (!(text2 == "StartConditions"))
				{
					if (!(text2 == "StopConditions"))
					{
						if (!(text2 == "StartCallbacks"))
						{
							if (!(text2 == "StopCallbacks"))
							{
								if (!(text2 == "FailStartCallbacks"))
								{
									if (!(text2 == "FailStopCallbacks"))
									{
										text += "Unsupported listener: ";
									}
									else
									{
										text += "FailStop";
									}
								}
								else
								{
									text += "FailStart";
								}
							}
							else
							{
								text += "Stop";
							}
						}
						else
						{
							text += "Start";
						}
					}
					else
					{
						text += "TryStop";
					}
				}
				else
				{
					text += "TryStart";
				}
			}
			else
			{
				text += "Unsupported listener";
			}
			if (array != null)
			{
				if (array.Length > 2)
				{
					text += ":\n";
				}
				else
				{
					text += ": ";
				}
				text += vp_EventDump.DumpDelegateNames(array);
			}
		}
		return text;
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000A26CC File Offset: 0x000A08CC
	private static string[] GetMethodNames(Delegate[] list)
	{
		list = vp_EventDump.RemoveDelegatesFromList(list);
		string[] array = new string[list.Length];
		if (list.Length == 1)
		{
			array[0] = ((list[0].Target == null) ? "" : ("(" + list[0].Target + ") ")) + list[0].Method.Name;
		}
		else
		{
			for (int i = 1; i < list.Length; i++)
			{
				array[i] = ((list[i].Target == null) ? "" : ("(" + list[i].Target + ") ")) + list[i].Method.Name;
			}
		}
		return array;
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000A277C File Offset: 0x000A097C
	private static Delegate[] RemoveDelegatesFromList(Delegate[] list)
	{
		List<Delegate> list2 = new List<Delegate>(list);
		for (int i = list2.Count - 1; i > -1; i--)
		{
			if (list2[i] != null && list2[i].Method.Name.Contains("m_"))
			{
				list2.RemoveAt(i);
			}
		}
		return list2.ToArray();
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x000A27D8 File Offset: 0x000A09D8
	private static string DumpDelegateNames(string[] array)
	{
		string text = "";
		foreach (string text2 in array)
		{
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + ((array.Length > 2) ? "\t\t\t\t\t\t\t" : "") + text2 + "\n";
			}
		}
		return text;
	}
}

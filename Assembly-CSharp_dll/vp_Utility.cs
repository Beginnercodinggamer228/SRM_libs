using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

// Token: 0x0200084A RID: 2122
public static class vp_Utility
{
	// Token: 0x06002C99 RID: 11417 RVA: 0x000A8334 File Offset: 0x000A6534
	[Obsolete("Please use 'vp_MathUtility.NaNSafeFloat' instead.")]
	public static float NaNSafeFloat(float value, float prevValue = 0f)
	{
		return vp_MathUtility.NaNSafeFloat(value, prevValue);
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000A833D File Offset: 0x000A653D
	[Obsolete("Please use 'vp_MathUtility.NaNSafeVector2' instead.")]
	public static Vector2 NaNSafeVector2(Vector2 vector, Vector2 prevVector = default(Vector2))
	{
		return vp_MathUtility.NaNSafeVector2(vector, prevVector);
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000A8346 File Offset: 0x000A6546
	[Obsolete("Please use 'vp_MathUtility.NaNSafeVector3' instead.")]
	public static Vector3 NaNSafeVector3(Vector3 vector, Vector3 prevVector = default(Vector3))
	{
		return vp_MathUtility.NaNSafeVector3(vector, prevVector);
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000A834F File Offset: 0x000A654F
	[Obsolete("Please use 'vp_MathUtility.NaNSafeQuaternion' instead.")]
	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, Quaternion prevQuaternion = default(Quaternion))
	{
		return vp_MathUtility.NaNSafeQuaternion(quaternion, prevQuaternion);
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000A8358 File Offset: 0x000A6558
	[Obsolete("Please use 'vp_MathUtility.SnapToZero' instead.")]
	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{
		return vp_MathUtility.SnapToZero(value, epsilon);
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000A8361 File Offset: 0x000A6561
	[Obsolete("Please use 'vp_MathUtility.SnapToZero' instead.")]
	public static float SnapToZero(float value, float epsilon = 0.0001f)
	{
		return vp_MathUtility.SnapToZero(value, epsilon);
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000A836A File Offset: 0x000A656A
	[Obsolete("Please use 'vp_MathUtility.ReduceDecimals' instead.")]
	public static float ReduceDecimals(float value, float factor = 1000f)
	{
		return vp_MathUtility.ReduceDecimals(value, factor);
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x000A8373 File Offset: 0x000A6573
	[Obsolete("Please use 'vp_3DUtility.HorizontalVector' instead.")]
	public static Vector3 HorizontalVector(Vector3 value)
	{
		return vp_3DUtility.HorizontalVector(value);
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x000A837C File Offset: 0x000A657C
	public static string GetErrorLocation(int level = 1, bool showOnlyLast = false)
	{
		StackTrace stackTrace = new StackTrace();
		string text = "";
		string text2 = "";
		for (int i = stackTrace.FrameCount - 1; i > level; i--)
		{
			if (i < stackTrace.FrameCount - 1)
			{
				text += " --> ";
			}
			StackFrame frame = stackTrace.GetFrame(i);
			if (frame.GetMethod().DeclaringType.ToString() == text2)
			{
				text = "";
			}
			text2 = frame.GetMethod().DeclaringType.ToString();
			text = text + text2 + ":" + frame.GetMethod().Name;
		}
		if (showOnlyLast)
		{
			try
			{
				text = text.Substring(text.LastIndexOf(" --> "));
				text = text.Replace(" --> ", "");
			}
			catch
			{
			}
		}
		return text;
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x000A8454 File Offset: 0x000A6654
	public static string GetTypeAlias(Type type)
	{
		string result = "";
		if (!vp_Utility.m_TypeAliases.TryGetValue(type, out result))
		{
			return type.ToString();
		}
		return result;
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x000A847E File Offset: 0x000A667E
	public static void Activate(GameObject obj, bool activate = true)
	{
		obj.SetActive(activate);
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x000A8487 File Offset: 0x000A6687
	public static bool IsActive(GameObject obj)
	{
		return obj.activeSelf;
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06002CA5 RID: 11429 RVA: 0x000A848F File Offset: 0x000A668F
	// (set) Token: 0x06002CA6 RID: 11430 RVA: 0x000A849C File Offset: 0x000A669C
	public static bool LockCursor
	{
		get
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}
		set
		{
			Cursor.visible = (!value && !vp_Utility.usingGamepad);
			Cursor.lockState = (value ? CursorLockMode.Locked : CursorLockMode.None);
		}
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000A84BD File Offset: 0x000A66BD
	public static void SetUsingGamepad(bool usingGamepad)
	{
		vp_Utility.usingGamepad = usingGamepad;
		vp_Utility.LockCursor = vp_Utility.LockCursor;
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x000A84D0 File Offset: 0x000A66D0
	public static void RandomizeList<T>(this List<T> list)
	{
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			int index = UnityEngine.Random.Range(i, count);
			T value = list[i];
			list[i] = list[index];
			list[index] = value;
		}
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x000A8516 File Offset: 0x000A6716
	public static T RandomObject<T>(this List<T> list)
	{
		List<T> list2 = new List<T>();
		list2.AddRange(list);
		list2.RandomizeList<T>();
		return list2.FirstOrDefault<T>();
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x000A852F File Offset: 0x000A672F
	public static List<T> ChildComponentsToList<T>(this Transform t) where T : Component
	{
		return t.GetComponentsInChildren<T>().ToList<T>();
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x000A853C File Offset: 0x000A673C
	public static bool IsDescendant(Transform descendant, Transform potentialAncestor)
	{
		return !(descendant == null) && !(potentialAncestor == null) && !(descendant.parent == descendant) && (descendant.parent == potentialAncestor || vp_Utility.IsDescendant(descendant.parent, potentialAncestor));
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x000A858B File Offset: 0x000A678B
	public static Component GetParent(Component target)
	{
		if (target == null)
		{
			return null;
		}
		if (target != target.transform)
		{
			return target.transform;
		}
		return target.transform.parent;
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x000A85B8 File Offset: 0x000A67B8
	public static Transform GetTransformByNameInChildren(Transform trans, string name, bool includeInactive = false, bool subString = false)
	{
		name = name.ToLower();
		foreach (object obj in trans)
		{
			Transform transform = (Transform)obj;
			if (!subString)
			{
				if (transform.name.ToLower() == name && (includeInactive || transform.gameObject.activeInHierarchy))
				{
					return transform;
				}
			}
			else if (transform.name.ToLower().Contains(name) && (includeInactive || transform.gameObject.activeInHierarchy))
			{
				return transform;
			}
			Transform transformByNameInChildren = vp_Utility.GetTransformByNameInChildren(transform, name, includeInactive, subString);
			if (transformByNameInChildren != null)
			{
				return transformByNameInChildren;
			}
		}
		return null;
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x000A867C File Offset: 0x000A687C
	public static Transform GetTransformByNameInAncestors(Transform trans, string name, bool includeInactive = false, bool subString = false)
	{
		if (trans.parent == null)
		{
			return null;
		}
		name = name.ToLower();
		if (!subString)
		{
			if (trans.parent.name.ToLower() == name && (includeInactive || trans.gameObject.activeInHierarchy))
			{
				return trans.parent;
			}
		}
		else if (trans.parent.name.ToLower().Contains(name) && (includeInactive || trans.gameObject.activeInHierarchy))
		{
			return trans.parent;
		}
		Transform transformByNameInAncestors = vp_Utility.GetTransformByNameInAncestors(trans.parent, name, includeInactive, subString);
		if (transformByNameInAncestors != null)
		{
			return transformByNameInAncestors;
		}
		return null;
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x000A871D File Offset: 0x000A691D
	public static UnityEngine.Object Instantiate(UnityEngine.Object original)
	{
		return vp_Utility.Instantiate(original, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x000A872F File Offset: 0x000A692F
	public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation)
	{
		if (vp_PoolManager.Instance == null || !vp_PoolManager.Instance.enabled)
		{
			return UnityEngine.Object.Instantiate(original, position, rotation);
		}
		return vp_GlobalEventReturn<UnityEngine.Object, Vector3, Quaternion, UnityEngine.Object>.Send("vp_PoolManager Instantiate", original, position, rotation);
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x000A8760 File Offset: 0x000A6960
	public static void Destroy(UnityEngine.Object obj)
	{
		vp_Utility.Destroy(obj, 0f);
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x000A876D File Offset: 0x000A696D
	public static void Destroy(UnityEngine.Object obj, float t)
	{
		if (vp_PoolManager.Instance == null || !vp_PoolManager.Instance.enabled)
		{
			Destroyer.Destroy(obj, t, "vp_Utility.Destroy", false, false);
			return;
		}
		vp_GlobalEvent<UnityEngine.Object, float>.Send("vp_PoolManager Destroy", obj, t);
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06002CB3 RID: 11443 RVA: 0x000A87A4 File Offset: 0x000A69A4
	public static int UniqueID
	{
		get
		{
			int num;
			for (;;)
			{
				num = UnityEngine.Random.Range(0, 1000000000);
				if (!vp_Utility.m_UniqueIDs.ContainsKey(num))
				{
					break;
				}
				if (vp_Utility.m_UniqueIDs.Count >= 1000000000)
				{
					vp_Utility.ClearUniqueIDs();
					UnityEngine.Debug.LogWarning("Warning (vp_Utility.UniqueID) More than 1 billion unique IDs have been generated. This seems like an awful lot for a game client. Clearing dictionary and starting over!");
				}
			}
			vp_Utility.m_UniqueIDs.Add(num, 0);
			return num;
		}
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x000A87F9 File Offset: 0x000A69F9
	public static void ClearUniqueIDs()
	{
		vp_Utility.m_UniqueIDs.Clear();
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x000A8805 File Offset: 0x000A6A05
	public static int PositionToID(Vector3 position)
	{
		return (int)Mathf.Abs(position.x * 10000f + position.y * 1000f + position.z * 100f);
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x000A8833 File Offset: 0x000A6A33
	[Obsolete("Please use 'vp_AudioUtility.PlayRandomSound' instead.")]
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds, Vector2 pitchRange)
	{
		vp_AudioUtility.PlayRandomSound(audioSource, sounds, pitchRange);
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x000A883D File Offset: 0x000A6A3D
	[Obsolete("Please use 'vp_AudioUtility.PlayRandomSound' instead.")]
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds)
	{
		vp_AudioUtility.PlayRandomSound(audioSource, sounds);
	}

	// Token: 0x04002AA2 RID: 10914
	private static bool usingGamepad;

	// Token: 0x04002AA3 RID: 10915
	private static readonly Dictionary<Type, string> m_TypeAliases = new Dictionary<Type, string>
	{
		{
			typeof(void),
			"void"
		},
		{
			typeof(byte),
			"byte"
		},
		{
			typeof(sbyte),
			"sbyte"
		},
		{
			typeof(short),
			"short"
		},
		{
			typeof(ushort),
			"ushort"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(uint),
			"uint"
		},
		{
			typeof(long),
			"long"
		},
		{
			typeof(ulong),
			"ulong"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(object),
			"object"
		},
		{
			typeof(bool),
			"bool"
		},
		{
			typeof(char),
			"char"
		},
		{
			typeof(string),
			"string"
		},
		{
			typeof(Vector2),
			"Vector2"
		},
		{
			typeof(Vector3),
			"Vector3"
		},
		{
			typeof(Vector4),
			"Vector4"
		}
	};

	// Token: 0x04002AA4 RID: 10916
	private static Dictionary<int, int> m_UniqueIDs = new Dictionary<int, int>();
}

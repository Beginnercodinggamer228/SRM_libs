using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;

// Token: 0x020007EF RID: 2031
public sealed class vp_ComponentPreset
{
	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06002A9F RID: 10911 RVA: 0x000A04EC File Offset: 0x0009E6EC
	// (set) Token: 0x06002AA0 RID: 10912 RVA: 0x000A04F4 File Offset: 0x0009E6F4
	public Type ComponentType
	{
		get
		{
			return this.m_ComponentType;
		}
		set
		{
			this.m_ComponentType = value;
		}
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000A04FD File Offset: 0x0009E6FD
	public static string Save(Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.InitFromComponent(component);
		return vp_ComponentPreset.Save(vp_ComponentPreset, fullPath, false);
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000A0514 File Offset: 0x0009E714
	public static string Save(vp_ComponentPreset savePreset, string fullPath, bool isDifference = false)
	{
		vp_ComponentPreset.m_FullPath = fullPath;
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(vp_ComponentPreset.m_FullPath);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null)
		{
			if (vp_ComponentPreset.m_ComponentType != null)
			{
				if (vp_ComponentPreset.ComponentType != savePreset.ComponentType)
				{
					return string.Concat(new string[]
					{
						"'",
						vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath),
						"' has the WRONG component type: ",
						vp_ComponentPreset.ComponentType.ToString(),
						".\n\nDo you want to replace it with a ",
						savePreset.ComponentType.ToString(),
						"?"
					});
				}
				if (File.Exists(vp_ComponentPreset.m_FullPath))
				{
					if (isDifference)
					{
						return "This will update '" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' with only the values modified since pressing Play or setting a state.\n\nContinue?";
					}
					return "'" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' already exists.\n\nDo you want to replace it?";
				}
			}
			if (File.Exists(vp_ComponentPreset.m_FullPath))
			{
				return "'" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' has an UNKNOWN component type.\n\nDo you want to replace it?";
			}
		}
		vp_ComponentPreset.ClearTextFile();
		vp_ComponentPreset.Append("///////////////////////////////////////////////////////////");
		vp_ComponentPreset.Append("// Component Preset Script");
		vp_ComponentPreset.Append("///////////////////////////////////////////////////////////\n");
		vp_ComponentPreset.Append("ComponentType " + savePreset.ComponentType.Name);
		foreach (vp_ComponentPreset.Field field in savePreset.m_Fields)
		{
			string str = "";
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			string text;
			if (fieldFromHandle.FieldType == typeof(float))
			{
				text = string.Format("{0:0.#######}", (float)field.Args);
			}
			else if (fieldFromHandle.FieldType == typeof(Vector4))
			{
				Vector4 vector = (Vector4)field.Args;
				text = string.Concat(new string[]
				{
					string.Format("{0:0.#######}", vector.x),
					" ",
					string.Format("{0:0.#######}", vector.y),
					" ",
					string.Format("{0:0.#######}", vector.z),
					" ",
					string.Format("{0:0.#######}", vector.w)
				});
			}
			else if (fieldFromHandle.FieldType == typeof(Vector3))
			{
				Vector3 vector2 = (Vector3)field.Args;
				text = string.Concat(new string[]
				{
					string.Format("{0:0.#######}", vector2.x),
					" ",
					string.Format("{0:0.#######}", vector2.y),
					" ",
					string.Format("{0:0.#######}", vector2.z)
				});
			}
			else if (fieldFromHandle.FieldType == typeof(Vector2))
			{
				Vector2 vector3 = (Vector2)field.Args;
				text = string.Format("{0:0.#######}", vector3.x) + " " + string.Format("{0:0.#######}", vector3.y);
			}
			else if (fieldFromHandle.FieldType == typeof(int))
			{
				text = ((int)field.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(bool))
			{
				text = ((bool)field.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(string))
			{
				text = (string)field.Args;
			}
			else
			{
				str = "//";
				text = "<NOTE: Type '" + fieldFromHandle.FieldType.Name.ToString() + "' can't be saved to preset.>";
			}
			if (!string.IsNullOrEmpty(text) && fieldFromHandle.Name != "Persist")
			{
				vp_ComponentPreset.Append(str + fieldFromHandle.Name + " " + text);
			}
		}
		return null;
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000A09A0 File Offset: 0x0009EBA0
	public static string SaveDifference(vp_ComponentPreset initialStatePreset, Component modifiedComponent, string fullPath, vp_ComponentPreset diskPreset)
	{
		if (initialStatePreset.ComponentType != modifiedComponent.GetType())
		{
			vp_ComponentPreset.Error("Tried to save difference between different type components in 'SaveDifference'");
			return null;
		}
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.InitFromComponent(modifiedComponent);
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.m_ComponentType = vp_ComponentPreset.ComponentType;
		for (int i = 0; i < vp_ComponentPreset.m_Fields.Count; i++)
		{
			if (!initialStatePreset.m_Fields[i].Args.Equals(vp_ComponentPreset.m_Fields[i].Args))
			{
				vp_ComponentPreset2.m_Fields.Add(vp_ComponentPreset.m_Fields[i]);
			}
		}
		foreach (vp_ComponentPreset.Field field in diskPreset.m_Fields)
		{
			bool flag = true;
			foreach (vp_ComponentPreset.Field field2 in vp_ComponentPreset2.m_Fields)
			{
				if (field.FieldHandle == field2.FieldHandle)
				{
					flag = false;
				}
			}
			bool flag2 = false;
			foreach (vp_ComponentPreset.Field field3 in vp_ComponentPreset.m_Fields)
			{
				if (field.FieldHandle == field3.FieldHandle)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				flag = false;
			}
			if (flag)
			{
				vp_ComponentPreset2.m_Fields.Add(field);
			}
		}
		return vp_ComponentPreset.Save(vp_ComponentPreset2, fullPath, true);
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x00003296 File Offset: 0x00001496
	public void InitFromComponent(Component component)
	{
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x00025E60 File Offset: 0x00024060
	public static vp_ComponentPreset CreateFromComponent(Component component)
	{
		return null;
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public int TryMakeCompatibleWithComponent(vp_Component component)
	{
		return 0;
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public bool LoadTextStream(string fullPath)
	{
		return true;
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x000A0B58 File Offset: 0x0009ED58
	public static bool Load(vp_Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(fullPath);
		return vp_ComponentPreset.Apply(component, vp_ComponentPreset);
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x000A0B7C File Offset: 0x0009ED7C
	public bool LoadFromResources(string resourcePath)
	{
		vp_ComponentPreset.m_FullPath = resourcePath;
		TextAsset textAsset = Resources.Load(vp_ComponentPreset.m_FullPath) as TextAsset;
		if (textAsset == null)
		{
			vp_ComponentPreset.Error("Failed to read file. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		return this.LoadFromTextAsset(textAsset);
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x000A0BCC File Offset: 0x0009EDCC
	public static vp_ComponentPreset LoadFromResources(vp_Component component, string resourcePath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromResources(resourcePath);
		vp_ComponentPreset.Apply(component, vp_ComponentPreset);
		return vp_ComponentPreset;
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x000A0BF0 File Offset: 0x0009EDF0
	public bool LoadFromTextAsset(TextAsset file)
	{
		vp_ComponentPreset.m_FullPath = file.name;
		List<string> list = new List<string>();
		foreach (string item in file.text.Split(new char[]
		{
			'\n'
		}))
		{
			list.Add(item);
		}
		if (list == null)
		{
			vp_ComponentPreset.Error("Preset is empty. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		this.ParseLines(list);
		return true;
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x000A0C64 File Offset: 0x0009EE64
	public static vp_ComponentPreset LoadFromTextAsset(vp_Component component, TextAsset file)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromTextAsset(file);
		vp_ComponentPreset.Apply(component, vp_ComponentPreset);
		return vp_ComponentPreset;
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x00003296 File Offset: 0x00001496
	private static void Append(string str)
	{
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x00003296 File Offset: 0x00001496
	private static void ClearTextFile()
	{
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x000A0C88 File Offset: 0x0009EE88
	private void ParseLines(List<string> lines)
	{
		vp_ComponentPreset.m_LineNumber = 0;
		foreach (string str in lines)
		{
			vp_ComponentPreset.m_LineNumber++;
			string text = vp_ComponentPreset.RemoveComments(str);
			if (!string.IsNullOrEmpty(text) && !this.Parse(text))
			{
				return;
			}
		}
		vp_ComponentPreset.m_LineNumber = 0;
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x00013CC5 File Offset: 0x00011EC5
	private bool Parse(string line)
	{
		return true;
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000A0D00 File Offset: 0x0009EF00
	private string[] FindMovedParameter(string type, string field)
	{
		string[] result;
		if (!this.MovedParameters.TryGetValue(type + "." + field, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public static bool Apply(vp_Component component, vp_ComponentPreset preset)
	{
		return true;
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000A0D2C File Offset: 0x0009EF2C
	public static Type GetFileType(string fullPath)
	{
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(fullPath);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null && vp_ComponentPreset.m_ComponentType != null)
		{
			return vp_ComponentPreset.m_ComponentType;
		}
		return null;
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000A0D70 File Offset: 0x0009EF70
	public static Type GetFileTypeFromAsset(TextAsset asset)
	{
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromTextAsset(asset);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null && vp_ComponentPreset.m_ComponentType != null)
		{
			return vp_ComponentPreset.m_ComponentType;
		}
		return null;
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000A0DB4 File Offset: 0x0009EFB4
	private static object TokensToObject(FieldInfo field, string[] tokens)
	{
		if (field.FieldType == typeof(float))
		{
			return vp_ComponentPreset.ArgsToFloat(tokens);
		}
		if (field.FieldType == typeof(Vector4))
		{
			return vp_ComponentPreset.ArgsToVector4(tokens);
		}
		if (field.FieldType == typeof(Vector3))
		{
			return vp_ComponentPreset.ArgsToVector3(tokens);
		}
		if (field.FieldType == typeof(Vector2))
		{
			return vp_ComponentPreset.ArgsToVector2(tokens);
		}
		if (field.FieldType == typeof(int))
		{
			return vp_ComponentPreset.ArgsToInt(tokens);
		}
		if (field.FieldType == typeof(bool))
		{
			return vp_ComponentPreset.ArgsToBool(tokens);
		}
		if (field.FieldType == typeof(string))
		{
			return vp_ComponentPreset.ArgsToString(tokens);
		}
		return null;
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x000A0EB4 File Offset: 0x0009F0B4
	private static string RemoveComments(string str)
	{
		string text = "";
		for (int i = 0; i < str.Length; i++)
		{
			switch (vp_ComponentPreset.m_ReadMode)
			{
			case vp_ComponentPreset.ReadMode.Normal:
				if (str[i] == '/' && str[i + 1] == '*')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.BlockComment;
					i++;
				}
				else if (str[i] == '/' && str[i + 1] == '/')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.LineComment;
					i++;
				}
				else
				{
					text += str[i].ToString();
				}
				break;
			case vp_ComponentPreset.ReadMode.LineComment:
				if (i == str.Length - 1)
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.Normal;
				}
				break;
			case vp_ComponentPreset.ReadMode.BlockComment:
				if (str[i] == '*' && str[i + 1] == '/')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.Normal;
					i++;
				}
				break;
			}
		}
		return text;
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000A0F94 File Offset: 0x0009F194
	private static Vector4 ArgsToVector4(string[] args)
	{
		if (args.Length - 1 != 4)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector4.zero;
		}
		Vector4 result;
		try
		{
			result = new Vector4(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture), Convert.ToSingle(args[3], CultureInfo.InvariantCulture), Convert.ToSingle(args[4], CultureInfo.InvariantCulture));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				", ",
				args[3],
				", ",
				args[4],
				"'"
			}));
			return Vector4.zero;
		}
		return result;
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x000A1070 File Offset: 0x0009F270
	private static Vector3 ArgsToVector3(string[] args)
	{
		if (args.Length - 1 != 3)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector3.zero;
		}
		Vector3 result;
		try
		{
			result = new Vector3(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture), Convert.ToSingle(args[3], CultureInfo.InvariantCulture));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				", ",
				args[3],
				"'"
			}));
			return Vector3.zero;
		}
		return result;
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x000A1130 File Offset: 0x0009F330
	private static Vector2 ArgsToVector2(string[] args)
	{
		if (args.Length - 1 != 2)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector2.zero;
		}
		Vector2 result;
		try
		{
			result = new Vector2(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				"'"
			}));
			return Vector2.zero;
		}
		return result;
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x000A11D8 File Offset: 0x0009F3D8
	private static float ArgsToFloat(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0f;
		}
		float result;
		try
		{
			result = Convert.ToSingle(args[1], CultureInfo.InvariantCulture);
		}
		catch
		{
			vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
			return 0f;
		}
		return result;
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000A1250 File Offset: 0x0009F450
	private static int ArgsToInt(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0;
		}
		int result;
		try
		{
			result = Convert.ToInt32(args[1], CultureInfo.InvariantCulture);
		}
		catch
		{
			vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
			return 0;
		}
		return result;
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000A12C0 File Offset: 0x0009F4C0
	private static bool ArgsToBool(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return false;
		}
		if (args[1].ToLower() == "true")
		{
			return true;
		}
		if (args[1].ToLower() == "false")
		{
			return false;
		}
		vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
		return false;
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x000A1334 File Offset: 0x0009F534
	private static string ArgsToString(string[] args)
	{
		string text = "";
		for (int i = 1; i < args.Length; i++)
		{
			text += args[i];
			if (i < args.Length - 1)
			{
				text += " ";
			}
		}
		return text;
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000A1374 File Offset: 0x0009F574
	public Type GetFieldType(string fieldName)
	{
		Type result = null;
		foreach (vp_ComponentPreset.Field field in this.m_Fields)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			if (fieldFromHandle.Name == fieldName)
			{
				result = fieldFromHandle.FieldType;
			}
		}
		return result;
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000A13E4 File Offset: 0x0009F5E4
	public object GetFieldValue(string fieldName)
	{
		object result = null;
		foreach (vp_ComponentPreset.Field field in this.m_Fields)
		{
			if (FieldInfo.GetFieldFromHandle(field.FieldHandle).Name == fieldName)
			{
				result = field.Args;
			}
		}
		return result;
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000A1454 File Offset: 0x0009F654
	public static string ExtractFilenameFromPath(string path)
	{
		int num = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
		if (num == -1)
		{
			return path;
		}
		if (num == path.Length - 1)
		{
			return "";
		}
		return path.Substring(num + 1, path.Length - num - 1);
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000A14A4 File Offset: 0x0009F6A4
	private static void PresetError(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogError(string.Concat(new object[]
		{
			"Preset Error: ",
			vp_ComponentPreset.m_FullPath,
			" (at ",
			vp_ComponentPreset.m_LineNumber,
			") ",
			message
		}));
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000A14FC File Offset: 0x0009F6FC
	private static void PresetWarning(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"Preset Warning: ",
			vp_ComponentPreset.m_FullPath,
			" (at ",
			vp_ComponentPreset.m_LineNumber,
			") ",
			message
		}));
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x000A1552 File Offset: 0x0009F752
	private static void Error(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogError("Error: " + message);
	}

	// Token: 0x040029B2 RID: 10674
	private static string m_FullPath = null;

	// Token: 0x040029B3 RID: 10675
	private static int m_LineNumber = 0;

	// Token: 0x040029B4 RID: 10676
	public static bool LogErrors = true;

	// Token: 0x040029B5 RID: 10677
	private static vp_ComponentPreset.ReadMode m_ReadMode = vp_ComponentPreset.ReadMode.Normal;

	// Token: 0x040029B6 RID: 10678
	private Type m_ComponentType;

	// Token: 0x040029B7 RID: 10679
	private List<vp_ComponentPreset.Field> m_Fields = new List<vp_ComponentPreset.Field>();

	// Token: 0x040029B8 RID: 10680
	private Dictionary<string, string[]> MovedParameters = new Dictionary<string, string[]>
	{
		{
			"vp_FPCamera.MouseAcceleration",
			new string[]
			{
				"vp_FPInput",
				"MouseLookAcceleration"
			}
		},
		{
			"vp_FPCamera.MouseSensitivity",
			new string[]
			{
				"vp_FPInput",
				"MouseLookSensitivity"
			}
		},
		{
			"vp_FPCamera.MouseSmoothSteps",
			new string[]
			{
				"vp_FPInput",
				"MouseLookSmoothSteps"
			}
		},
		{
			"vp_FPCamera.MouseSmoothWeight",
			new string[]
			{
				"vp_FPInput",
				"MouseLookSmoothWeight"
			}
		},
		{
			"vp_FPCamera.MouseAccelerationThreshold",
			new string[]
			{
				"vp_FPInput",
				"MouseLookAccelerationThreshold"
			}
		},
		{
			"vp_FPInput.ForceCursor",
			new string[]
			{
				"vp_FPInput",
				"MouseCursorForced"
			}
		}
	};

	// Token: 0x020007F0 RID: 2032
	private enum ReadMode
	{
		// Token: 0x040029BA RID: 10682
		Normal,
		// Token: 0x040029BB RID: 10683
		LineComment,
		// Token: 0x040029BC RID: 10684
		BlockComment
	}

	// Token: 0x020007F1 RID: 2033
	private class Field
	{
		// Token: 0x06002AC6 RID: 10950 RVA: 0x000A1677 File Offset: 0x0009F877
		public Field(RuntimeFieldHandle fieldHandle, object args)
		{
			this.FieldHandle = fieldHandle;
			this.Args = args;
		}

		// Token: 0x040029BD RID: 10685
		public RuntimeFieldHandle FieldHandle;

		// Token: 0x040029BE RID: 10686
		public object Args;
	}
}

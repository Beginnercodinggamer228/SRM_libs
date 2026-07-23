using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Token: 0x02000497 RID: 1175
public static class SlimeVarietyModulesStatic
{
	// Token: 0x06001884 RID: 6276 RVA: 0x0005F010 File Offset: 0x0005D210
	public static T GetCopyOf<T>(this Component copyInto, T copyFrom) where T : Component
	{
		Type type = copyInto.GetType();
		if (type != copyFrom.GetType())
		{
			return default(T);
		}
		while (type != typeof(Component) && type != null)
		{
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingAttr))
			{
				if (!SlimeVarietyModulesStatic.SKIP_PROP_NAMES.Contains(propertyInfo.Name) && propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.Name != "material")
				{
					try
					{
						propertyInfo.SetValue(copyInto, propertyInfo.GetValue(copyFrom, null), null);
					}
					catch (Exception ex)
					{
						Log.Error(string.Concat(new object[]
						{
							"ZOMG! Cannot set property when copying component. ",
							propertyInfo,
							" err: ",
							ex
						}), Array.Empty<object>());
					}
				}
			}
			foreach (FieldInfo fieldInfo in type.GetFields(bindingAttr))
			{
				if (fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0 && (fieldInfo.IsPublic || fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Length != 0))
				{
					if (fieldInfo.FieldType.IsValueType)
					{
						fieldInfo.SetValue(copyInto, fieldInfo.GetValue(copyFrom));
					}
					else if (fieldInfo.FieldType.IsSerializable)
					{
						fieldInfo.SetValue(copyInto, ObjectCopier.Clone<object>(fieldInfo.GetValue(copyFrom)));
					}
					else
					{
						fieldInfo.SetValue(copyInto, fieldInfo.GetValue(copyFrom));
					}
				}
			}
			type = type.BaseType;
		}
		return copyInto as T;
	}

	// Token: 0x04001812 RID: 6162
	private static readonly string[] SKIP_PROP_NAMES = new string[]
	{
		"sleepAngularVelocity",
		"sleepVelocity",
		"useConeFriction"
	};
}

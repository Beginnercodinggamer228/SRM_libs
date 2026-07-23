using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;

namespace MonoMod.Utils
{
	// Token: 0x02000432 RID: 1074
	public sealed class DMDEmitDynamicMethodGenerator : DMDGenerator<DMDEmitDynamicMethodGenerator>
	{
		// Token: 0x06001695 RID: 5781 RVA: 0x0004A48C File Offset: 0x0004868C
		protected override MethodInfo _Generate(DynamicMethodDefinition dmd, object context)
		{
			MethodBase originalMethod = dmd.OriginalMethod;
			MethodDefinition definition = dmd.Definition;
			Type[] array;
			if (originalMethod != null)
			{
				ParameterInfo[] parameters = originalMethod.GetParameters();
				int num = 0;
				if (!originalMethod.IsStatic)
				{
					num++;
					array = new Type[parameters.Length + 1];
					array[0] = originalMethod.GetThisParamType();
				}
				else
				{
					array = new Type[parameters.Length];
				}
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i + num] = parameters[i].ParameterType;
				}
			}
			else
			{
				int num2 = 0;
				if (definition.HasThis)
				{
					num2++;
					array = new Type[definition.Parameters.Count + 1];
					Type type4 = definition.DeclaringType.ResolveReflection();
					if (type4.IsValueType)
					{
						type4 = type4.MakeByRefType();
					}
					array[0] = type4;
				}
				else
				{
					array = new Type[definition.Parameters.Count];
				}
				for (int j = 0; j < definition.Parameters.Count; j++)
				{
					array[j + num2] = definition.Parameters[j].ParameterType.ResolveReflection();
				}
			}
			string text;
			if ((text = dmd.Name) == null)
			{
				text = "DMD<" + (((originalMethod != null) ? originalMethod.GetID(null, null, true, false, true) : null) ?? definition.GetID(null, null, true, true)) + ">";
			}
			string text2 = text;
			MethodInfo methodInfo = originalMethod as MethodInfo;
			Type type2;
			if ((type2 = ((methodInfo != null) ? methodInfo.ReturnType : null)) == null)
			{
				TypeReference returnType = definition.ReturnType;
				type2 = ((returnType != null) ? returnType.ResolveReflection() : null);
			}
			Type type3 = type2;
			MMDbgLog.Log(string.Format("new DynamicMethod: {0} {1}({2})", type3, text2, string.Join(",", array.Select(delegate(Type type)
			{
				if (type == null)
				{
					return null;
				}
				return type.ToString();
			}).ToArray<string>())));
			if (originalMethod != null)
			{
				string[] array2 = new string[7];
				array2[0] = "orig: ";
				int num3 = 1;
				MethodInfo methodInfo2 = originalMethod as MethodInfo;
				string text3;
				if (methodInfo2 == null)
				{
					text3 = null;
				}
				else
				{
					Type returnType2 = methodInfo2.ReturnType;
					text3 = ((returnType2 != null) ? returnType2.ToString() : null);
				}
				array2[num3] = (text3 ?? "NULL");
				array2[2] = " ";
				array2[3] = originalMethod.Name;
				array2[4] = "(";
				array2[5] = string.Join(",", originalMethod.GetParameters().Select(delegate(ParameterInfo arg)
				{
					string text4;
					if (arg == null)
					{
						text4 = null;
					}
					else
					{
						Type parameterType = arg.ParameterType;
						text4 = ((parameterType != null) ? parameterType.ToString() : null);
					}
					return text4 ?? "NULL";
				}).ToArray<string>());
				array2[6] = ")";
				MMDbgLog.Log(string.Concat(array2));
			}
			string[] array3 = new string[7];
			array3[0] = "mdef: ";
			int num4 = 1;
			TypeReference returnType3 = definition.ReturnType;
			array3[num4] = (((returnType3 != null) ? returnType3.ToString() : null) ?? "NULL");
			array3[2] = " ";
			array3[3] = text2;
			array3[4] = "(";
			array3[5] = string.Join(",", definition.Parameters.Select(delegate(ParameterDefinition arg)
			{
				string text4;
				if (arg == null)
				{
					text4 = null;
				}
				else
				{
					TypeReference parameterType = arg.ParameterType;
					text4 = ((parameterType != null) ? parameterType.ToString() : null);
				}
				return text4 ?? "NULL";
			}).ToArray<string>());
			array3[6] = ")";
			MMDbgLog.Log(string.Concat(array3));
			string name = text2;
			Type typeFromHandle = typeof(void);
			Type[] parameterTypes = array;
			Type owner;
			if ((owner = ((originalMethod != null) ? originalMethod.DeclaringType : null)) == null)
			{
				owner = (dmd.OwnerType ?? typeof(DynamicMethodDefinition));
			}
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeFromHandle, parameterTypes, owner, true);
			DMDEmitDynamicMethodGenerator._DynamicMethod_returnType.SetValue(dynamicMethod, type3);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			_DMDEmit.Generate(dmd, dynamicMethod, ilgenerator);
			return dynamicMethod;
		}

		// Token: 0x04000FC1 RID: 4033
		private static readonly FieldInfo _DynamicMethod_returnType = typeof(DynamicMethod).GetField("returnType", BindingFlags.Instance | BindingFlags.NonPublic) ?? typeof(DynamicMethod).GetField("m_returnType", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}

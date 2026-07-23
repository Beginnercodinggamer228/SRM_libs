using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004E RID: 78
[CreateAssetMenu(menuName = "ObjectPoolConfig")]
public class ObjectPoolConfig : ScriptableObject
{
	// Token: 0x06000160 RID: 352 RVA: 0x0000AACE File Offset: 0x00008CCE
	public IEnumerable<string> CheckPooledConfiguration()
	{
		int num;
		for (int ii = 0; ii < this.startupPools.Length; ii = num + 1)
		{
			ObjectPoolConfig.StartupPool startupPool = this.startupPools[ii];
			if (startupPool == null)
			{
				yield return string.Format("Pool {1}[{0}] is null.", ii, base.name);
			}
			else if (startupPool.prefab == null)
			{
				yield return string.Format("Pool {1}[{0}] has a null prefab.", ii, base.name);
			}
			else
			{
				string text = this.CheckForPooledParticleFX(startupPool.prefab, ii, !startupPool.doesNotSelfDestruct);
				if (!string.IsNullOrEmpty(text))
				{
					yield return text;
				}
			}
			num = ii;
		}
		yield break;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000AAE0 File Offset: 0x00008CE0
	private string CheckForPooledParticleFX(GameObject prefab, int index, bool shouldAutoDestruct)
	{
		string str = string.Format("Pool {2}[{0}] ({1}) ", index, prefab.name, base.name);
		ParticleSystem particleSystem = prefab.GetComponent<ParticleSystem>();
		bool flag = false;
		if (particleSystem == null)
		{
			particleSystem = prefab.GetComponentInChildren<ParticleSystem>();
			if (particleSystem == null)
			{
				return null;
			}
			flag = true;
			str += "child particle system ";
		}
		CFX_AutoDestructShuriken component = particleSystem.gameObject.GetComponent<CFX_AutoDestructShuriken>();
		if (shouldAutoDestruct)
		{
			if (component == null)
			{
				return string.Format(str + "does not have a CFX_AutoDestructShuriken script.", Array.Empty<object>());
			}
			if (!component.RecycleOnCompletion)
			{
				return string.Format(str + "is not set to be recycled.", Array.Empty<object>());
			}
			if (flag && !component.RecycleParent)
			{
				return string.Format(str + "is not set to have its parent recycled.", Array.Empty<object>());
			}
		}
		else if (component != null)
		{
			return string.Format(str + "is not supposed to have a CFX_AutoDestructShuriken script.", Array.Empty<object>());
		}
		return null;
	}

	// Token: 0x04000189 RID: 393
	public ObjectPoolConfig.StartupPool[] startupPools;

	// Token: 0x0400018A RID: 394
	public ObjectPoolConfig.StartupPoolMode startupPoolMode;

	// Token: 0x0400018B RID: 395
	public bool loggingEnabled;

	// Token: 0x0200004F RID: 79
	public enum StartupPoolMode
	{
		// Token: 0x0400018D RID: 397
		Awake,
		// Token: 0x0400018E RID: 398
		Start,
		// Token: 0x0400018F RID: 399
		CallManually
	}

	// Token: 0x02000050 RID: 80
	[Serializable]
	public class StartupPool
	{
		// Token: 0x04000190 RID: 400
		public int size;

		// Token: 0x04000191 RID: 401
		public GameObject prefab;

		// Token: 0x04000192 RID: 402
		public int maxSize;

		// Token: 0x04000193 RID: 403
		public bool doesNotSelfDestruct;
	}
}

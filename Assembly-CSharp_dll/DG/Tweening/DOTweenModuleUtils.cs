using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Scripting;

namespace DG.Tweening
{
	// Token: 0x02000C28 RID: 3112
	public static class DOTweenModuleUtils
	{
		// Token: 0x060057F2 RID: 22514 RVA: 0x00109648 File Offset: 0x00107848
		[Preserve]
		public static void Init()
		{
			if (DOTweenModuleUtils._initialized)
			{
				return;
			}
			DOTweenModuleUtils._initialized = true;
			DOTweenExternalCommand.SetOrientationOnPath += DOTweenModuleUtils.Physics.SetOrientationOnPath;
		}

		// Token: 0x060057F3 RID: 22515 RVA: 0x00109669 File Offset: 0x00107869
		[Preserve]
		private static void Preserver()
		{
			AppDomain.CurrentDomain.GetAssemblies();
			typeof(MonoBehaviour).GetMethod("Stub");
		}

		// Token: 0x0400437B RID: 17275
		private static bool _initialized;

		// Token: 0x02000C29 RID: 3113
		public static class Physics
		{
			// Token: 0x060057F4 RID: 22516 RVA: 0x0010968B File Offset: 0x0010788B
			public static void SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
			{
				if (options.isRigidbody)
				{
					((Rigidbody)t.target).rotation = newRot;
					return;
				}
				trans.rotation = newRot;
			}

			// Token: 0x060057F5 RID: 22517 RVA: 0x001096AE File Offset: 0x001078AE
			public static bool HasRigidbody2D(Component target)
			{
				return target.GetComponent<Rigidbody2D>() != null;
			}

			// Token: 0x060057F6 RID: 22518 RVA: 0x001096BC File Offset: 0x001078BC
			[Preserve]
			public static bool HasRigidbody(Component target)
			{
				return target.GetComponent<Rigidbody>() != null;
			}

			// Token: 0x060057F7 RID: 22519 RVA: 0x001096CC File Offset: 0x001078CC
			[Preserve]
			public static TweenerCore<Vector3, Path, PathOptions> CreateDOTweenPathTween(MonoBehaviour target, bool tweenRigidbody, bool isLocal, Path path, float duration, PathMode pathMode)
			{
				Rigidbody rigidbody = tweenRigidbody ? target.GetComponent<Rigidbody>() : null;
				TweenerCore<Vector3, Path, PathOptions> result;
				if (tweenRigidbody && rigidbody != null)
				{
					result = (isLocal ? rigidbody.DOLocalPath(path, duration, pathMode) : rigidbody.DOPath(path, duration, pathMode));
				}
				else
				{
					result = (isLocal ? target.transform.DOLocalPath(path, duration, pathMode) : target.transform.DOPath(path, duration, pathMode));
				}
				return result;
			}
		}
	}
}

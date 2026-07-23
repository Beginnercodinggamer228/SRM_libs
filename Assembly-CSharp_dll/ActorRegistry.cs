using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class ActorRegistry : MonoBehaviour
{
	// Token: 0x06000483 RID: 1155 RVA: 0x0001D28C File Offset: 0x0001B48C
	public void Register(RegisteredActorBehaviour actor)
	{
		if (actor is RegistryFixedUpdateable)
		{
			this.Register(actor as RegistryFixedUpdateable);
		}
		if (actor is RegistryUpdateable)
		{
			this.Register(actor as RegistryUpdateable);
		}
		if (actor is RegistryLateUpdateable)
		{
			this.Register(actor as RegistryLateUpdateable);
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001D2CA File Offset: 0x0001B4CA
	public void Deregister(RegisteredActorBehaviour actor)
	{
		if (actor is RegistryFixedUpdateable)
		{
			this.Deregister(actor as RegistryFixedUpdateable);
		}
		if (actor is RegistryUpdateable)
		{
			this.Deregister(actor as RegistryUpdateable);
		}
		if (actor is RegistryLateUpdateable)
		{
			this.Deregister(actor as RegistryLateUpdateable);
		}
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001D308 File Offset: 0x0001B508
	private void Register(RegistryFixedUpdateable actor)
	{
		if (!this.fixedUpdateActors.Contains(actor))
		{
			this.fixedUpdateActors.Add(actor);
			this.fixedUpdateActorsList.Add(actor);
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001D331 File Offset: 0x0001B531
	private void Deregister(RegistryFixedUpdateable actor)
	{
		if (this.fixedUpdateActors.Contains(actor))
		{
			this.fixedUpdateActors.Remove(actor);
			this.fixedUpdateActorsList.Remove(actor);
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001D35A File Offset: 0x0001B55A
	private void Register(RegistryUpdateable actor)
	{
		if (!this.updateActors.Contains(actor))
		{
			this.updateActors.Add(actor);
			this.updateActorsList.Add(actor);
		}
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001D383 File Offset: 0x0001B583
	private void Deregister(RegistryUpdateable actor)
	{
		if (this.updateActors.Contains(actor))
		{
			this.updateActors.Remove(actor);
			this.updateActorsList.Remove(actor);
		}
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0001D3AC File Offset: 0x0001B5AC
	private void Register(RegistryLateUpdateable actor)
	{
		if (!this.lateUpdateActors.Contains(actor))
		{
			this.lateUpdateActors.Add(actor);
			this.lateUpdateActorsList.Add(actor);
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001D3D5 File Offset: 0x0001B5D5
	private void Deregister(RegistryLateUpdateable actor)
	{
		if (this.lateUpdateActors.Contains(actor))
		{
			this.lateUpdateActors.Remove(actor);
			this.lateUpdateActorsList.Remove(actor);
		}
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001D400 File Offset: 0x0001B600
	public void FixedUpdate()
	{
		if (this.fixedUpdateActorsList.Data.Length > this.FixedUpdate_localActorsData.Length)
		{
			Array.Resize<RegistryFixedUpdateable>(ref this.FixedUpdate_localActorsData, Math.Max(this.fixedUpdateActorsList.Data.Length, 50));
		}
		int count = this.fixedUpdateActorsList.GetCount();
		this.fixedUpdateActorsList.Data.CopyTo(this.FixedUpdate_localActorsData, 0);
		for (int i = 0; i < count; i++)
		{
			RegistryFixedUpdateable registryFixedUpdateable = this.FixedUpdate_localActorsData[i];
			try
			{
				registryFixedUpdateable.RegistryFixedUpdate();
			}
			catch (Exception ex)
			{
				string message = "Failed to execute ActorRegistry.FixedUpdate. (see exception below)";
				object[] args;
				if (registryFixedUpdateable != null)
				{
					object[] array = new object[6];
					array[0] = "actor.id";
					array[1] = ((MonoBehaviour)registryFixedUpdateable).GetInstanceID();
					array[2] = "actor.name";
					array[3] = ((MonoBehaviour)registryFixedUpdateable).name;
					array[4] = "actor.type";
					args = array;
					array[5] = registryFixedUpdateable.GetType();
				}
				else
				{
					args = new object[0];
				}
				Log.Error(message, args);
				Log.Error(ex.ToString(), Array.Empty<object>());
			}
		}
		Array.Clear(this.FixedUpdate_localActorsData, 0, this.FixedUpdate_localActorsData.Length);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001D520 File Offset: 0x0001B720
	public void Update()
	{
		if (this.updateActorsList.Data.Length > this.Update_localActorsData.Length)
		{
			Array.Resize<RegistryUpdateable>(ref this.Update_localActorsData, Math.Max(this.updateActorsList.Data.Length, 50));
		}
		int count = this.updateActorsList.GetCount();
		this.updateActorsList.Data.CopyTo(this.Update_localActorsData, 0);
		for (int i = 0; i < count; i++)
		{
			RegistryUpdateable registryUpdateable = this.Update_localActorsData[i];
			try
			{
				registryUpdateable.RegistryUpdate();
			}
			catch (NullReferenceException ex)
			{
				if (registryUpdateable != null)
				{
					Log.Error("Null reference exception caught in ActorRegistry.Update", new object[]
					{
						"name",
						((MonoBehaviour)registryUpdateable).name,
						"type",
						registryUpdateable.GetType(),
						"stacktrace",
						ex.StackTrace
					});
				}
			}
			catch (Exception ex2)
			{
				Log.Error("Unhandled exception caught in ActorRegistry.Update", new object[]
				{
					"name",
					((MonoBehaviour)registryUpdateable).name,
					"type",
					registryUpdateable.GetType(),
					"ex",
					ex2.Message
				});
			}
		}
		Array.Clear(this.Update_localActorsData, 0, this.Update_localActorsData.Length);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0001D678 File Offset: 0x0001B878
	public void LateUpdate()
	{
		if (this.lateUpdateActorsList.Data.Length > this.LateUpdate_localActorsData.Length)
		{
			Array.Resize<RegistryLateUpdateable>(ref this.LateUpdate_localActorsData, Math.Max(this.lateUpdateActorsList.Data.Length, 50));
		}
		int count = this.lateUpdateActorsList.GetCount();
		this.lateUpdateActorsList.Data.CopyTo(this.LateUpdate_localActorsData, 0);
		for (int i = 0; i < count; i++)
		{
			RegistryLateUpdateable registryLateUpdateable = this.LateUpdate_localActorsData[i];
			try
			{
				registryLateUpdateable.RegistryLateUpdate();
			}
			catch (NullReferenceException ex)
			{
				if (registryLateUpdateable != null)
				{
					Log.Error("Null reference exception caught in ActorRegistry.Update", new object[]
					{
						"name",
						((MonoBehaviour)registryLateUpdateable).name,
						"type",
						registryLateUpdateable.GetType(),
						"stacktrace",
						ex.StackTrace
					});
				}
			}
			catch (Exception ex2)
			{
				Log.Error("Unhandled exception caught in ActorRegistry.Update", new object[]
				{
					"name",
					((MonoBehaviour)registryLateUpdateable).name,
					"type",
					registryLateUpdateable.GetType(),
					"ex",
					ex2.Message
				});
			}
		}
		Array.Clear(this.LateUpdate_localActorsData, 0, this.LateUpdate_localActorsData.Length);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0001D7D0 File Offset: 0x0001B9D0
	private void CheckedActorBehaviour(Action actorUpdate, GameObject gObj, string lifeCycleName, object actor)
	{
		bool flag = this.CheckNanPosition(gObj, "Before", lifeCycleName, actor);
		actorUpdate();
		if (!flag)
		{
			this.CheckNanPosition(gObj, "After", lifeCycleName, actor);
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001D7FC File Offset: 0x0001B9FC
	private bool CheckNanPosition(GameObject obj, string stage, string lifecycle, object behaviour)
	{
		Vector3 position = obj.transform.position;
		if (this.IsNanPosition(position))
		{
			Log.Error("Invalid Position Found", new object[]
			{
				"stage",
				stage,
				"lifecycle",
				lifecycle,
				"behaviour",
				behaviour.GetType(),
				"actor",
				obj.name
			});
			return true;
		}
		return false;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001D86C File Offset: 0x0001BA6C
	private bool IsNanPosition(Vector3 pos)
	{
		return float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z);
	}

	// Token: 0x040004A9 RID: 1193
	private HashSet<RegistryFixedUpdateable> fixedUpdateActors = new HashSet<RegistryFixedUpdateable>();

	// Token: 0x040004AA RID: 1194
	private ExposedArrayList<RegistryFixedUpdateable> fixedUpdateActorsList = new ExposedArrayList<RegistryFixedUpdateable>();

	// Token: 0x040004AB RID: 1195
	private HashSet<RegistryUpdateable> updateActors = new HashSet<RegistryUpdateable>();

	// Token: 0x040004AC RID: 1196
	private ExposedArrayList<RegistryUpdateable> updateActorsList = new ExposedArrayList<RegistryUpdateable>();

	// Token: 0x040004AD RID: 1197
	private HashSet<RegistryLateUpdateable> lateUpdateActors = new HashSet<RegistryLateUpdateable>();

	// Token: 0x040004AE RID: 1198
	private ExposedArrayList<RegistryLateUpdateable> lateUpdateActorsList = new ExposedArrayList<RegistryLateUpdateable>();

	// Token: 0x040004AF RID: 1199
	private const int MIN_LOCAL_ARRAY_RESIZE_AMOUNT = 50;

	// Token: 0x040004B0 RID: 1200
	private RegistryFixedUpdateable[] FixedUpdate_localActorsData = new RegistryFixedUpdateable[50];

	// Token: 0x040004B1 RID: 1201
	private RegistryUpdateable[] Update_localActorsData = new RegistryUpdateable[50];

	// Token: 0x040004B2 RID: 1202
	private RegistryLateUpdateable[] LateUpdate_localActorsData = new RegistryLateUpdateable[50];
}

using System.Collections.Generic;

namespace CX.Obj
{
	public class ObjectPool<T> : IObjectPool<T>, IPoolControlable
	{
		private Queue<T> queues;
		private int poolCount;
		private IPooledObject<T> pooledObj;

		public ObjectPool(IPooledObject<T> pooledObj, int poolCount) 
		{
			queues = new Queue<T>();
			this.pooledObj = pooledObj;
			this.poolCount = poolCount;

			InitPool();
		}

		protected virtual void InitPool()
		{
//			for (int i = 0; i < poolCount; i++)
//			{
//				CreatePoolObj();
//			}
			Loop.UpdateEvent += HandleUpdateEvent;
		}

		void HandleUpdateEvent ()
		{
			CreatePoolObj();
			if (queues.Count >= poolCount)
				Loop.UpdateEvent -= HandleUpdateEvent;
		}

		protected virtual void CreatePoolObj()
		{
			var obj = pooledObj.Make ();
			pooledObj.InAactivate(obj);
			queues.Enqueue (obj);
		}

		public virtual T GetOne()
		{
			T obj = default(T);
			if (queues.Count > 0)
			{
				obj = queues.Dequeue ();
				pooledObj.Activate(obj);
			}
			else
			{
				CreatePoolObj();
				obj = GetOne();
			}
			return obj;
		}

		public virtual void PutBack(T obj)
		{
			pooledObj.InAactivate(obj);
			queues.Enqueue(obj);
		}

		public void Clear()
		{
			foreach (var obj in queues)
			{
				pooledObj.Destroy(obj);
			}
			queues.Clear();
		}
	}
}

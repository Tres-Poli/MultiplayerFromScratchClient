using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataStructures
{
	public abstract class SerializableDictionary
	{
		public bool IsSingleLineView = true;
		
		public abstract Dictionary<int, int> GetKeysFirstIndexes();
	}
	
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : SerializableDictionary, ISerializationCallbackReceiver, IDictionary<TKey, TValue>
	{
		[Serializable]
		public class SerializableKeyValuePair
		{
			public TKey Key;
			public TValue Value;
			
			public SerializableKeyValuePair(TKey key, TValue value)
			{
				Key = key;
				Value = value;
			}
			
			public void SetValue(TValue value) =>
					Value = value;
		}
		
		private readonly Dictionary<TKey, uint> keyPositions;
		
		[SerializeField] private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();
		
		public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
		public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();
		public TValue this[TKey key]
		{
			get => list[(int)KeyPositions[key]].Value;
			set
			{
				if (KeyPositions.TryGetValue(key, out uint index))
				{
					list[(int)index].SetValue(value);
				}
				else
				{
					KeyPositions[key] = (uint)list.Count;
					list.Add(new SerializableKeyValuePair(key, value));
				}
			}
		}
		public int Count => list.Count;
		public bool IsReadOnly => false;
		
		private Dictionary<TKey, uint> KeyPositions => keyPositions;

		public SerializableDictionary()
		{
			keyPositions = MakeKeyPositions();
			#if !UNITY_2020_2_OR_NEWER
			Debug.LogWarning($"<b>{nameof(SerializableDictionary)}:</b> unavailable in Unity version lower 2020.2.");
			#endif
		}

		private Dictionary<TKey, uint> MakeKeyPositions()
		{
			int numEntries = list.Count;
			var result = new Dictionary<TKey, uint>(numEntries);
			for (int i = 0; i < numEntries; i++)
			{
				var key = list[i].Key;
				if (key == null)
					continue;

				result[key] = (uint)i;
			}

			return result;
		}

		public void OnBeforeSerialize() {}
		
		public void OnAfterDeserialize()
		{
			Dictionary<TKey, uint> positions = MakeKeyPositions();
			foreach (var position in positions)
			{
				keyPositions.TryAdd(position.Key, position.Value);
			}
		}

		public override Dictionary<int, int> GetKeysFirstIndexes()
		{
			var res = new Dictionary<int, int>();
			var keyFirst = new List<KeyValuePair<TKey, int>>();
			for (var index = 0; index < list.Count; index++)
			{
				TKey key = list[index].Key;
				int first = index;
				var found = false;

				var collection = keyFirst.Where(pair => pair.Key == null || pair.Key.Equals(key));
				foreach (var pair in collection)
				{
					first = pair.Value;
					found = true;
					break;
				}

				if (!found)
				{
					keyFirst.Add(new KeyValuePair<TKey, int>(key, index));
				}

				res.Add(index, first);
			}

			return res;
		}

		public void Add(TKey key, TValue value)
		{
			if (KeyPositions.ContainsKey(key))
			{
				Debug.LogError("ArgumentException: An element with the same key already exists in the dictionary.");
				return;
			}

			KeyPositions[key] = (uint)list.Count;
			list.Add(new SerializableKeyValuePair(key, value));
		}
		
		public bool ContainsKey(TKey key) =>
				KeyPositions.ContainsKey(key);
		
		public bool Remove(TKey key)
		{
			if (!KeyPositions.TryGetValue(key, out uint index))
			{
				return false;
			}

			var kp = KeyPositions;
			kp.Remove(key);
			var numEntries = list.Count;
			for (uint i = index; i < numEntries; i++)
			{
				kp[list[(int)i].Key] = i;
			}
			
			list.RemoveAt((int)index);
			OnAfterDeserialize();
			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			value = default;
			if (!KeyPositions.TryGetValue(key, out uint index))
			{
				return false;
			}

			value = list[(int)index].Value;
			return true;
		}
		
		public void Add(KeyValuePair<TKey, TValue> kvp) =>
				Add(kvp.Key, kvp.Value);

		public void Clear()
		{
			list.Clear();
			keyPositions.Clear();
		}
		
		public bool Contains(KeyValuePair<TKey, TValue> kvp) =>
				KeyPositions.ContainsKey(kvp.Key);
		
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			var numKeys = list.Count;
			if (array.Length - arrayIndex < numKeys)
			{
				Debug.LogError("ArgumentException: arrayIndex");
				return;
			}
			
			for (int i = 0; i < numKeys; i++, arrayIndex++)
			{
				var entry = list[i];
				array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
			}
		}
		
		public bool Remove(KeyValuePair<TKey, TValue> kvp) =>
				Remove(kvp.Key);
		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return list.Select(ToKeyValuePair).GetEnumerator();
			KeyValuePair<TKey, TValue> ToKeyValuePair(SerializableKeyValuePair skvp)
			{
				return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();
	}
}
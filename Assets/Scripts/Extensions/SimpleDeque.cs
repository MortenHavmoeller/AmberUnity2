using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class SimpleDeque<T>
{
	private const int LIST_ALLOCATION_SIZE = 5;

	private LinkedList<List<T>> _lists;
	private int _count;
	public int Count { get { return _count; } }

	private int[] _listCounts;

	public SimpleDeque(int defaultCapacity)
	{
		_lists = new LinkedList<List<T>>();
		_lists.AddLast(new List<T>(defaultCapacity));
		_count = 0;
		InitializeListCounts();
	}

	public void Clear()
	{
		int defaultCapacity = _lists.Last.Value.Capacity;
		var firstList = GetFirstList();
		firstList.Clear();
		_lists.Clear();
		_lists.AddLast(firstList);
		_count = 0;
		InitializeListCounts();
	}

	private void InitializeListCounts()
	{
		_listCounts = new int[LIST_ALLOCATION_SIZE];
		for (int i = 0; i < _listCounts.Length; i++)
		{
			_listCounts[i] = 0;
		}
	}

	private void FitListCounts(int listAmount)
	{
		if (listAmount < _listCounts.Length)
			return;

		int[] result = new int[LIST_ALLOCATION_SIZE * (1 + (listAmount - 1)/LIST_ALLOCATION_SIZE)];
		for (int i = 0; i < _listCounts.Length; i++)
		{
			if (i >= result.Length)
				break;

			result[i] = _listCounts[i];
		}
		_listCounts = result;
	}

	private void IncrementLastListCount()
	{
		_listCounts[_lists.Count - 1]++;
	}

	public void AddLast(T item)
	{
		if (_lists.Last.Value.Capacity <= _lists.Last.Value.Count)
		{
			_lists.AddLast(new List<T>(_lists.Last.Value.Capacity));
			FitListCounts(_lists.Count);
		}

		_lists.Last.Value.Add(item);
		_count++;
		IncrementLastListCount();
	}

	public T ElementAt(int index)
	{
		LinkedListNode<List<T>> current = _lists.First;
		int advancement = _listCounts[0];

		for (int i = 0;  i < _listCounts.Length; i++)
		{
			advancement += _listCounts[i];
			if (advancement > index)
			{
				advancement -= _listCounts[i];
				break;
			}
			current = current.Next;
		}
		
		return current.Value[index - advancement];
	}

	public T PopAt(int index)
	{
		LinkedListNode<List<T>> current = _lists.First;
		int advancement = _listCounts[0];

		for (int i = 0; i < _listCounts.Length; i++)
		{
			advancement += _listCounts[i];
			if (advancement > index)
			{
				advancement -= _listCounts[i];
				break;
			}
			current = current.Next;
		}

		T result = current.Value[index - advancement];
		current.Value.RemoveAt(index - advancement);
		return result;
	}

	private List<T> GetFirstList()
	{
		return _lists.First.Value;
	}
}

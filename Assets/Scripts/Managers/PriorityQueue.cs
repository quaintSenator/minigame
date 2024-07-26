using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class CleverTimeDieEventData : EventData
{
    private GameObject go_shall_die;
    public CleverTimeDieEventData(GameObject go)
    {
        go_shall_die = go;
    }
}
public class PElement
{
    public Action<EventData> callback;
}
public class PriorityElement
{
    public double priority;
    public PElement stuff;

    public PriorityElement(double p, PElement s)
    {
        priority = p;
        stuff = s;
    }

    public bool Beats(PriorityElement p)
    {
        return priority < p.priority;
    }
}

public class PriorityQueue
{
    private readonly List<PriorityElement> _list;
    private int end;

    public PriorityQueue()
    {
        end = 1; //Ideosyncracy of heaps

        //Create a new list to hold the elements. 
        _list = new List<PriorityElement>();

        //Fill up the 0th element (a bit hacky, but makes the math later on easier)
        _list.Insert(0, null);
    }

    public void Enqueue(PriorityElement p)
    {
        //Insert into the list
        _list.Insert(end, p);

        //'Bubble' up to the correct position
        BubbleUp(end);

        //Increase element count
        end++;
    }
    public PriorityElement Dequeue()
    {
        if (IsEmpty()) return null;

        //Store the top element in a temporary location
        var result = _list[1];

        //Swap the top and bottom elements of the list
        Swap(end - 1, 1);

        //Set the bottom element to null (which is now the top element)
        //-This is fine as we have saved it in a temporary location
        _list[end - 1] = null;
        end--;
        BubbleDown();
        return result;
    }

    public bool IsEmpty()
    {
        return end == 1;
    }

    private void BubbleUp(int index)
    {
        //Top level item can't be bubbled up
        if (index == 1) return;

        //Init
        var currentIndex = index;
        var parentIndex = Parent(index);

        //Try and move the element as high as possible, until something above it has a higher priority than it
        while (_list[currentIndex].Beats(_list[parentIndex]))
        {
            Swap(currentIndex, parentIndex);
            currentIndex = parentIndex;
            if (currentIndex == 1) return;
            parentIndex = Parent(currentIndex);
        }
    }

    private void BubbleDown()
    {
        //If empty or containing one element, no need to bubble!
        if (end == 2 || end == 1) return;
        var currentIndex = 1;
        var leftIndex = Left(currentIndex);
        var rightIndex = Right(currentIndex);
        //Keep look to see if the left or right node is higher priority- if so, swap them and move progressively down
        while (true)
            //If the node has bot ha right and left element, check them
            if (leftIndex < end && rightIndex < end)
            {
                if (_list[leftIndex].Beats(_list[currentIndex]) || _list[rightIndex].Beats(_list[currentIndex]))
                {
                    var replaceIndex = rightIndex;
                    if (_list[leftIndex].Beats(_list[rightIndex])) replaceIndex = leftIndex;

                    Swap(currentIndex, replaceIndex);

                    currentIndex = replaceIndex;
                    leftIndex = Left(currentIndex);
                    rightIndex = Right(currentIndex);
                }
                else
                {
                    return;
                }
            }
            //If no right element exits, just need to check the left
            else if (leftIndex < end)
            {
                if (_list[leftIndex].Beats(_list[currentIndex]))
                {
                    Swap(leftIndex, currentIndex);
                    return;
                }
                return;
            }
            else
            {
                return;
            }
    }
    //Used for testing purposes
    private bool verifyHeap()
    {
        if (end == 1 || end == 2) return true;
        for (var x = end - 1; x >= 2; x--)
            if (_list[x].Beats(_list[Parent(x)]))
                return false;
        return true;
    }
    private int Parent(int index)
    {
        return index % 2 == 1 ? (index - 1) / 2 : index / 2;
    }

    private int Left(int index)
    {
        return index * 2;
    }

    private int Right(int index)
    {
        return index * 2 + 1;
    }

    private void Swap(int index0, int index1)
    {
        var temp = _list[index1];
        _list[index1] = _list[index0];
        _list[index0] = temp;
    }

    public PriorityElement Peek()
    {
        if (!IsEmpty())
        {
            return _list[1];
        }
        return null;
    }
    #if UNITY_EDITOR
    public void print()
    {
        var sb = new StringBuilder();
        for (int i = 1; i < _list.Count; i++)
        {
            sb.Append(" ");
            sb.Append(_list[i].priority.ToString());
        }
        Debug.Log(sb.ToString());
    }
    #endif
    
}
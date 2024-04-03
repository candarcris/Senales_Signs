using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LRUcache<Tkey, TValue>
{
    private int _maxCapacity;
    private LinkedList<(Tkey key, TValue value)> _lruList;
    private Dictionary<Tkey, LinkedListNode<(Tkey key, TValue value)>> _cache;
    public LRUcache(int capacity)
    {
        _maxCapacity = capacity;
        _lruList = new();
        _cache = new();
    }

    public bool TryGetValue(Tkey key, out TValue value)
    {
        if(_cache.TryGetValue(key, out var node))
        {  
            value = node.Value.value;
            _lruList.Remove(node);
            _lruList.AddLast(node);
            return true; 
        }
        else
        {
            value = default(TValue);
            return false;
        }
    }

    public void SetValue(Tkey key, TValue value)
    {
        // Verifica si la clave ya existe en el caché
        if (_cache.TryGetValue(key, out var node))
        {
            // Si la clave existe, actualiza su valor y su posición en la lista enlazada
            _lruList.Remove(node);
            _lruList.AddLast((key, value));
            _cache[key] = _lruList.Last;
        }
        else
        {
            // Si la clave no existe
            if (_cache.Count >= _maxCapacity)
            {
                // Si el caché está lleno, elimina el elemento menos recientemente usado
                var leastUsed = _lruList.First;
                _cache.Remove(leastUsed.Value.key);
                _lruList.RemoveFirst();
            }

            // Añade el nuevo elemento al final de la lista enlazada y lo agrega al diccionario
            var newNode = new LinkedListNode<(Tkey key, TValue value)>((key, value));
            _lruList.AddLast(newNode);
            _cache[key] = newNode;
        }
    }
}

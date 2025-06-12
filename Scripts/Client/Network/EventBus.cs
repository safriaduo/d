using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dawnshard.Network
{
    public class EventBus<T1,T2>
    {
        private Dictionary<T1, Action<T2>> _eventDictionary = new();

        public void Subscribe(T1 index, Action<T2> listener)
        {
            if (!_eventDictionary.ContainsKey(index))
            {
                _eventDictionary[index] = listener;
            }
            else
            {
                _eventDictionary[index] += listener;
            }
        }

        public void Unsubscribe(T1 index, Action<T2> listener)
        {
            if (_eventDictionary.ContainsKey(index))
            {
                _eventDictionary[index] -= listener;
                if (_eventDictionary[index] == null)
                {
                    _eventDictionary.Remove(index);
                }
            }
        }

        public void UnsubscribeToAll()
        {
            foreach (var eventDictionaryEntry in _eventDictionary.ToList())
            {
                Debug.Log("Unsubscribing "+eventDictionaryEntry.Key+" from bus");
                Unsubscribe(eventDictionaryEntry.Key, eventDictionaryEntry.Value);
            }
        }

        public void Publish(T1 index, T2 eventData)
        {
            if (_eventDictionary.ContainsKey(index))
            {
                _eventDictionary[index]?.Invoke(eventData);
            }
            else
            {
                Debug.LogWarning($"No listeners for Card ID: {index}");
            }
        }
    }
}
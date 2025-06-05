using System;
using UnityEngine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static class EventBus<T> where T : IEvent
    {
        public static event Action<T> OnEvent;

        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }


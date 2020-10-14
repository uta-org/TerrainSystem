using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains
{
    public class ThreadedDataRequester : MonoBehaviour
    {
        private static ThreadedDataRequester _instance;
        private readonly Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

        private void Awake()
        {
            _instance = FindObjectOfType<ThreadedDataRequester>();
        }

        public static void RequestData(Func<object> generateData, Action<object> callback)
        {
            ThreadStart threadStart = delegate { _instance.DataThread(generateData, callback); };

            new Thread(threadStart).Start();
        }

        private void DataThread(Func<object> generateData, Action<object> callback)
        {
            var data = generateData();
            lock (_dataQueue)
            {
                _dataQueue.Enqueue(new ThreadInfo(callback, data));
            }
        }

        private void Update()
        {
            if (_dataQueue.Count > 0)
                for (var i = 0; i < _dataQueue.Count; i++)
                {
                    var threadInfo = _dataQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
        }

        private struct ThreadInfo
        {
            public readonly Action<object> Callback;
            public readonly object Parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                this.Callback = callback;
                this.Parameter = parameter;
            }
        }
    }
}
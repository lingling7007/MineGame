
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimpleFramework
{
    public class EnumeratorTaskManager : MonoSingleton<EnumeratorTaskManager>
    {


        private Dictionary<int, EnumeratorTaskData> mapTasks = new Dictionary<int, EnumeratorTaskData>();
        private List<EnumeratorTaskData> tempCaches = new List<EnumeratorTaskData>();

        public void InitialTask(int type, int maxCount = 6)
        {
            if (mapTasks.ContainsKey(type))
            {
                LogCtrl.Error("Call GetFreeType Function  GetType. Error : " + type);
                return;
            }
            mapTasks.Add(type, CreateTaskData(type, maxCount));

        }

        public void AddTask(int type, IEnumeratorTask task)
        {
            if (!mapTasks.ContainsKey(type))
            {
                InitialTask(type);
            }
            mapTasks[type].Addition(task);
        }

        public void ExecuteTask(IEnumeratorTask task, System.Action finishCallback)
        {
            StartCoroutine(task.DoLoadAsync(finishCallback));
        }

        public int GetFreeType()
        {
            return GetFreeType(mapTasks.Count);
        }

        private int GetFreeType(int type)
        {
            if (mapTasks.ContainsKey(type))
            {
                return GetFreeType(type + 1);
            }
            return type;
        }

        /// <summary>
        /// 缓存起来的对象 性能有影响吗 待测。
        /// </summary>
        public EnumeratorTaskData CreateTaskData(int type, int maxCount = 6)
        {
            EnumeratorTaskData data = null;
            if (tempCaches.Count > 0)
            {
                data = tempCaches[0];
                tempCaches.RemoveAt(0);
            }
            else
            {
                data = new EnumeratorTaskData();
            }
            data.Initial(type, maxCount);
            return data;
        }


    }

    public class EnumeratorTaskData
    {
        private List<IEnumeratorTask> mlistTask;
        public int TaskType { get; private set; }
        public int MaxCount { get; private set; }
        public int CurrentCount { get; private set; }

        public void Initial(int type, int maxCount)
        {
            if (mlistTask != null)
            {
                mlistTask.Clear();
            }
            else
            {
                mlistTask = new List<IEnumeratorTask>();
            }
            TaskType = type;
            CurrentCount = 0;
            MaxCount = maxCount;
        }

        public void Addition(IEnumeratorTask task)
        {
            if (mlistTask.Contains(task))
            {
                LogCtrl.Warn("AddTask  Contains   Type : " + TaskType);
                return;
            }
            mlistTask.Add(task);
            TryExcuteNextTask();
        }

        private void TryExcuteNextTask()
        {
            if (mlistTask.Count > 0 && CurrentCount < MaxCount)
            {
                CurrentCount++;
                IEnumeratorTask task = mlistTask[0];
                mlistTask.RemoveAt(0);
                EnumeratorTaskManager.Instance.ExecuteTask(task, ExecuteFinish);
            }

        }
        private void ExecuteFinish()
        {
            CurrentCount--;
            TryExcuteNextTask();
        }
        public void Release()
        {
            if (mlistTask != null)
            {
                mlistTask.Clear();
            }
        }
    }
}

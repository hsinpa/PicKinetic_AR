using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using PicKinetic.Model;

namespace PicKinetic
{
    public class PicKineticAR : Singleton<PicKineticAR>
    {
        [SerializeField]
        private IModelManager _ModelManager;
        public IModelManager ModelManager => _ModelManager;

        protected PicKineticAR() { } // guarantee this will be always a singleton only - can't use the constructor!

        private Subject subject;

        private Observer[] observers = new Observer[0];

        private void Awake()
        {
            subject = new Subject();
            _ModelManager.SetUp();

            RegisterAllController(subject);
        }

        private void Start()
        {
            Init();
        }

        public void Notify(string entity, params object[] objects)
        {
            subject.notify(entity, objects);
        }

        public void Init()
        {
            Notify(EventFlag.Event.GameStart);
        }

        private void RegisterAllController(Subject p_subject)
        {
            Transform ctrlHolder = transform.Find("Controller");

            if (ctrlHolder == null) return;

            observers = transform.GetComponentsInChildren<Observer>();

            foreach (Observer observer in observers)
            {
                subject.addObserver(observer);
            }
        }

        public T GetObserver<T>() where T : Observer
        {
            foreach (Observer observer in observers)
            {
                if (observer.GetType() == typeof(T)) return (T)observer;
            }

            return default(T);
        }

    }
}
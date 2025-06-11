using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FoodyGo.Utils.DI
{
    public abstract class Scope : MonoBehaviour
    {
        protected Container container;
        
        [SerializeField] List<MonoBehaviour> _monoBehaviours;

        protected virtual void Awake()
        {
            container = new Container();
            Register();
            InjectAll();
        }

        public virtual void Register()
        {
            foreach (MonoBehaviour monoBehaviour in _monoBehaviours)
            {
                container.RegisterMonoBehaviour(monoBehaviour);
            }
        }

        protected virtual void InjectAll()
        {
            MonoBehaviour[] monoBehaviours = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (MonoBehaviour monoBehaviour in monoBehaviours)
            {
                Inject(monoBehaviour);
            }
        }

        /// <summary>
        /// 의존성을 주입함
        /// </summary>
        /// <param name="target">주입할 대상</param>
        protected virtual void Inject(object target)
        {
            Type type = target.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetCustomAttributes<InjectAttribute>() != null)
                {
                    object value = container.Resolve(fieldInfo.FieldType);
                    if (value != null)
                    {
                        fieldInfo.SetValue(target, value);
                    }
                }
            }
        }
    }
}
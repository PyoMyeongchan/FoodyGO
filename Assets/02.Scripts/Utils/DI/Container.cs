using System.Collections.Generic;
using System;
using UnityEngine;

namespace FoodyGo.Utils.DI
{
    public class Container
    {
        public Container()
        {
            _registrations = new Dictionary<Type, object>();
        }

        private Dictionary<Type, object> _registrations;

        /// <summary>
        /// 생성자가 있는 일반 C# 클래스 등록(생성해서 추가함)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Register<T>()
        where T : class, new()
        {
            T obj = new T();
            _registrations[typeof(T)] = obj;
        }
        
        /// <summary>
        /// MonoBehaviour 객체를 생성해서 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterMonoBehaviour<T>()
        where T : MonoBehaviour
        {
            T obj = new GameObject(typeof(T).Name).AddComponent<T>();
            _registrations[typeof(T)] = obj;
        }

        /// <summary>
        /// Hierarchy에 존재하는 객체를 추가
        /// </summary>
        /// <param name="monoBehaviour"></param>
        public void RegisterMonoBehaviour(MonoBehaviour monoBehaviour)
        {
            _registrations[monoBehaviour.GetType()] = monoBehaviour;
        }

        /// <summary>
        /// 등록된거 가져오는 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return (T)_registrations[typeof(T)];
        }

        public object Resolve(Type type)
        {
            if (_registrations.TryGetValue(type, out object obj))
            {
                return obj;
            }
            else
            {
                return null;
            }
        }
    }    
}



using System;
using System.Collections.Generic;
using FoodyGo.Singletons;

namespace FoodyGo.UI
{
    public class UIManager : Singleton_1<UIManager>
    {
        Dictionary<Type, UI_Base> _uis = new Dictionary<Type, UI_Base>();

        public void RegisterUI(UI_Base ui)
        {
            if (_uis.ContainsKey(ui.GetType()))
            {
                throw new Exception("UI already registered");
            }
            _uis.Add(ui.GetType(), ui);
            
        }

        public void UnRegisterUI(UI_Base ui)
        {
            _uis.Remove(ui.GetType());
        }


        public T Resolve<T>() where T : UI_Base
        {
            return (T)_uis[typeof(T)];
        }
    }    
}
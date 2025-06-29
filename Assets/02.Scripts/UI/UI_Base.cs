using System;
using UnityEngine;

namespace FoodyGo.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UI_Base : MonoBehaviour
    {
        public int sortingOrder
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }
        Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            UIManager.instance.RegisterUI(this);
        }

        private void OnDisable()
        {
            UIManager.instance.UnRegisterUI(this);
        }

        public virtual void Show()
        {
            _canvas.enabled = true;
        }

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }
    }
}

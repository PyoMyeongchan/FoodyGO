using FoodyGo.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodyGo.InputSystems
{
    public class ThrowTouchPad : MonoBehaviour
    {
        [Header("Throw Settings")] 
        [Tooltip("던질때 속도")] 
        [SerializeField] float _throwSpeed = 35f;

        [Tooltip("던질 공 프리팹")] 
        [SerializeField] MonsterBallController _throwObjectPrefab;
        MonsterBallController _throwObject;
        
        [Header("Input Actions")]
        [Tooltip("스크린 터치 좌표")]
        [SerializeField] InputActionReference _touchPosition; // Vector2
        [Tooltip("스크린 터치 여부")]
        [SerializeField] InputActionReference _touchPress; // Button
        
        [SerializeField]GameObject _target;
        bool _isDragging;
        bool _isThrowing;
        double _beginDragTimeMark; 
        Vector2 _cachedTouchPosition;
        Vector2 _cachedBeginDragPosition;

        private void OnEnable()
        {
            _touchPosition.action.performed += OnTouchPositionPerformed;
            _touchPosition.action.Enable();
            _touchPress.action.performed += OnTouchPressPerformed;
            _touchPress.action.Enable();
        }

        private void OnDisable()
        {
            _touchPosition.action.performed -= OnTouchPositionPerformed;
            _touchPosition.action.Disable();
            _touchPress.action.performed -= OnTouchPressPerformed;
            _touchPress.action.Disable();
        }

        private void Start()
        {
            ResetThrowObject();
        }

        void OnTouchPositionPerformed(InputAction.CallbackContext context)
        {
            if (_isThrowing)
            {
                return;
            }

            _cachedTouchPosition = context.ReadValue<Vector2>();
        }

        void OnTouchPressPerformed(InputAction.CallbackContext context)
        {
            if (_isThrowing)
            {
                return;
            }
            
            // 터치 눌림
            if (context.ReadValueAsButton())
            {
                if (_isDragging == false)
                {
                    _isDragging = true;
                    _cachedBeginDragPosition = _cachedTouchPosition;
                    _beginDragTimeMark = context.time;
                }
            }
            // 터치 뗌
            else
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    double elapsedDraggingTime = context.time - _beginDragTimeMark; // 드래그 총 시간
                    Vector2 dragDelta = _cachedTouchPosition - _cachedBeginDragPosition; // 드래그 거리
                    
                    // 속도 검사
                    float dragVelocityY = dragDelta.y / (float)elapsedDraggingTime;

                    if (dragVelocityY >= _throwSpeed)
                    {
                        _throwObject.Throw(_target,2.0f,1.0f);
                    }
                }
            }
        }

        void ResetThrowObject()
        {
            if (_throwObject == null)
            {
                _throwObject = Instantiate(_throwObjectPrefab, Camera.main.transform);
            }
            _throwObject.transform.localPosition = new Vector3(0, -1f, 2.5f);
        }
    }
}
using System;
using System.Collections;
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
        [SerializeField] GameObject _throwObjectPrefab;
        GameObject _throwObject;
        
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
                        StartCoroutine(C_Throw(2f, 1f));
                    }
                }
            }
        }

        /// <summary>
        /// 공을 던지는 애니메이션 구현
        /// </summary>
        /// <param name="arcHeight">포물선 높이</param>
        /// <param name="duration">던져진 동안의 시간</param>
        /// <returns></returns>
        IEnumerator C_Throw(float arcHeight, float duration)
        {
            _isThrowing = true;
            
            _throwObject.transform.SetParent(null);
            Vector3 throwStartPosition = _throwObject.transform.position;
            Vector3 throwEndPosition = _target.transform.position;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float ease = Mathf.Sin(t * Mathf.PI * 0.5f);
                Vector3 lerp = Vector3.Lerp(throwStartPosition, throwEndPosition, ease);

                float heihtOffset = arcHeight * MathF.Sin(Mathf.PI * ease);
                Vector3 targetPosition = new Vector3(lerp.x, lerp.y + heihtOffset, lerp.z );
                _throwObject.transform.position = targetPosition;
                yield return null;
            }

            _throwObject.transform.position = throwEndPosition;
            _isThrowing = false;
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
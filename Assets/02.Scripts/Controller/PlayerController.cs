using System;
using System.Collections;
using FoodyGo.Mapping;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodyGo.Controller
{
    public class PlayerController : MonoBehaviour
    {
#if UNITY_EDITOR
        // 현재 속도
        public Vector3 velocity;
        public Vector3 direction;
        public float speed = 5f;
        
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] GoogleMapTileManager _mapTileManager;

        IEnumerator Start()
        {
            yield return new WaitUntil(() => _mapTileManager.isInitialized);
            transform.position = _mapTileManager.GetCenterTileWorldPosition();
        }

        private void OnEnable()
        {
            _moveInputAction.action.performed += OnMovePerformed;
            _moveInputAction.action.canceled += OnMoveCanceled;
            _moveInputAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveInputAction.action.performed -= OnMovePerformed;
            _moveInputAction.action.canceled -= OnMoveCanceled;
            _moveInputAction.action.Disable();
        }

        private void FixedUpdate()
        {
            if (direction.sqrMagnitude > 0)
            {
                velocity = direction * speed;
                transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
            }
            else
            {
                velocity = Vector3.zero;
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 input2D = context.ReadValue<Vector2>();
            direction = new Vector3(input2D.x, 0, input2D.y);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            direction = Vector3.zero;
        }
#endif
    }    
    
}




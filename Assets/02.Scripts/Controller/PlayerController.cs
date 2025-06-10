using System;
using System.Collections;
using FoodyGo.Mapping;
using FoodyGo.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodyGo.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask _battleMask;
#if UNITY_EDITOR
        // 현재 속도
        public Vector3 velocity;
        public Vector3 direction;
        public float speed = 5f;
        
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] GoogleMapTileManager _mapTileManager;
        [SerializeField] CinemachineCamera _cam;

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
            Vector3 camForward = _cam.transform.forward;
            Vector3 camRight = _cam.transform.right;
            
            camForward.y = 0f;
            camRight.y = 0f;
            
            Vector3 movedirection = (camForward * direction.z + camRight * direction.x);
            
            if (movedirection.sqrMagnitude > 0)
            {
                velocity = movedirection.normalized * speed;
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
        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _battleMask.value) > 0)
            {
                UI_BattleConfirmWindow window = UIManager.instance.Resolve<UI_BattleConfirmWindow>();
                window.Show();
            }
        }
        
    }    
    
}




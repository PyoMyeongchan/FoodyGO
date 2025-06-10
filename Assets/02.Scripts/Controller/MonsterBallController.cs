using System;
using System.Collections;
using UnityEngine;

namespace FoodyGo.Controller
{
    public class MonsterBallController : MonoBehaviour
    {
        GameObject _target;
        bool _isThrowing;
        [SerializeField] float _radiuse = 0.7f;
        [SerializeField] float _bounceDamping = 0.6f;
        [SerializeField] LayerMask _targetMask;
        Vector3 _lastVelocity;

        public void Throw(GameObject target, float arcHeight, float duration)
        {
            if (_isThrowing)
            {
                return;
            }

            _target = target;
            StartCoroutine(C_Throw(arcHeight, duration));
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

            Vector3 throwStartPosition = transform.position;
            Vector3 throwEndPosition = _target.transform.position;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                Vector3 lastPosition = transform.position;
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float ease = Mathf.Sin(t * Mathf.PI * 0.5f);
                Vector3 lerp = Vector3.Lerp(throwStartPosition, throwEndPosition, ease);

                float heihtOffset = arcHeight * MathF.Sin(Mathf.PI * ease);
                Vector3 targetPosition = new Vector3(lerp.x, lerp.y + heihtOffset, lerp.z);
                transform.position = targetPosition;

                _lastVelocity = (transform.position - lastPosition) / Time.deltaTime;

                if (Physics.SphereCast(lastPosition, _radiuse, _lastVelocity, out RaycastHit hit,
                        Vector3.Distance(transform.position, lastPosition), _targetMask))
                {
                    _isThrowing = false;
                    StartCoroutine(C_Bounce(hit.normal));
                    yield break;
                }

                yield return null;
            }

            transform.position = throwEndPosition;
            _isThrowing = false;
        }

        IEnumerator C_Bounce(Vector3 normal)
        {
            Destroy(gameObject, 5.0f);
            _lastVelocity = Vector3.Reflect(_lastVelocity, normal) * _bounceDamping;

            while (true)
            {
                _lastVelocity += Physics.gravity * Time.deltaTime;
                transform.position += _lastVelocity * Time.deltaTime;
                yield return null;
            }
        }
    }
}

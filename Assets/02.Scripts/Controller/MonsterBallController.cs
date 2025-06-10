using System;
using System.Collections;
using UnityEngine;

namespace FoodyGo.Controller
{
    public class MonsterBallController : MonoBehaviour
    {
        GameObject _target;
        bool _isThrowing;
        Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

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
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float ease = Mathf.Sin(t * Mathf.PI * 0.5f);
                Vector3 lerp = Vector3.Lerp(throwStartPosition, throwEndPosition, ease);

                float heihtOffset = arcHeight * MathF.Sin(Mathf.PI * ease);
                Vector3 targetPosition = new Vector3(lerp.x, lerp.y + heihtOffset, lerp.z );
                transform.position = targetPosition;
                yield return null;
            }

            transform.position = throwEndPosition;
            _rigidbody.isKinematic = false;
            _isThrowing = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.Equals(_target))
            {
                //TODO : 이 몬스터 포획
                Debug.Log("몬스터와 충돌");
                StopAllCoroutines();
                _rigidbody.isKinematic = false;
            }
        }
    }
}

using System;
using FoodyGo.Mapping;
using UnityEngine;

namespace FoodyGo.Services.GPS
{
    /// <summary>
    /// Target의 transform을 기반으로 Location을 갱신해서 시뮬레이션용 데이터를 제공
    /// </summary>
    public class SimulatedLocationProvider : MonoBehaviour, ILocationProvider
    {
        public Transform target { get; set; }

        public MapLocation startLocation { get; set; }

        public double latitude { get; private set; }

        public double longitude { get; private set; }

        public double altitude { get; private set; }

        public bool isRunning { get; private set; } // 갱신 동작
        
        public event Action<double, double, double, float, double> onLocationUpdated;

        private double _metersPerDegreeLatitude = 111320; // 위도 1도당 약 111.32km
        private float _updateLocationInterval = 0.1f; // 업데이트 간격
        private float _updatedTimeMark; // 갱신된 시간
        private Vector3 _previousTargetPosition; // 이전 프레임 타켓의 위치 
        const float MIN_MOVE_DISTANCE = 0.01f;

        void Update()
        {
            // 시간 경과 확인
            if (Time.time - _updatedTimeMark < _updateLocationInterval)
            {
                return;
            }

            //  최소한 거리이상 움직였다면 Location 갱신
            if (Vector3.Distance(this._previousTargetPosition, this.target.position) >= MIN_MOVE_DISTANCE)
            {
                UpdateLocation();
                _previousTargetPosition = this.target.position;
            }

            _updatedTimeMark = Time.time;
        }

        public void StartService()
        {
            if (target == null)
            {
                throw new Exception("GPS 시뮬레이션 대상 없음");
            }

            isRunning = true;
            _previousTargetPosition = target.position;
            _updatedTimeMark = Time.time;
            UpdateLocation();
            Debug.Log("GPS 시뮬레이션 시작");
        }

        public void Stop()
        {
            isRunning = false;
            Debug.Log("GPS 시뮬레이션 종료");
        }

        /// <summary>
        /// target의 transform 변화에 따른 위도, 경도를 갱신
        /// </summary>
        private void UpdateLocation()
        {
            if (target == null)
            {
                return;
            }

            Vector3 currentPosition = this.target.position;

            double meterToDegreeLatitude = 1.0 / _metersPerDegreeLatitude;
            double meterToDegreeLongitude = 1.0 / (_metersPerDegreeLatitude * Math.Cos(startLocation.latitude * Mathf.Deg2Rad));

            double deltaLatitude = currentPosition.z * meterToDegreeLatitude;
            double deltaLongitude = currentPosition.x * meterToDegreeLongitude;

            double newLatitude = startLocation.latitude + deltaLatitude;
            double newLongitude = startLocation.longitude + deltaLongitude;
            
            latitude = newLatitude;
            longitude = newLongitude;

            onLocationUpdated?.Invoke(newLatitude, newLongitude, 0, 1f, DateTime.Now.Ticks);
            Debug.Log($"GPS 시뮬레이션 데이터 갱신됨 : Lat :{newLatitude}, Lng :{newLongitude}");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using FoodyGo.Mapping;
using FoodyGo.Utils;
using UnityEngine;

namespace FoodyGo.Services.GPS
{
    public class GPSLocationService : MonoBehaviour
    {
        public bool isReady { get; private set; }

        [Header("맵 타일 Settings")]
        [Tooltip("맵 타일 스케일")]
        [field: SerializeField]
        public int mapTileScale { get; private set; } = 1;
        
        [Tooltip("맵 타일 크기 (픽셀)")]
        [field: SerializeField]
        public int mapTileSizePixels { get; private set; } = 640;

        [Tooltip("맵 타일 Zoom 레벨 (1 ~ 20)")]
        [field: SerializeField]
        [Range(1, 20)]
        public int mapTileZoomLevel { get; private set; } = 15;

        [Header("Simulation Settings (Editor Only)")] 
        [SerializeField] bool _isSimulation;
        [SerializeField] Transform _simulationTarget;
        [SerializeField] MapLocation _simulationStartLocation = new MapLocation(37.4946, 127.0276056);

        public double latitude { get; private set; }
        public double longitude { get; private set; }
        public double altitude { get; private set; }
        public float accuracy { get; private set; }
        public double timeStamp { get; private set; }

        public event Action onMapReDraw;
        
        public MapLocation mapCenter;      
        public Vector3 mapWorldCenter;
        public Vector2 mapScale;
        public MapEnvelope mapEnvelope;
        
        private ILocationProvider _locationProvider;
        
        private void Awake()
        {
#if UNITY_EDITOR
            SimulatedLocationProvider simulatedLocationProvider = gameObject.AddComponent<SimulatedLocationProvider>();
            simulatedLocationProvider.target = _simulationTarget;
            simulatedLocationProvider.startLocation = _simulationStartLocation;
            _locationProvider = simulatedLocationProvider;
            timeStamp = Epoch.Now;
#else
            _locationProvider = gameObject.AddComponent<DeviceLocationProvider>();
#endif
        }

        private void OnEnable()
        {
            _locationProvider.onLocationUpdated += OnLocationUpdated;
            _locationProvider.StartService();
        }

        private void OnDisable()
        {
            _locationProvider.onLocationUpdated -= OnLocationUpdated;
            _locationProvider.Stop();
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(() => _locationProvider.isRunning);
             isReady = true;
        }

        private void OnLocationUpdated(double newLatitude, double newLongitude, double newAltitude, float newAccuracy, double newTimeStamp)
        {
            latitude = newLatitude;
            longitude = newLongitude;
            altitude = newAltitude;
            accuracy = newAccuracy;
            timeStamp = newTimeStamp;

            if (mapEnvelope.Contains(new MapLocation(latitude, longitude)))
            {
                CenterMap();
            }
        }

        private void CenterMap()
        {
            mapCenter.latitude = latitude;
            mapCenter.longitude = longitude;
            mapWorldCenter.x = GoogleMapUtils.LonToX(mapCenter.longitude);
            mapWorldCenter.y = GoogleMapUtils.LatToY(mapCenter.latitude);

            mapScale.x = (float)GoogleMapUtils.CalculateScaleX(latitude, mapTileSizePixels, mapTileScale, mapTileZoomLevel);
            mapScale.y = (float)GoogleMapUtils.CalculateScaleY(longitude, mapTileSizePixels, mapTileScale, mapTileZoomLevel);

            var lon1 = GoogleMapUtils.AdjustLonByPixels(longitude, -mapTileSizePixels/2, mapTileZoomLevel);
            var lat1 = GoogleMapUtils.AdjustLatByPixels(latitude, mapTileSizePixels/2, mapTileZoomLevel);

            var lon2 = GoogleMapUtils.AdjustLonByPixels(longitude, mapTileSizePixels/2, mapTileZoomLevel);
            var lat2 = GoogleMapUtils.AdjustLatByPixels(latitude, -mapTileSizePixels/2, mapTileZoomLevel);

            mapEnvelope = new MapEnvelope((float)lon1, (float)lat1, (float)lon2, (float)lat2);
            
            onMapReDraw?.Invoke();
        }

    }
}
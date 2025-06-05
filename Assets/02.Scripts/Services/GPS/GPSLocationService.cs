using System;
using FoodyGo.Mapping;
using UnityEngine;

namespace FoodyGo.Services.GPS
{
    public class GPSLocationService : MonoBehaviour
    {
        [Header("맵 타일 Settings")]
        [Tooltip("맵 타일 스케일")]
        [SerializeField]private int _mapTileScale = 1;
        [Tooltip("맵 타일 크기 (픽셀)")]
        [SerializeField]private int _mapTileSizePixels = 640;
        [Tooltip("맵 타일 Zoom 레벨 (1 ~ 20)")]
        [SerializeField]
        [Range(1, 20)]
        private int _mapTileZoomLevel = 15;
        
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

        private void OnLocationUpdated(double newLatitude, double newLongitude, double newAltitude, float newAccuracy, double newTimeStamp)
        {
            latitude = newLatitude;
            longitude = newLongitude;
            altitude = newAltitude;
            accuracy = newAccuracy;
            timeStamp = newTimeStamp;

            if (mapEnvelope.Contains(new MapLocation(latitude, longitude)) == false)
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

            mapScale.x = (float)GoogleMapUtils.CalculateScaleX(latitude, _mapTileSizePixels, _mapTileScale, _mapTileZoomLevel);
            mapScale.y = (float)GoogleMapUtils.CalculateScaleY(longitude, _mapTileSizePixels, _mapTileScale, _mapTileZoomLevel);

            var lon1 = GoogleMapUtils.AdjustLonByPixels(longitude, -_mapTileSizePixels/2, _mapTileZoomLevel);
            var lat1 = GoogleMapUtils.AdjustLatByPixels(latitude, _mapTileSizePixels/2, _mapTileZoomLevel);

            var lon2 = GoogleMapUtils.AdjustLonByPixels(longitude, _mapTileSizePixels/2, _mapTileZoomLevel);
            var lat2 = GoogleMapUtils.AdjustLatByPixels(latitude, -_mapTileSizePixels/2, _mapTileZoomLevel);

            mapEnvelope = new MapEnvelope((float)lon1, (float)lat1, (float)lon2, (float)lat2);
            
            onMapReDraw?.Invoke();
        }

    }
}
using System;
using System.Collections.Generic;
using FoodyGo.Services.GooglePlaces;
using FoodyGo.Services.GPS;
using UnityEngine;


namespace FoodyGo.Manager
{
    public class PlacesMarkerManager : MonoBehaviour
    {
        [SerializeField] GameObject _markerPrefab;
        [SerializeField] GooglePlacesService _googlePlacesService;
        [SerializeField] GPSLocationService _gpsLocationService;
        [SerializeField] int _markerCount = 10;
        private List<GameObject> _markers;

        private void Awake()
        {
            _markers = new List<GameObject>(_markerCount);
        }

        public void RefreshMarkers()
        {
            _googlePlacesService.SearchNearByRequest(_gpsLocationService.latitude, _gpsLocationService.longitude, 200f,new List<string>{"restaurant"}, null, null,null, _markerCount, "POPULARITY","ko", "KR", GooglePlacesService.PlacesFields.DisplayName | GooglePlacesService.PlacesFields.Types | GooglePlacesService.PlacesFields.FormattedAddress | GooglePlacesService.PlacesFields.Location, ReSpawnMarkers);
        }

        private void ReSpawnMarkers(IEnumerable<(string name, double latitude, double longitude)> places)
        {
            for (int i = _markers.Count - 1; i >= 0; i--)
            {
                Destroy(_markers[i]);
                _markers.RemoveAt(i);
            }

            foreach (var place in places)
            {
                GameObject marker = Instantiate(_markerPrefab);
                _markers.Add(marker);
            }
        }
    }    
}



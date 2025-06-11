using System;
using System.Collections;
using System.Collections.Generic;
using FoodyGo.Controllers;
using FoodyGo.Mapping;
using FoodyGo.Services.GooglePlaces;
using FoodyGo.Services.GPS;
using UnityEngine;

namespace FoodyGo.Manager
{
    public class PlacesMarkerManager : MonoBehaviour
    {
        [SerializeField] PlaceMarkerController _markerPrefab;
        [SerializeField] GooglePlacesService _googlePlacesService;
        [SerializeField] GPSLocationService _gpsLocationService;
        [SerializeField] int _markerCount = 10;
        private List<PlaceMarkerController> _markers;

        private void Awake()
        {
            _markers = new List<PlaceMarkerController>(_markerCount);
            
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(()=> _gpsLocationService.isReady);
            RefreshMarkers();
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
                PlaceMarkerController marker = Instantiate(_markerPrefab);
                marker.RefreshPlace(place.name);
                marker.transform.position = new Vector3(GoogleMapUtils.LonToUnityX(place.longitude, _gpsLocationService.mapOrigin.longitude, _gpsLocationService.mapTileZoomLevel), 0f, GoogleMapUtils.LatToUnityY(place.latitude, _gpsLocationService.mapOrigin.latitude, _gpsLocationService.mapTileZoomLevel));
                _markers.Add(marker);
            }
        }
    }    
}



using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace FoodyGo.Services.GoogleMaps
{
    /// <summary>
    /// API 웹요청 포맷
    /// </summary>
    public class GoogleStaticMapService : MonoBehaviour
    {
        private const string BASE_URL = "https://maps.googleapis.com/maps/api/staticmap?";
        private const string API_KEY = "AIzaSyCxyXbtOzkib4P_w6r96RW9VNDvUxvdC4g";
        private Texture2D _cashedTexture;

        public void LoadMap(double latitude, double longitude, float zoom, Vector2 size, Action<Texture2D> onComplete)
        {
            StartCoroutine(C_LoadMap(latitude, longitude, zoom, size, onComplete));
        }

        IEnumerator C_LoadMap(double latitude, double longitude, float zoom, Vector2 size, Action<Texture2D> onComplete)
        {
            string url = BASE_URL + "center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + size.x + "x" + size.y + "&key=" + API_KEY;
            Debug.Log($"[{nameof(GoogleStaticMapService)}] : Request map texture ... {url}");

            url = UnityWebRequest.UnEscapeURL(url);
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            
            yield return request.SendWebRequest();

            _cashedTexture = DownloadHandlerTexture.GetContent(request);
            onComplete.Invoke(_cashedTexture);
            
        }

    }    
}




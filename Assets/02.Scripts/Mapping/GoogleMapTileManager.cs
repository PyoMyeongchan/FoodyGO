using System;
using System.Collections;
using FoodyGo.Services.GoogleMaps;
using FoodyGo.Services.GPS;
using Unity.VisualScripting;
using UnityEngine;

namespace FoodyGo.Mapping
{
    /// <summary>
    /// MapTile 생성, 갱신, 제거 등의 관리
    /// GPS 데이터가 범위를 벗어날때 타일맵 확장 및 반대방향 타일맵 삭제
    /// </summary>
    public class GoogleMapTileManager : MonoBehaviour
    {
        public bool isInitialized { get; private set; }

        [Header("Configuration")]
        [SerializeField] GoogleStaticMapService _googleStaticMapService;
        [SerializeField] GPSLocationService _gpsLocationService;
        [SerializeField] GoogleMapTile _mapTilePrefab;
        [SerializeField] Transform _mapTilesParent;
        
        [Header("Debug")]
        Vector2Int _currentCenterTileCoord;
        
        [Header("Managed mapTiles")]
        GoogleMapTile[,] _mapTiles = new GoogleMapTile[GRID_SIZE,GRID_SIZE];
        readonly int[] TILE_OFFSETS = {-1, 0, 1};
        private const int GRID_SIZE = 3;

        IEnumerator Start()
        {
            yield return new WaitUntil(() => _gpsLocationService.isReady);
            InitializeTiles();
            isInitialized = true;
        }

        private void Update()
        {
            if (isInitialized == false)
            {
                return;
            }
            
            HandleCenterShift();
        }

        /// <summary>
        /// 현재 GPS 기반으로 중심 타일 인덱스 계산
        /// 3x3 배열로 MapTile들 생성
        /// </summary>
        void InitializeTiles()
        {
            _currentCenterTileCoord = CalcTileCoordinate(_gpsLocationService.mapCenter);
            CreateMapTiles(_currentCenterTileCoord);
        }

        void CreateMapTiles(Vector2Int centerTile)
        {
            // 중심 인덱스 기준으로 모든 방향 타일들 인덱스 계산
            for (int i = 0; i< TILE_OFFSETS.Length; i++)
            {
                for (int j = 0; j < TILE_OFFSETS.Length; j++)
                {
                    Vector2Int coord = new Vector2Int(centerTile.x + TILE_OFFSETS[i], centerTile.y + TILE_OFFSETS[j]);
                    
                    GoogleMapTile tile = Instantiate(_mapTilePrefab, _mapTilesParent);
                    tile.tileOffset = new Vector2Int(i - 1, j - 1);
                    tile.googleStaticMapService = _googleStaticMapService;
                    tile.zoomLevel = _gpsLocationService.mapTileZoomLevel;
                    tile.gpsLocationService = _gpsLocationService;
                    tile.name =$"MapTile_{coord.x}_{coord.y}";
                    tile.transform.position = CalcWorldPosition(coord);
                    tile.RefreshMapTile();
                    _mapTiles[i, j] = tile;
                }
            }
        }

        public Vector3 GetCenterTileWorldPosition()
        {
            return CalcWorldPosition(_currentCenterTileCoord);
        }

        /// <summary>
        /// 타일 인덱스로 게임월드 포지션 산출
        /// </summary>
        /// <param name="coord">타일 인덱스</param>
        /// <returns>월드 포지션</returns>
        Vector3 CalcWorldPosition(Vector2Int coord)
        {
            float spacing = 10;
            return new Vector3(-coord.x * spacing, 0f, coord.y * spacing);
        }



        /// <summary>
        /// 특정 위도, 경도에 해당하는 MapTile 의 인덱스를 계산 
        /// </summary>
        /// <param name="center">MapTile을 그릴 위도, 경도 중심</param>
        /// <returns>MapTile 인덱스</returns>
        Vector2Int CalcTileCoordinate(MapLocation center)
        {
            // 메르카토르 픽셀 좌표 (zoom = 21)
            int pixelX21 = GoogleMapUtils.LonToX(center.longitude);
            int pixelY21 = GoogleMapUtils.LatToY(center.latitude);

            // GoogleMap zoomLevel 1당 2배씩 값이 작아지기때문에
            // Zoomlevel 차이만큼 오른쪽으로 BIt-Shift하면 원하는 픽셀값을 구할 수 있다.
            int shift = 21 - _gpsLocationService.mapTileZoomLevel;
            int pixelX = pixelX21 >> shift;
            int pixelY = pixelY21 >> shift;
            
            // MapTile 당 픽셀수로 나누면 인덱스를 구할 수 있음
           return new Vector2Int(Mathf.RoundToInt(pixelX / (float)_gpsLocationService.mapTileSizePixels), Mathf.RoundToInt(pixelY / (float)_gpsLocationService.mapTileSizePixels));
            
        }

        private void HandleCenterShift()
        {
           Vector2Int newCenter = CalcTileCoordinate(_gpsLocationService.mapCenter);
           int dx = newCenter.x - _currentCenterTileCoord.x;
           int dy = newCenter.y - _currentCenterTileCoord.y;

           if (dx == 0 && dy == 0)
           {
               return;
           }

           if (Math.Abs(dx) == 1)
           {
               ShiftHorizontal(dx);
           }

           if (Math.Abs(dy) == 1)
           {
               ShiftVertical(dy);
           }

        }


        /// <summary>
        /// x축 방향 이동
        /// </summary>
        /// <param name="direction">타일이 움직일 방향</param>
        private void ShiftHorizontal(int direction)
        {
            if (Mathf.Abs(direction) != 1)
            {
                throw new ArgumentException("wrong direction");
            }
            
            Debug.Log($"ShiftHorizontal {direction}");

            int oldX = direction > 0 ? 0 : GRID_SIZE - 1; // 사라질 인덱스
            int newX = direction > 0 ? GRID_SIZE - 1 : 0; // 새로 생길 인덱스

            GoogleMapTile[] olds = new GoogleMapTile[GRID_SIZE];

            for (int i = 0; i < GRID_SIZE; i++)
            {
                olds[i] = _mapTiles[oldX, i];
            }

            _currentCenterTileCoord.x += direction;

            // 배열 한칸씩 다 당겨주는 코드
            if (direction > 0)
            {
                for (int x = 0; x < GRID_SIZE - 1; x++)
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        _mapTiles[x, y] = _mapTiles[x + 1, y];
                    }
                }
            }
            else
            {
                for (int x = GRID_SIZE - 1; x > 0; x--)
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        _mapTiles[x, y] = _mapTiles[x - 1, y];
                    }
                }
            }

            for (int y = 0; y < GRID_SIZE; y++)
            {
                _mapTiles[newX, y] = olds[y];
            }

            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    GoogleMapTile tile = _mapTiles[x, y];
                    Vector2Int offset = new Vector2Int(TILE_OFFSETS[x], TILE_OFFSETS[y]);
                    tile.tileOffset = offset;
                    tile.worldCenterLocation = _gpsLocationService.mapCenter;
                    tile.transform.position = CalcWorldPosition(_currentCenterTileCoord + offset);

                    if (x == newX)
                    {
                        tile.RefreshMapTile();
                    }
                }
            }
        }
        /// <summary>
        /// Z축 방향 이동
        /// </summary>
        /// <param name="direction">타일이 움직일 방향</param>
         private void ShiftVertical(int direction)
            {
                if (Mathf.Abs(direction) != 1)
                {
                    throw new ArgumentException("wrong direction");
                }

                Debug.Log($"ShiftVertical {direction}");
                
                int oldY = direction > 0 ? 0 : GRID_SIZE - 1; // 사라질 인덱스
                int newY = direction > 0 ? GRID_SIZE - 1 : 0; // 새로 생길 인덱스
            
                GoogleMapTile[] olds = new GoogleMapTile[GRID_SIZE];

                for (int i = 0; i < GRID_SIZE; i++)
                {
                    olds[i] = _mapTiles[i, oldY];
                }

                _currentCenterTileCoord.y += direction;
            
                // 배열 한칸씩 다 당겨주는 코드
                if (direction > 0)
                {
                    for (int x = 0; x < GRID_SIZE; x++)
                    {
                        for (int y = 0; y < GRID_SIZE - 1; y++)
                        {
                            _mapTiles[x, y] = _mapTiles[x, y + 1];    
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < GRID_SIZE; x++)
                    {
                        for (int y = GRID_SIZE - 1; y > 0; y--)
                        {
                            _mapTiles[x, y] = _mapTiles[x, y - 1];    
                        }
                    }
                }
        
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    _mapTiles[x, newY] = olds[x];
                }

                for (int x = 0; x < GRID_SIZE; x++)
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        GoogleMapTile tile = _mapTiles[x, y];
                        Vector2Int offset = new Vector2Int(TILE_OFFSETS[x], TILE_OFFSETS[newY]);
                        tile.tileOffset = offset;
                        tile.worldCenterLocation = _gpsLocationService.mapCenter;
                        tile.transform.position = CalcWorldPosition(_currentCenterTileCoord + offset);

                        if (y == newY)
                        {
                            tile.RefreshMapTile();
                        }
                    }
                }
            }
    }
    
}
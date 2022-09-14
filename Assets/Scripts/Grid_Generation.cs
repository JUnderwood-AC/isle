using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

using MapManagement;

namespace MapGeneration
{
    public class Grid_Generation : MonoBehaviour
    {

        [Header("Map Generation Variables")]
        [SerializeField] private TileBase[] tile;
        [SerializeField] private Tilemap map;
        [SerializeField] int height, width;

        public Grid_Tile testerTile;

        [Header("Noise Generation Variables")]
        [SerializeField] private float waterLevel;
        private float[,] _noiseMap;
        private float[,] _falloffMap;
        
        // This is only for reference whilst developing, should be removed for production.
        public List<Grid_Tile> tileSO = new List<Grid_Tile>();
        
        [Header("Grid, and tile data.")]
        private List<Grid_Tile.Tile_Data> _tileSOData = new List<Grid_Tile.Tile_Data>();
        private Dictionary<Vector3Int, Grid_Tile> _tileData;

        private void NoiseGeneration(int width, int height)
        {
            _noiseMap = new float[width, height];
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * (height * width) + xOffset, y * (height * width) + yOffset);
                    _noiseMap[x, y] = noiseValue;
                }
            }
        }

        private void FallOffGeneration(int width, int height)
        {
            _falloffMap = new float[width, height];
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    float xv = x / (float)width * 2 - 1;
                    float yv = y / (float)height * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                    _falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                }
            }
        }

        private void Start()
        {
            Vector3Int[,] positions = new Vector3Int[width, height];
            TileBase[,] tileArray = new TileBase[width, height];
            
            _tileData = new Dictionary<Vector3Int, Grid_Tile>();

            NoiseGeneration(width, height);
            FallOffGeneration(width, height);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseValue = _noiseMap[x, y];
                    noiseValue -= _falloffMap[x, y];

                    var newTile = Instantiate(testerTile);
                    newTile.data.tileNoise = noiseValue;
                    
                    map.SetTile(new Vector3Int(x, y, 0), newTile);
                    // This can be removed for production, keep for development debugging.
                    tileSO.Add(newTile);
                    _tileSOData.Add(newTile.data);
                    _tileData.Add(new Vector3Int(x, y, 0), newTile);

                }
            }

        }

        //This is purely for functional testing at present :) I am aware its hugely inefficient.
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                Vector2 mouseGridPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3Int cellposition = map.WorldToCell(mouseGridPosition);
                Debug.Log(string.Format("Co-ords of player is [X: {0} Y: {0}]", cellposition.x, cellposition.y));
                Debug.Log(map.GetTile(cellposition));

                Debug.Log(_tileData[cellposition]);
                _tileData[cellposition].tester();
                map.RefreshTile(cellposition);
            }
        }
    }
}

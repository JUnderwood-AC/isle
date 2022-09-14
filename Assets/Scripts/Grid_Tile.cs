using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapManagement
{
    public class Grid_Tile : Tile
    {
        public Sprite[] g_sprites;
        [SerializeField] public Tile_Data data;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = data.tileNoise < 0.25 ? g_sprites[1] : g_sprites[0];
        }

        public void tester()
        {
            data.isPort = true;
        }

        [System.Serializable]
        public class Tile_Data
        {
            public float tileNoise;
            public bool isPort;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Grid_Tile")]
        public static void CreateGridTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Grid Tile", "New Grid Tile", "Asset",
                "Save Grid Tile", "Assets");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Grid_Tile>(), path);
        }
#endif
    }
}

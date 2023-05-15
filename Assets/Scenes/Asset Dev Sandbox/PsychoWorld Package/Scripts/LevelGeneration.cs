using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;


public class LevelGeneration : MonoBehaviour
{
    [Header("Level Type")] 
    public bool isHome;
    public GameObject cabinPrefab;
    
    [Header("Level Properties")] 
    public int rawMapWidth;
    public int rawMapDepth;
    public int borderThickness;
    public GameObject tilePrefab;
    public GameObject flatPrefab;
    public GameObject borderPrefab;
    public GameObject centerPrefab;
    public GameObject waterPrefab;
    public GameObject waterSpecularPrefab;
    public GameObject wall;
    public int waterHeight;
    public TMP_Text countDownText;
    
    [Header("Prefab List")] 
    public Prefab[] staticPrefabs;
    public Prefab[] psychoPrefabs;
    public Prefab[] dynamicPrefabs;

    [Header("Texture Properties")]
    public int tileResolution;
    public Texture2D texture1, texture2;
    public AnimationCurve blendCurve;
    
    [Header("Height Map Perlin Noise")]
    public float levelScale;
    public float heightMultiplier;
    public AnimationCurve heightCurve;
    public Wave[] heightWave, scatterWave;
    
    [Header("Scatter Map Properties")] 
    public float scatterDensity;
    public float scatterRadius;
    
    [Header("Miscellaneous")]
    public GameObject playerPrefab;
    public static event Action OnReady;
    
    private Transform myTransform;
    private Vector3 myPosition;
    
    [NonSerialized] public int tileWidth, tileDepth;
    [NonSerialized] public int mapWidth, mapDepth;


    void Start()
    {
        BuildMap(isHome);
    }

    void BuildMap(bool home)
    {
        // Parameters up for change:
        isHome = home;
        
        // Else randomly generated
        heightWave = new Wave[3];
        for (int i = 0; i < 3; i++)
        {
            heightWave[i] = new Wave(Random.Range(1f, 9999f), Random.Range(0.01f, 1.0f),  Random.Range(0.01f, 1.0f));
        }
        
        rawMapWidth = Random.Range(4, 8);
        rawMapDepth = Random.Range(4, 8);
        
        if (borderThickness < 3)
        {
            borderThickness = 3;
        }
        mapWidth = rawMapWidth + borderThickness * 2;
        mapDepth = rawMapDepth + borderThickness * 2;
        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        tileWidth = (int)tileSize.x;
        tileDepth = (int)tileSize.z;
        
        myTransform = transform;
        myPosition = myTransform.position;
        StartCoroutine(nameof(GenerateMap));
    }

    IEnumerator GenerateMap() {

        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidth; xTileIndex++) {
            for (int zTileIndex = 0; zTileIndex < mapDepth; zTileIndex++) {
                // EX. 2x2 playable, 4x4 border, 6x6flat tiles
                // raw would be 2x2
                // border thickness would be 2
                // map width would be 6
                
                //bounds of flat would be noninclusive map-thickness = 6-2
                
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(myPosition.x + xTileIndex * tileWidth, myPosition.y, myPosition.z + zTileIndex * tileDepth);
                StartCoroutine(AsyncTileGen( xTileIndex,  zTileIndex,  tilePosition));
                yield return 0;
            }
        }
        
        yield return 0; // Raycast collision doesnt work until after the mesh generation frame
        
        // Create Player bounds (by scaling a cube)
        GameObject north, south, east, west;
        north = Instantiate(wall, new Vector3((mapWidth-1)*tileWidth/2f, 0, (mapDepth-1)*tileDepth), Quaternion.identity);
        north.transform.localScale = new (mapWidth*tileWidth-tileWidth,2*levelScale+levelScale,1);
        north.gameObject.name = "North Wall";
        
        south = Instantiate(wall, new Vector3((mapWidth-1)*tileWidth/2f, 0, 0), Quaternion.identity);
        south.transform.localScale = new (mapWidth*tileWidth-tileWidth,2*levelScale+levelScale,1);
        south.gameObject.name = "South Wall";
        
        east = Instantiate(wall, new Vector3((mapWidth-1)*tileWidth, 0, (mapDepth-1)*tileDepth/2f), Quaternion.identity);
        east.transform.localScale = new (1,2*levelScale+levelScale,mapDepth*tileDepth-tileDepth);
        east.gameObject.name = "East Wall";
        
        west = Instantiate(wall, new Vector3(0, 0, (mapDepth-1)*tileDepth/2f), Quaternion.identity);
        west.transform.localScale = new (1,2*levelScale+levelScale,mapDepth*tileDepth-tileDepth);
        west.gameObject.name = "West Wall";
        
        yield return new WaitForSeconds(1);
        
        NavMeshSurface navMeshSurface = gameObject.GetComponent<NavMeshSurface>();

        // Generate the NavMesh
        navMeshSurface.BuildNavMesh();
        
        yield return new WaitForSeconds(1);
        
        
        // Instantiate Player object and water planes
        Vector2 centerPos = new(mapWidth * tileWidth / 2f - (tileWidth / 2f) , mapDepth * tileDepth / 2f - (tileDepth / 2f));
        InstantiateObjectInMap(playerPrefab, centerPos.x, centerPos.y);
        Instantiate(waterPrefab, new Vector3(centerPos.x, waterHeight, centerPos.y), Quaternion.identity).transform.localScale = new Vector3(mapWidth, 1, mapDepth);
        Instantiate(waterSpecularPrefab, new Vector3(centerPos.x, waterHeight, centerPos.y), Quaternion.identity).transform.localScale = new Vector3(mapWidth, 1, mapDepth);

        yield return new WaitForSeconds(0.5f);
        
        if (!isHome) StartCoroutine(StartCountdown());
        
        // Broadcast ready state to initialize all the enemies' scripts
        Debug.Log("Invoking Ready State");
        OnReady?.Invoke();
    }

    private IEnumerator AsyncTileGen(int xTileIndex, int zTileIndex, Vector3 tilePosition)
    {
                        
        //  0-1 and 5-6
        if((xTileIndex >= 0 && xTileIndex < borderThickness-1 || xTileIndex > mapWidth-borderThickness && xTileIndex <= mapWidth)
           || (zTileIndex >= 0 && zTileIndex < borderThickness-1 || zTileIndex > mapDepth-borderThickness && zTileIndex <= mapDepth)
          )
        {
            // Flat (completely flat)
            Instantiate (flatPrefab, tilePosition, Quaternion.identity, myTransform);
        }
        // 2 and 4
        else if((xTileIndex == borderThickness-1 || xTileIndex == mapWidth-borderThickness )
                || (zTileIndex == borderThickness-1 || zTileIndex == mapDepth-borderThickness))
        {
            // Border (Interpolated height)
            Instantiate (borderPrefab, tilePosition, Quaternion.identity, myTransform);
        }else if (xTileIndex == mapWidth/2 && zTileIndex == mapDepth/2)
        {
            // Center Tile OR Level Tile, depending on whether we are in Psycho or Normal
            Instantiate(isHome ? centerPrefab : tilePrefab, tilePosition, Quaternion.identity, myTransform);
        }
        else
        {
            // Level (noisemap Assets)
            Instantiate (tilePrefab, tilePosition, Quaternion.identity, myTransform);
        }
        yield return 0;
    }
    
    private void InstantiateObjectInMap(GameObject prefab, float x, float y)
    {
        Vector3 pos = transform.position;
        Vector3 prefabPos = new(pos.x + x - tileWidth/2f + 0.5f, pos.y + 10, pos.z + y - tileDepth/2f + 0.5f);
        Ray ray = new(prefabPos, Vector3.down);
        if (Physics.Raycast(ray, out var hit, 15f))
        {
            prefabPos = hit.point + Vector3.up;
        }
        Instantiate(prefab, prefabPos, Quaternion.identity);
    }

    IEnumerator StartCountdown()
    {
        yield return 0;
        // We must call this coroutine after player instantiation
        GameObject player = GameObject.FindGameObjectWithTag("Player").transform.Find("Timer (Canvas)").gameObject;
        player.SetActive(true);
        
        float countdownTime = 120f;
        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;

            if (countdownTime < 30)
            {
                player.GetComponent<CountdownTimer>().countdown.text = "Return Home: " + countdownTime.ToString();
            }
        }
        
        SceneManager.LoadScene("HomeWorld");
    }
}



[Serializable]
public class Prefab {
    public GameObject prefab;
    public bool useScaleCurve;
    public AnimationCurve scale;
}

[Serializable]
public class Wave {
    public float seed ;
    public float frequency;
    public float amplitude;

    public Wave(float sd, float freq, float amp)
    {
        seed = sd;
        frequency = freq; 
        amplitude = amp;
    }
}
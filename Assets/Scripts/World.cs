using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

// World
// 블록의 텍스쳐 정보를 저장하기 위한 컴포넌트
// 
// 자료형=========================================================================================
// biomes                           >> 지형 형성을 위한 정보
// 
// playerCamera                 >> 게임 카메라의 위치정보
// spawnPosition                 >> 게임 카메라가 스폰되는 위치값
// 
// Material                          >> 텍스처 아틀라스를 입력
// BlockType                       >> 블록의 텍스처 정보 배열
//// chunksToCreate               >> 새로 생성해야 하는 청크들의 리스트
// 
// chunks                            >> 청크들 이차원 배열
// activeChunks                   >> 현재 맵에 보이게끔 설정된 청크
// 
// modifications                   >> 나무 등 일반 지형과 다른 블록 생성시 블록 배치를 위한 선입선출 이중 큐
// chunksToUpdate              >> 업데이트해야 하는 청크 리스트
// chunksToDraw                 >> 메쉬를 그려야 하는 청크 큐
// 
// creativeInventoryWindow >> 크리에이티브모드 인벤토리창
// cursorSlot                       >> 마우스 커서의 아이템 슬롯
// 
// globalLightLevel               >> 맵의 전체적인 밝기 조정
// day                                 >> 낮의 배경색
// night                               >> 밤의 배경색
// 
// debugScreen                    >> 디버그스크린 오브젝트
// 
// ChunkUpdateThread          >> 청크 업데이트기능을 수행하는 스레드
// ChunkUpdateThreadLock   >> 청크 업데이트 스레드 잠금용도
// ChunkListThreadLock         >> 청크 리스트 스레드 잠금용도?
// 
// clouds                             >> 구름 관련 컴포넌트
// 
// inUI                                >> 인벤토리를 표시하는지에 대한 bool값 정의
//  - get, set = _inUI
// 
// instance                          >> 유일한 오브젝트의 월드 컴포넌트(싱글턴 패턴)
// 
// worldData                        >> 월드의 각종 정보
// 
// 
// 메서드=========================================================================================
//// GenerateWorld()
//// 월드가 시작될 때, VoxelData.worldSizeInChunks만큼의 가로세로 개수의 청크 생성
// 
//// CreateChunk()
//// 청크를 생성하는 메서드
//// 초기화에 가까운 생성 코드이고, 메쉬 생성 안함
// 
// UpdateChunks()
// 중간에 여러 이유로 값이 바뀐 청크의 블록을 새로 업데이트시키는 메서드
// 
// ApplyModifications()
// 바뀌어야 하는 블록 리스트가 있다면 이를 청크에 전달시킴
// 이때, 나무 등 구조물 생성을 위해 청크를 생성시킬 때 배지채야 하는 블록 정보도 전달
// 
// GetChunkCoord(Vector3 pos)
// pos가 해당되는 청크좌표 반환
// 
// CheckViewDistance()
// 플레이어 카메라와 근접한 위치에 청크가 존재하는지 확인, 플레이어가 있는 청크의 일정 범위 안에 있는 청크들만 골라서 활성화시킴
// 
// CheckForVoxel(float _x, float _y, float _z)
// 글로벌 좌표값으로 해당 좌표에 블럭이 있는지 확인
// 
// GetVoxelState(Vector3 pos)
// CheckForVoxel이랑 거의 동일하지만 블럭 존재 여부를 확인하지 않고 불투명한 블록 여부를 확인한다.s
// 
// GetVoxel(Vector3 pos)
// 해당 좌표에 블럭이 있는지 반환하는 메서드
// 청크를 생성하는 코드에 관여하고 있지만(Chunk클래스의 PopulateVoxelMap), 이미 해당되는 청크가 한번 구현된 바가 있다면 기존에 사용하던 값을 불러와야 함
// 
// IsChunkInWorld(ChunkCoord coord)
// ChunkCoord 좌표에 해당하는 청크가 맵 안에 있는지 반환한다
// 청크 생성 및 구현단계에서는 나오지 않고, 해당 청크를 보여주기 위한 거리계산에서 사용된다
// 
// IsVoxelInWorld(Vector3 pos)
// Vector3좌표가 맵 안에 있는지 반환
// 
// GetChunkFromVector3(Vector3 pos)
// pos에 해당되는 청크 반환
// 
// SetGlobalLightValue()
// 마크 월드의 밝기 광원을 조절하는 스크립트
// 
// LoadWorld()
// 
// 
// 
// 
public class World : MonoBehaviour
{
    public Settings settings;

    [Header("World Generation Values")]
    public BiomeAttribute[] biomes;
    //public int seed;

    //[Header("Performance")]
    //public bool enableThreading;

    [Range(0f, 1f)]
    public float globalLightLevel;
    public Color day, night;

    public Transform playerCamera;
    public Vector3 spawnPosition;

    public Material material, transparentMaterial;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>(); //, chunksToCreate = new List<ChunkCoord>();
    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastCoord;

    private List<Chunk> chunksToUpdate = new List<Chunk>();
    public Queue<Chunk> chunksToDraw = new Queue<Chunk>();

    bool applyingModifications = false;
    
    Queue<Queue<VoxelMod>> modifications = new Queue<Queue<VoxelMod>>();

    private bool _inUI = false;

    public Clouds clouds;

    public GameObject debugScreen, creativeInventoryWindow, cursorSlot;

    Thread ChunkUpdateThread;
    public object ChunkUpdateThreadLock = new object(), ChunkListThreadLock = new object();

    private static World _instance;
    public static World instance { get { return _instance; } }

    public WorldData worldData;

    public string appPath;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;

        appPath = Application.persistentDataPath;

        string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
        settings = JsonUtility.FromJson<Settings>(jsonImport);
    }

    private void Start()
    {
        worldData = SaveSystem.LoadWorld("Prototype");

        Random.InitState(VoxelData.seed);

        Shader.SetGlobalFloat("minGlobalLightLevel", VoxelData.minLightLevel);
        Shader.SetGlobalFloat("maxGlobalLightLevel", VoxelData.maxLightLevel);

        LoadWorld();

        SetGlobalLightValue();

        spawnPosition = new Vector3(VoxelData.worldCenter, VoxelData.chunkHeight - 50f, VoxelData.worldCenter);
        //GenerateWorld();
        playerCamera.position = spawnPosition;
        CheckViewDistance();
        playerLastCoord = GetChunkCoord(playerCamera.position);

        if (settings.enableThreading)
        {
            ChunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
            ChunkUpdateThread.Start();
        }
    }

    public void SetGlobalLightValue()
    {
        Shader.SetGlobalFloat("GlobalLightLevel", globalLightLevel);
        Camera.main.backgroundColor = Color.Lerp(night, day, globalLightLevel);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoord(playerCamera.position);

        //material.SetFloat("GlobalLightLevel", globalLightLevel);

        if (!playerChunkCoord.Equals(playerLastCoord)) CheckViewDistance();

        //if (chunksToCreate.Count > 0 && !isCreatingChunks) StartCoroutine("CreateChunks");

        //if (chunksToCreate.Count > 0) CreateChunk();

        if(chunksToDraw.Count > 0)
        {
                //if (chunksToDraw.Peek().isEditable) 
            chunksToDraw.Dequeue().CreateMesh();
        }

        if (!settings.enableThreading)
        {
            if (!applyingModifications) ApplyModifications();

            if (chunksToUpdate.Count > 0) UpdateChunks();
        }
        
        if (Input.GetKeyDown(KeyCode.F3)) debugScreen.SetActive(!debugScreen.activeSelf);

        if (Input.GetKeyDown(KeyCode.F1)) SaveSystem.SaveWorld(worldData);
    }

    void LoadWorld()
    {
        for (int x = (VoxelData.worldSizeInChunks / 2) - settings.loadDistance; x < (VoxelData.worldSizeInChunks / 2) + settings.loadDistance; x++)
            for (int z = (VoxelData.worldSizeInChunks / 2) - settings.loadDistance; z < (VoxelData.worldSizeInChunks / 2) + settings.loadDistance; z++)
            {
                worldData.LoadChunk(new Vector2Int(x, z));
            }
    }

    //void GenerateWorld()
    //{
    //    for (int x = (VoxelData.worldSizeInChunks / 2) - settings.viewDistance; x < (VoxelData.worldSizeInChunks / 2) + settings.viewDistance; x++)
    //        for (int z = (VoxelData.worldSizeInChunks / 2) - settings.viewDistance; z < (VoxelData.worldSizeInChunks / 2) + settings.viewDistance; z++)
    //        {
    //            ChunkCoord newChunk = new ChunkCoord(x, z);
    //            chunks[x, z] = new Chunk(newChunk); //, true);
    //            //chunksToCreate.Add(newChunk);
    //            //activeChunks.Add(new ChunkCoord(x, z));
    //        }
    //    //for(int i = 0; i < chunksToUpdate.Count; i++)
    //    //{
    //    //    chunksToUpdate[0].UpdateChunk();
    //    //    chunksToUpdate.RemoveAt(0);
    //    //}

    //}

    //void CreateChunk()
    //{
    //    ChunkCoord c = chunksToCreate[0];
    //    chunksToCreate.RemoveAt(0);
    //    //activeChunks.Add(c);
    //    chunks[c.x, c.z].Init();
    //}

    public void AddChunkToUpdate(Chunk chunk)
    {
        AddChunkToUpdate(chunk, false);
    }

    public void AddChunkToUpdate(Chunk chunk, bool insert)
    {
        lock (ChunkUpdateThreadLock)
        {
            if (!chunksToUpdate.Contains(chunk))
            {
                if (insert) chunksToUpdate.Insert(0, chunk);
                else chunksToUpdate.Add(chunk);
            }
        }
    }


    void UpdateChunks()
    {
        //bool updated = false;
        //int index = 0;

        lock (ChunkUpdateThreadLock)
        {
            //while (!updated && index < chunksToUpdate.Count - 1)
            //{
                //if (chunksToUpdate[index].isEditable)
                //{
                    chunksToUpdate[0].UpdateChunk();

                    if(!activeChunks.Contains(chunksToUpdate[0].coord))
                        activeChunks.Add(chunksToUpdate[0].coord);

                    chunksToUpdate.RemoveAt(0);
                    //updated = true;
                //}
                //else index++;
            //}
        }
    }

    void ThreadedUpdate()
    {
        while (true)
        {
            if (!applyingModifications) ApplyModifications();

            if (chunksToUpdate.Count > 0) UpdateChunks();
        }
    }

    private void OnDisable()
    {
        if (settings.enableThreading)
            ChunkUpdateThread.Abort();
    }

    void ApplyModifications()
    {
        applyingModifications = true;
        while (modifications.Count > 0)
        {
            Queue<VoxelMod> queue = modifications.Dequeue();
            while (queue.Count > 0)
            {
                VoxelMod v = queue.Dequeue();

                worldData.SetVoxel(v.position, v.id);

                //ChunkCoord c = GetChunkCoord(v.position);

                //if (chunks[c.x, c.z] == null)
                //{
                //    chunks[c.x, c.z] = new Chunk(c); 
                //    chunksToCreate.Add(c);
                //}

                //chunks[c.x, c.z].modifications.Enqueue(v);
            }
        }

        applyingModifications = false;
    }

    ChunkCoord GetChunkCoord(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth), z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);
        return new ChunkCoord(x, z);
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth), z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);
        return chunks[x, z];
    }

    void CheckViewDistance()
    {
        clouds.UpdateClouds();

        ChunkCoord coord = GetChunkCoord(playerCamera.position);
        playerLastCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        activeChunks.Clear();

        for (int x = coord.x - settings.viewDistance; x < coord.x + settings.viewDistance; x++)
        {
            for (int z = coord.z - settings.viewDistance; z < coord.z + settings.viewDistance; z++)
            {
                ChunkCoord thisChunkCoord = new ChunkCoord(x, z);

                if (IsChunkInWorld(thisChunkCoord))
                {
                    if (chunks[x, z] == null)
                    //{
                        chunks[x, z] = new Chunk(thisChunkCoord); //, false);
                        //chunksToCreate.Add(thisChunkCoord);
                    //}
                    //else if (!chunks[x, z].IsActive)
                    //{
                        chunks[x, z].IsActive = true;
                    //}
                    activeChunks.Add(thisChunkCoord);
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(thisChunkCoord))
                        previouslyActiveChunks.RemoveAt(i);
                }

            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].IsActive = false;
    }

    public bool CheckForVoxel(Vector3 pos)
    {
        //ChunkCoord thisChunk = new ChunkCoord(pos);

        //if (!IsVoxelInWorld(pos)) return false;
        //if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.chunkHeight) return false;

        //if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isEditable)
        //    return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFormGlobalVector3(pos).id].isSolid;

        //return blockTypes[GetVoxel(pos)].isSolid;

        VoxelState voxel = worldData.GetVoxel(pos);

        if (blockTypes[voxel.id].isSolid) return true;
        else return false;
    }

    public VoxelState GetVoxelState(Vector3 pos)
    {
        //ChunkCoord thisChunk = new ChunkCoord(pos);

        //if (!IsVoxelInWorld(pos)) return false;
        //if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.chunkHeight) return null;

        //if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isEditable)
        //    return chunks[thisChunk.x, thisChunk.z].GetVoxelFormGlobalVector3(pos);

        //return new VoxelState(GetVoxel(pos));

        return worldData.GetVoxel(pos);
    }

    public bool inUI
    {
        get { return _inUI; }
        set
        {
            _inUI = value;
            if (_inUI)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                creativeInventoryWindow.SetActive(true);
                cursorSlot.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                creativeInventoryWindow.SetActive(false);
                cursorSlot.SetActive(false);
            }
        }
    }

    public ushort GetVoxel(Vector3 pos)
    {
        /* 중요한 값들이므로 함부로 건들지 말것 */

        int yPos = Mathf.FloorToInt(pos.y);

        if (!IsVoxelInWorld(pos)) return 0;

        if (yPos == 0) return 1;

        /*바이옴 선택*/
        int solidGroundHeight = 42, count = 0, strongestBiomeIndex = 0; //index, 
        float sumOfHeights = 0f, strongestWeight = 0f;

        for(int i = 0; i < biomes.Length; i++)
        {
            float weight = Noise.get2DPerlin(new Vector2(pos.x, pos.z), biomes[i].offset, biomes[i].scale);

            if(weight > strongestWeight)
            {
                strongestWeight = weight;
                strongestBiomeIndex = i;
            }

            float height = biomes[i].terrainHeight * Noise.get2DPerlin(new Vector2(pos.x, pos.z), 0, biomes[i].terrainScale) * weight;

            if(height > 0)
            {
                sumOfHeights += height;
                count++;
            }
        }

        BiomeAttribute biome = biomes[strongestBiomeIndex];

        sumOfHeights /= count;

        int terrainHeight = Mathf.FloorToInt(sumOfHeights + solidGroundHeight);

        //if (Noise.get2DPerlin(new Vector2(pos.x, pos.z), 123445, 2f) > 0.5f) index = 0;
        //else index = 1;


        /* 언덕 만들기 */
        //int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + solidGroundHeight;
        ushort voxelValue = 0;

        if (yPos == terrainHeight) voxelValue = biome.surfaceBlock;
        else if (yPos <= terrainHeight - 7) voxelValue = 4;
        else if (yPos <= terrainHeight) voxelValue = biome.subSurfaceBlock;
        else voxelValue = 0;

        /* 광맥 만들기 */
        if (voxelValue == 4)
        {
            foreach(Lode lode in biome.lodes)
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (Noise.get3DPerlin(pos, lode.noiseOffest, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
        }

        /* 나무 만들기 */
        if(yPos == terrainHeight && biome.placeMajorFlora)
        {
            if (Noise.get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.majorFloraZoneScale) > biome.majorFloraZoneThreshold)
            {
                if(Noise.get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.majorFloraPlacementScale) > biome.majorFloraPlacementThreshold)
                {
                    modifications.Enqueue(Structure.GenerateMajorFlora(biome.majorFloraindex, pos, biome.minHeight, biome.maxHeight));
                }
            }
        }
        
        return voxelValue;
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < VoxelData.worldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.worldSizeInChunks - 1) return true;
        else return false;
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.worldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.chunkHeight && pos.z >=0 && pos.z < VoxelData.worldSizeInVoxels) return true;
        else return false;
    }
}

// BlockType 클래스
// blockname                      >> 블록 이름
// isSolid                            >> 체크 해제시 해당 블록은 공기취급
// texture_~                       >> 블록의 해당 방향 텍스처 정보
// icon                                >> 블록의 아이콘 스프라이트
// renderNeighborFaces       >> 블록이 투과성을 띄고 있는지 여부
// transparentMaterial          >> 투명블록 머티리얼
// opacity                            >> 블록이 투과성이거나 할 때 그림자가 조금씩 짐
// 
// GetTextureID(int faceIndex)
// 6개 방향의 면에 대한 텍스처 정보를 반환
// short 개수의 블럭이라도 한 블럭당 최대 6면의 방향이 있으므로, int값의 반환이라도 괜찮은듯
[System.Serializable]
public  class BlockType
{
    public string blockname;
    public bool isSolid, renderNeighborFaces;
    public byte opacity;
    public Sprite icon;

    [Header("Texture Values")]
    public int texture_back;
    public int texture_front;
    public int texture_top;
    public int texture_bottom;
    public int texture_left;
    public int texture_right;
    //back front top bottom left right
    public int GetTextureID(ushort faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return texture_back;
            case 1:
                return texture_front;
            case 2:
                return texture_top;
            case 3:
                return texture_bottom;
            case 4:
                return texture_left;
            case 5:
                return texture_right;
            default : Debug.Log("Texture err : We don't have sutch id");
                return 0;
        }
    }
}

// VoxelMod 클래스
// 임의의 좌표에 해당되는 id의 블록을 생성하기 위한 정보
// position                   >> 좌표
// id                            >> 블럭 id
public class VoxelMod
{
    public Vector3 position;
    public ushort id;

    public VoxelMod()
    {
        position = new Vector3();
        id = 0;
    }

    public VoxelMod(Vector3 _position, ushort _id)
    {
        position = _position;
        id = _id;
    }
}

// Settings 클래스
// 설정 파일을 불러와서 사용할 수 있는 정보
// viewDistance               >> 청크를 구현하는 범위(청크가 보이는 범위)
// loadDistance               >> 청크를 불러오는 범위
// mouseSensitivity          >> 마우스 감도...?
// enableThreading           >> 스레드의 동작 여부
// enableAnimatedChunks >> 청크 애니메이션 적용 여부
// 
[System.Serializable]
public class Settings
{
    [Header("Game Data")]
    public string version = "0.0.01";

    [Header("Performance")]
    public int viewDistance = 8, loadDistance = 16;
    public CloudStyle clouds = CloudStyle.Fancy;
    public bool enableThreading = true, enableAnimatedChunks = false;

    [Header("Controls")]
    [Range(0.1f, 10f)]
    public float mouseSensitivity = 2f;

    //[Header("World Gen")]
    //public int seed;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

// World
// ����� �ؽ��� ������ �����ϱ� ���� ������Ʈ
// 
// �ڷ���=========================================================================================
// biomes                           >> ���� ������ ���� ����
// 
// playerCamera                 >> ���� ī�޶��� ��ġ����
// spawnPosition                 >> ���� ī�޶� �����Ǵ� ��ġ��
// 
// Material                          >> �ؽ�ó ��Ʋ�󽺸� �Է�
// BlockType                       >> ����� �ؽ�ó ���� �迭
//// chunksToCreate               >> ���� �����ؾ� �ϴ� ûũ���� ����Ʈ
// 
// chunks                            >> ûũ�� ������ �迭
// activeChunks                   >> ���� �ʿ� ���̰Բ� ������ ûũ
// 
// modifications                   >> ���� �� �Ϲ� ������ �ٸ� ��� ������ ��� ��ġ�� ���� ���Լ��� ���� ť
// chunksToUpdate              >> ������Ʈ�ؾ� �ϴ� ûũ ����Ʈ
// chunksToDraw                 >> �޽��� �׷��� �ϴ� ûũ ť
// 
// creativeInventoryWindow >> ũ������Ƽ���� �κ��丮â
// cursorSlot                       >> ���콺 Ŀ���� ������ ����
// 
// globalLightLevel               >> ���� ��ü���� ��� ����
// day                                 >> ���� ����
// night                               >> ���� ����
// 
// debugScreen                    >> ����׽�ũ�� ������Ʈ
// 
// ChunkUpdateThread          >> ûũ ������Ʈ����� �����ϴ� ������
// ChunkUpdateThreadLock   >> ûũ ������Ʈ ������ ��ݿ뵵
// ChunkListThreadLock         >> ûũ ����Ʈ ������ ��ݿ뵵?
// 
// clouds                             >> ���� ���� ������Ʈ
// 
// inUI                                >> �κ��丮�� ǥ���ϴ����� ���� bool�� ����
//  - get, set = _inUI
// 
// instance                          >> ������ ������Ʈ�� ���� ������Ʈ(�̱��� ����)
// 
// worldData                        >> ������ ���� ����
// 
// 
// �޼���=========================================================================================
//// GenerateWorld()
//// ���尡 ���۵� ��, VoxelData.worldSizeInChunks��ŭ�� ���μ��� ������ ûũ ����
// 
//// CreateChunk()
//// ûũ�� �����ϴ� �޼���
//// �ʱ�ȭ�� ����� ���� �ڵ��̰�, �޽� ���� ����
// 
// UpdateChunks()
// �߰��� ���� ������ ���� �ٲ� ûũ�� ����� ���� ������Ʈ��Ű�� �޼���
// 
// ApplyModifications()
// �ٲ��� �ϴ� ��� ����Ʈ�� �ִٸ� �̸� ûũ�� ���޽�Ŵ
// �̶�, ���� �� ������ ������ ���� ûũ�� ������ų �� ����ä�� �ϴ� ��� ������ ����
// 
// GetChunkCoord(Vector3 pos)
// pos�� �ش�Ǵ� ûũ��ǥ ��ȯ
// 
// CheckViewDistance()
// �÷��̾� ī�޶�� ������ ��ġ�� ûũ�� �����ϴ��� Ȯ��, �÷��̾ �ִ� ûũ�� ���� ���� �ȿ� �ִ� ûũ�鸸 ��� Ȱ��ȭ��Ŵ
// 
// CheckForVoxel(float _x, float _y, float _z)
// �۷ι� ��ǥ������ �ش� ��ǥ�� ���� �ִ��� Ȯ��
// 
// GetVoxelState(Vector3 pos)
// CheckForVoxel�̶� ���� ���������� �� ���� ���θ� Ȯ������ �ʰ� �������� ��� ���θ� Ȯ���Ѵ�.s
// 
// GetVoxel(Vector3 pos)
// �ش� ��ǥ�� ���� �ִ��� ��ȯ�ϴ� �޼���
// ûũ�� �����ϴ� �ڵ忡 �����ϰ� ������(ChunkŬ������ PopulateVoxelMap), �̹� �ش�Ǵ� ûũ�� �ѹ� ������ �ٰ� �ִٸ� ������ ����ϴ� ���� �ҷ��;� ��
// 
// IsChunkInWorld(ChunkCoord coord)
// ChunkCoord ��ǥ�� �ش��ϴ� ûũ�� �� �ȿ� �ִ��� ��ȯ�Ѵ�
// ûũ ���� �� �����ܰ迡���� ������ �ʰ�, �ش� ûũ�� �����ֱ� ���� �Ÿ���꿡�� ���ȴ�
// 
// IsVoxelInWorld(Vector3 pos)
// Vector3��ǥ�� �� �ȿ� �ִ��� ��ȯ
// 
// GetChunkFromVector3(Vector3 pos)
// pos�� �ش�Ǵ� ûũ ��ȯ
// 
// SetGlobalLightValue()
// ��ũ ������ ��� ������ �����ϴ� ��ũ��Ʈ
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
        /* �߿��� �����̹Ƿ� �Ժη� �ǵ��� ���� */

        int yPos = Mathf.FloorToInt(pos.y);

        if (!IsVoxelInWorld(pos)) return 0;

        if (yPos == 0) return 1;

        /*���̿� ����*/
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


        /* ��� ����� */
        //int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + solidGroundHeight;
        ushort voxelValue = 0;

        if (yPos == terrainHeight) voxelValue = biome.surfaceBlock;
        else if (yPos <= terrainHeight - 7) voxelValue = 4;
        else if (yPos <= terrainHeight) voxelValue = biome.subSurfaceBlock;
        else voxelValue = 0;

        /* ���� ����� */
        if (voxelValue == 4)
        {
            foreach(Lode lode in biome.lodes)
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (Noise.get3DPerlin(pos, lode.noiseOffest, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
        }

        /* ���� ����� */
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

// BlockType Ŭ����
// blockname                      >> ��� �̸�
// isSolid                            >> üũ ������ �ش� ����� �������
// texture_~                       >> ����� �ش� ���� �ؽ�ó ����
// icon                                >> ����� ������ ��������Ʈ
// renderNeighborFaces       >> ����� �������� ��� �ִ��� ����
// transparentMaterial          >> ������ ��Ƽ����
// opacity                            >> ����� �������̰ų� �� �� �׸��ڰ� ���ݾ� ��
// 
// GetTextureID(int faceIndex)
// 6�� ������ �鿡 ���� �ؽ�ó ������ ��ȯ
// short ������ ���̶� �� ���� �ִ� 6���� ������ �����Ƿ�, int���� ��ȯ�̶� ��������
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

// VoxelMod Ŭ����
// ������ ��ǥ�� �ش�Ǵ� id�� ����� �����ϱ� ���� ����
// position                   >> ��ǥ
// id                            >> �� id
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

// Settings Ŭ����
// ���� ������ �ҷ��ͼ� ����� �� �ִ� ����
// viewDistance               >> ûũ�� �����ϴ� ����(ûũ�� ���̴� ����)
// loadDistance               >> ûũ�� �ҷ����� ����
// mouseSensitivity          >> ���콺 ����...?
// enableThreading           >> �������� ���� ����
// enableAnimatedChunks >> ûũ �ִϸ��̼� ���� ����
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

// AtlasPacker 클래스
// 텍스처 아틀라스 팩커의 유니티 에디터 원도우 창 스크립트
// 
// 자료형=========================================================================================
// blockSize             >> 블럭 하나당 픽셀 수(텍스쳐는 정사각형)
// atlasSizeInBlocks >> 텍스쳐 아틀라스의 블럭 개수(텍스쳐 아틀라스 결과물은 정사각형)
// atlasSize             >> 텍스쳐 아틀라스의 
// rowTextures         >> 텍스쳐 아틀라스를 만들기 위해 읽어온 폴더 안의 에셋 배열
// sortedTextures     >> rowTextures에서 걸러내어진 실제 아틀라스용 텍스쳐 리스트
// atlas                   >> 결과물 텍스쳐 아틀라스
// 
// 메서드=========================================================================================
// LoadTextures()
// 텍스쳐를 정해진 위치의 폴더에서 로드하는 메서드
// 
// PackAtlas()
// 텍스쳐 아틀라스를 생성하는 메서드
// 
public class AtlasPacker : EditorWindow
{
    int blockSize = 16, atlasSizeInBlocks = 16, atlasSize;
    Object[] rawTextures = new Object[256];
    List<Texture2D> sortedTextures = new List<Texture2D>();
    Texture2D atlas;

    [MenuItem("Minecraft Clone/Atlas Packer")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    private void OnGUI()
    {
        atlasSize = blockSize * atlasSizeInBlocks;

        GUILayout.Label("Minecraft Clone Texture Atlas Packer", EditorStyles.boldLabel);

        blockSize = EditorGUILayout.IntField("Block Size", blockSize);
        atlasSizeInBlocks = EditorGUILayout.IntField("Atlas Size(in blocks)", atlasSizeInBlocks);

        GUILayout.Label(atlas);

        if(GUILayout.Button("Load Textures"))
        {
            LoadTextures();
            PackAtlas();
        }

        if(GUILayout.Button("Clear Textures"))
        {
            atlas = new Texture2D(atlasSize, atlasSize);

            Debug.Log("Atlas Packer : Textures Clear");
        }

        if(GUILayout.Button("Save Atlas"))
        {
            byte[] bytes = atlas.EncodeToPNG();

            try{
                File.WriteAllBytes(Application.dataPath + "/Textures/Packed_Altas.png", bytes);
            } catch
            {
                Debug.Log("Atlas Packer : Save fail");
            }
        }
    }

    void LoadTextures()
    {
        sortedTextures.Clear();
        rawTextures = Resources.LoadAll("AtlasPacker", typeof(Texture2D));

        int index = 0;
        foreach(Object tex in rawTextures)
        {
            Texture2D t = (Texture2D)tex;
            
            if(t.width == blockSize && t.height == blockSize) sortedTextures.Add(t);
            else Debug.Log("Atlas Packer : " + tex.name + " load fail");

            index++;
        }

        Debug.Log("Atlas Packer : " + sortedTextures.Count + " load succes");
    }

    void PackAtlas()
    {
        atlas = new Texture2D(atlasSize, atlasSize);
        Color[] pixels = new Color[atlasSize * atlasSize];

        for(int x = 0; x < atlasSize; x++)
        {
            int currentBlockX = x / blockSize;

            for(int y = 0; y < atlasSize; y++)
            {
                int currentBlockY = y / blockSize;

                int index = currentBlockY * atlasSizeInBlocks + currentBlockX;

                int currentPixelX = x - (currentBlockX * blockSize), currentPixelY = y - (currentBlockY * blockSize);

                if (index < sortedTextures.Count)
                    pixels[y * atlasSize + x] = sortedTextures[index].GetPixel(x, blockSize - y - 1);
                else pixels[y * atlasSize + x] = new Color(0f, 0f, 0f, 0f);
            }
        }

        atlas.SetPixels(pixels);
        atlas.Apply();
    }

}

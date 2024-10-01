using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BiomeAttribute
// ScriptableObject (������ ������ �����̳�)
// 
// �ڷ���=========================================================================================
// biomeName                            >> ������ �̸�
// terrainHeight                          >> ���� ���� ������ ������ ����
// terrainScale                            >> ������ ������ ũ�⸦ �����ϴ� ������ �����ϰ�
// 
// majorFloraZoneScale               >> �Ĺ��� �����Ǵ� ���� ũ��
// majorFloraZoneThreshold         >> �Ĺ��� �����Ǵ� ���� ������� ������ ������
// majorFloraZoneThreshold         >> �Ĺ� �������������� �Ĺ� ���� ��
// majorFloraPlacementScale       >> �Ĺ� ���� �� ������
// maxHeight                             >> �����Ǵ� ���� �ִ����
// minHeight                              >> �����Ǵ� ���� �ּҳ���
// placeMajorFlora                      >> �⺻ �Ĺ��� �����ϴ��� ����
// offset                                     >> ���� ������
// scale                                      >> ���� ������
// 
[CreateAssetMenu(fileName = "BiomeAttribute", menuName = "Biome Attribute")]
public class BiomeAttribute : ScriptableObject
{
    [Header("Biome")]
    public string biomeName;
    public int terrainHeight, offset;
    public float terrainScale, scale;

    public ushort surfaceBlock, subSurfaceBlock;

    [Header("Major Flora")]
    public int majorFloraindex;
    public float majorFloraZoneScale = 1.3f;

    [Range(0.1f, 1f)]
    public float majorFloraZoneThreshold = 0.6f;

    public float majorFloraPlacementScale = 15f;

    [Range(0.1f, 1f)]
    public float majorFloraPlacementThreshold = 0.8f;
    public bool placeMajorFlora = true;

    public int maxHeight = 12, minHeight = 5;

    public Lode[] lodes;
}


// lodes                             >> ���� ���ϵ��� ���� ����
// nodeName                     >> �̸�
// blockID                          >> ������ ����� �̸�
// minHeight / maxHeight   >> ������ �����Ǵ� �ּ� / �ִ� ����
// scale                              >> ���� ���� ���� �������� �����ϰ� ������
// threshold                       >> ������(�ش簪���� ������ ���� �ƴ� ���� ���� �� �̷������� ����� ����)
// noiseOffest                     >> ���� ���� ���� �������� �����°� ����
[System.Serializable]
public class Lode
{
    public string nodeName;
    public byte blockID;
    public int minHeight, maxHeight;
    public float scale, threshold, noiseOffest;

}
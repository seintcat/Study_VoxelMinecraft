using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BiomeAttribute
// ScriptableObject (일종의 데이터 컨테이너)
// 
// 자료형=========================================================================================
// biomeName                            >> 지형의 이름
// terrainHeight                          >> 랜덤 지형 생성의 상한을 정함
// terrainScale                            >> 지형의 굴곡의 크기를 조절하는 노이즈 스케일값
// 
// majorFloraZoneScale               >> 식물이 스폰되는 구역 크기
// majorFloraZoneThreshold         >> 식물이 스폰되는 구역 노이즈로 돌릴때 판정값
// majorFloraZoneThreshold         >> 식물 스폰지역에서의 식물 스폰 빈도
// majorFloraPlacementScale       >> 식물 스폰 빈도 판정값
// maxHeight                             >> 생성되는 블럭의 최대높이
// minHeight                              >> 생성되는 블럭의 최소높이
// placeMajorFlora                      >> 기본 식물을 생성하는지 여부
// offset                                     >> 지형 오프셋
// scale                                      >> 지형 스케일
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


// lodes                             >> 광맥 지하동굴 관련 값들
// nodeName                     >> 이름
// blockID                          >> 생성할 블록의 이름
// minHeight / maxHeight   >> 광맥이 생성되는 최소 / 최대 높이
// scale                              >> 랜덤 지형 생성 노이즈의 스케일과 동일함
// threshold                       >> 판정값(해당값보다 높으면 생성 아님 생성 안함 뭐 이런식으로 만들기 위함)
// noiseOffest                     >> 랜덤 지형 생성 노이즈의 오프셋과 동일
[System.Serializable]
public class Lode
{
    public string nodeName;
    public byte blockID;
    public int minHeight, maxHeight;
    public float scale, threshold, noiseOffest;

}
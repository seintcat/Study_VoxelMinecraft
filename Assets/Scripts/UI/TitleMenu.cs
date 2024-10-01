using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

// TitleMenu
// 메인화면을 구성하는 UI 컴포넌트
// 
// 자료형=========================================================================================
// mainMenuObject    >> 메인메뉴 화면 UI 오브젝트
// settingsObject       >> 세팅화면 UI 오브젝트
// seedFeild               >> 시드값을 적는 텍스트박스
// viewDistanceSlider >> 세팅화면의 viewDistance값을 조절하는 슬라이더
// viewDistanceText   >> viewDistanceSlider의 텍스트
// mouseSlider          >> 마우스 감도 값을 조절하는 슬라이더
// mouseText             >> mouseSlider의 텍스트
// threadingToggle     >> 청크 로딩에 스레드를 사용하는지 여부
// chunkAnimToggle   >> 청크 생성시 에니에이션 효과를 주는지 여부
// clouds                   >> 구름 옵션 드롭다운메뉴
// 
// 
// 메서드=========================================================================================
// StartGame()
// 게임 화면을 불러옴(씬 이동)
// 
// QuitGame()
// 게임 종료
// 
// EnterSettings()
// 세팅 화면 진입
// 
// LeaveSettings()
// 세팅 화면 이탈
// 
// 
public class TitleMenu : MonoBehaviour
{
    public GameObject mainMenuObject, settingsObject;

    [Header("Main Menu UI Elements")]
    public TextMeshProUGUI seedFeild;

    [Header("Settings Menu UI Elements")]
    public Slider viewDistanceSlider;
    public Slider mouseSlider;
    public TextMeshProUGUI viewDistanceText, mouseText;
    public Toggle threadingToggle, chunkAnimToggle;
    public TMP_Dropdown clouds;

    Settings settings;

    private void Awake()
    {
        if(!File.Exists(Application.dataPath + "/settings.cfg"))
        {
            Debug.Log("No settings file found, creating new one.");

            settings = new Settings();
            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);
        }
        else
        {
            Debug.Log("Settings file found, loading settings.");

            string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
        }
    }

    public void StartGame()
    {
        VoxelData.seed = Mathf.Abs(seedFeild.text.GetHashCode()) % 8000000;
        SceneManager.LoadScene("World", LoadSceneMode.Single);
    }

    public void EnterSettings()
    {
        viewDistanceSlider.value = settings.viewDistance;
        UpdateViewDistanceSlider();
        mouseSlider.value = settings.mouseSensitivity;
        UpdateMouseSlider();
        threadingToggle.isOn = settings.enableThreading;
        chunkAnimToggle.isOn = settings.enableAnimatedChunks;
        clouds.value = (int)settings.clouds;

        mainMenuObject.SetActive(false);
        settingsObject.SetActive(true);
    }

    public void LeaveSettings()
    {
        settings.viewDistance = (int)viewDistanceSlider.value;
        settings.mouseSensitivity = mouseSlider.value;
        settings.enableThreading = threadingToggle.isOn;
        settings.enableAnimatedChunks = chunkAnimToggle.isOn;
        settings.clouds = (CloudStyle)clouds.value;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

        mainMenuObject.SetActive(true);
        settingsObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateViewDistanceSlider()
    {
        viewDistanceText.text = "View Distance: " + viewDistanceSlider.value;
    }

    public void UpdateMouseSlider()
    {
        mouseText.text = "Mouse Sensitivity: " + mouseSlider.value.ToString("F1");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

// TitleMenu
// ����ȭ���� �����ϴ� UI ������Ʈ
// 
// �ڷ���=========================================================================================
// mainMenuObject    >> ���θ޴� ȭ�� UI ������Ʈ
// settingsObject       >> ����ȭ�� UI ������Ʈ
// seedFeild               >> �õ尪�� ���� �ؽ�Ʈ�ڽ�
// viewDistanceSlider >> ����ȭ���� viewDistance���� �����ϴ� �����̴�
// viewDistanceText   >> viewDistanceSlider�� �ؽ�Ʈ
// mouseSlider          >> ���콺 ���� ���� �����ϴ� �����̴�
// mouseText             >> mouseSlider�� �ؽ�Ʈ
// threadingToggle     >> ûũ �ε��� �����带 ����ϴ��� ����
// chunkAnimToggle   >> ûũ ������ ���Ͽ��̼� ȿ���� �ִ��� ����
// clouds                   >> ���� �ɼ� ��Ӵٿ�޴�
// 
// 
// �޼���=========================================================================================
// StartGame()
// ���� ȭ���� �ҷ���(�� �̵�)
// 
// QuitGame()
// ���� ����
// 
// EnterSettings()
// ���� ȭ�� ����
// 
// LeaveSettings()
// ���� ȭ�� ��Ż
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

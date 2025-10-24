using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Progress;




public class CSVImporter : EditorWindow
{
    public string csvCharacterURL 
        = "https://docs.google.com/spreadsheets/d/1hHqiJAxyAH7mlPhK_Ju-aB2Xg7pe64iD2wDnvnq6mfg/export?format=csv";
    public string csvItemURL
        = "https://docs.google.com/spreadsheets/d/1gvUeY1-aoUYnaQpVjtdAXHEJxkZ9XCemsN6IN5bIipg/export?format=csv";

     
    private string savePath;

    [MenuItem("Tools/Import Data From Google Sheets")]
    public static void ShowWindow() 
    {
        GetWindow(typeof(CSVImporter), false, "CSV Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet CSV URL", EditorStyles.boldLabel);

        csvCharacterURL = EditorGUILayout.TextField("CSV URL", csvCharacterURL);
        csvItemURL = EditorGUILayout.TextField("CSV URL", csvItemURL);

        //캐릭터 데이터
        if (GUILayout.Button("Download and Generate SO / Character"))
        {
            //에디터 잔용 코루틴
            EditorCoroutineUtility.StartCoroutineOwnerless(ImportCSVCharacter());
        }

        //아이템 데이터
        if (GUILayout.Button("Download and Generate SO / Item"))
        {
            //에디터 잔용 코루틴
            EditorCoroutineUtility.StartCoroutineOwnerless(ImportCSVItem());
        }

    }

    //csv가져와서 스크립터블 오브젝트 생성하는 코루틴
    IEnumerator ImportCSVCharacter()
    {
        savePath = $"Assets/Data/CharacterData";
        //저장경로 없으면 새로 만들기
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        //csv가져오기요첨
        UnityWebRequest www = UnityWebRequest.Get(csvCharacterURL);
        //요청보내고대기
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DownloadFail" + www.error);
            yield break;
        }

        string[] lines = www.downloadHandler.text.Split("\n");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] value = lines[i].Split(',');

            CharacterData character = ScriptableObject.CreateInstance<CharacterData>();
            character.charName = value[0];
            character.charDescription = value[1];
            character.charID = int.Parse(value[2]);
            character.charHP = int.Parse(value[3]);
            character.charJumpCount = int.Parse(value[4]);
            character.charJumpPower = float.Parse(value[5]);
            character.charMagnetRadius = float.Parse(value[6]);

            string assetPath = $"{savePath}/Character_{character.charID}_{character.charName}.asset";

            AssetDatabase.CreateAsset(character, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("캐릭터 데이터 SO 생성완료");
    }

    IEnumerator ImportCSVItem()
    {
        savePath = $"Assets/Data/ItemData";
        //저장경로 없으면 새로 만들기
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        //csv가져오기요첨
        UnityWebRequest www = UnityWebRequest.Get(csvItemURL);
        //요청보내고대기
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DownloadFail" + www.error);
            yield break;
        }

        string[] lines = www.downloadHandler.text.Split("\n");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] value = lines[i].Split(',');

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = value[0];
            item.itemDescription = value[1];
            item.itemID = int.Parse(value[2]);

            string assetPath = $"{savePath}/Item_{item.itemID}_{item.itemName}.asset";

            AssetDatabase.CreateAsset(item, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("아이템 데이터 SO 생성완료");
    }
}

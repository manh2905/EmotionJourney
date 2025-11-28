using UnityEditor;
using UnityEngine;

public class CardAssetCreator
{
    // Tạo một menu item mới trong menu "Assets"
    [MenuItem("Assets/Create/Custom Card/Create Card Data Asset")]
    public static void CreateMyAsset()
    {
        // 1. Tạo một instance mới của CardData
        CardData asset = ScriptableObject.CreateInstance<CardData>();

        // 2. Tạo một đường dẫn mặc định
        string path = "Assets/ScriptableObjects/NewCardData.asset";

        // 3. Thực hiện tạo asset file trên ổ đĩa
        AssetDatabase.CreateAsset(asset, path);

        // 4. Lưu lại database
        AssetDatabase.SaveAssets();

        // 5. Highlight file vừa tạo
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
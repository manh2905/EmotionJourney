using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Cần thêm namespace này

public class TestHarness : MonoBehaviour
{
    // Biến tĩnh (static) để theo dõi instance duy nhất của GameManager
    public static TestHarness Instance;
    
    // Tham chiếu đến BattleManager để gọi quá trình chơi
    public BattleManager battleManager;

    [Header("Test Data")]
    [Tooltip("Gán 3 CardData assets ở đây để mô phỏng 3 lá bài được chọn.")]
    // Danh sách các CardData để dùng cho test (Cần gán trong Inspector)
    public List<CardData> testCardsToUse; 
    
    // Phương thức này chạy trước Start() và được dùng cho logic Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            // Nếu chưa có instance nào, đặt instance này làm duy nhất
            Instance = this;
            // NGĂN GAME OBJECT BỊ HỦY KHI TẢI SCENE MỚI
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có instance, hủy bản sao mới này
            Destroy(gameObject);
            Debug.LogWarning("Đã có một GameManager đang tồn tại. Bản sao này đã bị hủy.");
        }
    }

    // Nút dùng để chạy test
    private void Update()
    {
        // Nhấn phím Space để bắt đầu một lượt chơi mô phỏng
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimulateTurnTest();
        }
        
        // Nhấn phím M để tải Scene Map (Giả sử bạn có Scene tên là "Map")
        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadNewScene("Map");
        }
        
        // Nhấn phím C để tải Scene Combat (Giả sử bạn có Scene tên là "Combat")
        if (Input.GetKeyDown(KeyCode.C))
        {
            LoadNewScene("Test");
        }
    }
    
    private void LoadNewScene(string sceneName)
    {
        Debug.Log($"Đang tải Scene mới: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public void SimulateTurnTest()
    {
        Debug.Log("======================================");
        Debug.Log("BẮT ĐẦU LƯỢT CHƠI MÔ PHỎNG (Nhấn Space)");
        Debug.Log("======================================");

        if (battleManager == null)
        {
            Debug.LogError("Lỗi Test: Battle Manager chưa được gán.");
            return;
        }

        if (testCardsToUse.Count < 3)
        {
            Debug.LogError("Lỗi Test: Vui lòng gán ít nhất 3 CardData vào trường 'Test Cards To Use'!");
            return;
        }

        // 1. START TURN: (Gồm Reset Stamina, Reveal/Rút bài)
        battleManager.StartPlayerTurn(); 
        
        Debug.Log("--- GIAI ĐOẠN DRAFT SIMULATION ---");

        // Giả lập việc chọn 3 lá bài theo thứ tự
        // (Lưu ý: Nếu Stamina không đủ, TrySelectCard sẽ thất bại)

        bool success1 = battleManager.draftManager.TrySelectCard(testCardsToUse[0]);
        bool success2 = battleManager.draftManager.TrySelectCard(testCardsToUse[1]);
        bool success3 = battleManager.draftManager.TrySelectCard(testCardsToUse[2]);

        if (success1 && success2 && success3)
        {
            Debug.Log("Mô phỏng: Chọn 3 lá thành công. Bắt đầu Confirm.");
            
            // 2. CONFIRM DRAFT -> Bắt đầu RESOLVE
            battleManager.draftManager.ConfirmDraft();
        }
        else
        {
            Debug.LogError("Mô phỏng: Chọn bài thất bại do Stamina hoặc giới hạn.");
        }
        
        Debug.Log("======================================");
        Debug.Log("KẾT THÚC LƯỢT MÔ PHỎNG. Kiểm tra Console.");
        Debug.Log("======================================");
    }
}
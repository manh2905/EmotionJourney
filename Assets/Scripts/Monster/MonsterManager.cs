using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance;

    public MonsterData monsterData;
    public MonsterBehaviour monsterPrefab;
    private MonsterBehaviour monsterInstance;

    public int level;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnMonster();
    }

    public void SpawnMonster()
    {
        MonsterScaler.ScaleMonster(monsterData, level);

        monsterInstance = Instantiate(monsterPrefab, transform);
        monsterInstance.data = monsterData;
    }

    

    //private void RewardPlayer()
    //{
    //    // Rơi 1–3 lá ngẫu nhiên
    //    int count = Random.Range(1, 4);

    //    for (int i = 0; i < count; i++)
    //    {
    //        var card = GetRandomRewardCard();
    //        DeckSystem.Instance.AddCard(card); // CALL DECK SYSTEM
    //    }

    //    Debug.Log("Monster defeated! Rewards added.");
    //}

    //private CardData GetRandomRewardCard()
    //{
    //    int index = Random.Range(0, monsterData.rewardCards.Count);
    //    return monsterData.rewardCards[index];
    //}
}

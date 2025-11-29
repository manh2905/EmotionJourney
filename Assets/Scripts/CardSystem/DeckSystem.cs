using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Cần dùng cho Shuffle

public class DeckSystem : MonoBehaviour
{
    // Cần phải kéo thả các CardData assets bạn đã tạo vào đây trong Inspector
    public List<CardData> masterDeckList; // Danh sách tất cả CardData gốc

    // Danh sách các lá bài trong trận đấu
    private List<CardData> currentDeck = new List<CardData>(); // Bộ bài hiện tại
    public List<CardData> hand = new List<CardData>(); // Bài trên tay (luôn tối đa 7 lá)
    private List<CardData> discardPile = new List<CardData>(); // Bài đã sử dụng

    private const int MAX_HAND_SIZE = 7; // Số lá bài tối đa trên tay [cite: 36, 71]

    void Start()
    {
        InitializeDeck();
    }

    // Khởi tạo Deck từ Master List và trộn bài lần đầu
    public void InitializeDeck()
    {
        // Copy Master List vào Current Deck
        currentDeck.Clear();
        currentDeck.AddRange(masterDeckList);
        ShuffleDeck();
    }

    // Trộn bài (sử dụng Fisher-Yates shuffle)
    public void ShuffleDeck()
    {
        // Đảm bảo bạn đã thêm 'using System.Linq;' ở trên
        // Đây là cách trộn đơn giản hơn
        currentDeck = currentDeck.OrderBy(a => Random.Range(0, 1000)).ToList();
    }

    // Xử lý vòng lặp Reveal -> Refill
    public void RevealAndRefillHand()
    {
        
        while (hand.Count < MAX_HAND_SIZE)
        {
            if (currentDeck.Count == 0)
            {
                // Nếu Deck hết bài, trộn Discard Pile thành Deck mới
                if (discardPile.Count > 0)
                {
                    currentDeck.AddRange(discardPile);
                    discardPile.Clear();
                    ShuffleDeck();
                    // Kiểm tra lại nếu vẫn không có bài sau khi trộn
                    if (currentDeck.Count == 0) break; 
                }
                else
                {
                    // Không còn bài nào để rút (cả Deck và Discard đều trống)
                    break;
                }
            }
            
            // Rút lá bài trên cùng
            CardData cardToDraw = currentDeck[0];
            currentDeck.RemoveAt(0);
            hand.Add(cardToDraw);
        }
    }

    // Chuyển 3 lá bài đã dùng vào Discard Pile
    public void DiscardUsedCards(List<CardData> usedCards)
    {
        foreach (CardData card in usedCards)
        {
            // Loại bỏ các lá bài này khỏi tay (hand) và cho vào Discard
            hand.Remove(card);
            discardPile.Add(card);
        }
        // Xử lý lá bài còn lại trên tay sau Resolve sẽ là công việc của Refill
    }
}
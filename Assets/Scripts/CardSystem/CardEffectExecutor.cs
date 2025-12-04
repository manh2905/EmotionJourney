using System.Collections.Generic;
using UnityEngine;

// Class tĩnh, chỉ chứa hàm thực thi hiệu ứng
public static class CardEffectExecutor
{
    public static void ExecuteEffects(
        List<CardData> cards,
        EmometerSystem emometer,
        StaminaSystem stamina,
        PlayerBehaviour playerStats,
        MonsterBehaviour currentMonster)
    {
        int fearRiskLeft = 0;
        int discardCard = 0;
        float totalDamage = 0f;
        float damageMultiplier = 1f; // Bored → ×0.85
        int cardIndex = 0;

        foreach (CardData card in cards)
        {
            Debug.Log($"<color=yellow>▶ Xử lý lá: {card.cardName} (slot {cardIndex + 1})</color>");

            if( discardCard != 0 )
            {
                discardCard--;
                continue;
            }
            
            if (fearRiskLeft > 0)
            {
                if (Random.value < 0.5f)
                {
                    Debug.Log($"<color=red>✖ Lá {card.cardName} bị lỗi do Sợ Hãi!</color>");
                    fearRiskLeft--;
                    cardIndex++;
                    continue;
                }
                fearRiskLeft--;
            }

            // CẬP NHẬT EMOMETER
            
            emometer.ShiftEmotion(card.emotionValue);

            bool isPositiveCard = (card.emotionType == EmotionType.Funny || card.emotionType == EmotionType.Happy);
            bool isNegativeCard = !isPositiveCard;

            bool positiveBurnout = (emometer.isBurnedOut && emometer.isPositiveBurnout && isPositiveCard);
            bool negativeBurnout = (emometer.isBurnedOut && !emometer.isPositiveBurnout && isNegativeCard);

            // Damage gốc của lá
            float dmg = card.damageValue;

           
            // HIỆU ỨNG THEO TỪNG LOẠI CẢM XÚC
           
            switch (card.emotionType)
            {
                // FUNNY — Heal +1 Emotion
                case EmotionType.Funny:
                    if (positiveBurnout)
                    {
                        Debug.Log("<color=red>Burnout +10 → Heal bị vô hiệu!</color>");
                    }
                    else
                    {
                        playerStats.Heal(playerStats.data.maxHP * 0.10f);
                        Debug.Log("<color=green>Heal 10% HP</color>");
                    }
                    break;

                // BORED — -15% tổng damage
                case EmotionType.Bored:
                    if (negativeBurnout)
                    {
                        damageMultiplier *= 0.70f; // debuff x2 = -30%
                        Debug.Log("<color=red>Bored ×2 debuff → tổng damage ×0.70</color>");
                    }
                    else
                    {
                        damageMultiplier *= 0.85f; // -15%
                        Debug.Log("<color=orange>Bored → tổng damage ×0.85</color>");
                    }
                    break;

                // SCARED — +50% dodge + rủi ro 2 lá sau
                case EmotionType.Scared:
                    {
                        if (negativeBurnout)
                        {
                            Debug.Log("<color=red>Burnout -10 → Scared debuff mạnh</color>");
                            if (Random.value < 0.5f)
                            {
                                // Né tăng 100% thay vì 50%
                                playerStats.SetDodgeChance(100f);
                                Debug.Log("<color=cyan>Scared (Burnout) → +100% Dodge</color>");
                            }
                            else
                            {
                                // Rủi ro bỏ qua luôn 2 lá sau
                                discardCard = 2;
                                Debug.Log("<color=red>Scared (Burnout) → 2 lá kế tiếp bị lỗi</color>");
                            }
                        }
                        else
                        {
                            // CHẾ ĐỘ BÌNH THƯỜNG → 50% né hoặc 50% rủi ro 2 lá
                            if (Random.value < 0.5f)
                            {
                                playerStats.SetDodgeChance(50f);
                                Debug.Log("<color=cyan>Scared → +50% Dodge</color>");
                            }
                            else
                            {
                                fearRiskLeft = 2;
                                Debug.Log("<color=red>Scared → 2 lá kế tiếp có thể bị lỗi</color>");
                            }
                        }

                        break;
                    }


                // HAPPY — 0 stamina +2 positive
                case EmotionType.Happy:
                    Debug.Log("<color=#00FFAA>Happy → không tốn stamina, +2 Emotion</color>");
                    break;

                // ANGRY — damage ×1.5 + mất 5% HP
                case EmotionType.Angry:
                    if (negativeBurnout)
                    {
                        dmg *= 1.5f;
                        float hpLoss = playerStats.data.maxHP * 0.1f;
                        playerStats.TakeDamageByAngry(hpLoss);
                        Debug.Log($"<color=red>Angry → damage ×1.5, mất {hpLoss} HP</color>");
                        break;
                    }
                    else
                    {
                        dmg *= 1.5f;
                        float hpLoss = playerStats.data.maxHP * 0.05f;
                        playerStats.TakeDamageByAngry(hpLoss);
                        Debug.Log($"<color=red>Angry → damage ×1.5, mất {hpLoss} HP</color>");
                        break;
                    }
                    
            }

            // BURNOUT GIẢM DAMAGE
            
            if (positiveBurnout || negativeBurnout)
            {
                dmg *= 0.5f;
                Debug.Log("<color=orange>⚠ Burnout → Damage lá ×0.5</color>");
            }

            // CỘNG DAME VÀO TỔNG
           
            totalDamage += dmg;

            cardIndex++;
        }

        
        float finalDamage = totalDamage * damageMultiplier;

        Debug.Log($"<color=lime> Tổng damage = ({totalDamage}) × {damageMultiplier} = {Mathf.RoundToInt(finalDamage)}</color>");

        playerStats.Attack(Mathf.RoundToInt(totalDamage));



        // Apply damage
        currentMonster.TakeDamage(Mathf.RoundToInt(totalDamage));
    }
}

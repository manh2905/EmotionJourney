using System.Collections.Generic;
using UnityEngine;

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
        float damageMultiplier = 1f;
        int cardIndex = 0;

        //MessageLogSystem.Instance.AddMessage("Bat dau xu ly " + cards.Count + " la");

        foreach (CardData card in cards)
        {
            //MessageLogSystem.Instance.AddMessage("Xu ly la: " + card.cardName);

            // Lá bị bỏ do hiệu ứng Scared
            if (discardCard == 2)
            {
                MessageLogSystem.Instance.AddMessage("Hai lá sau bị loại do Sợ hãi");               
            }
            if (discardCard != 0)
            {
                discardCard--;
                continue;
            }

            // Scared gây lỗi ngẫu nhiên
            if (fearRiskLeft > 0)
            {
                if (Random.value < 0.5f)
                {
                    MessageLogSystem.Instance.AddMessage("Lá " + card.cardName + " bị lỗi do Sợ Hãi");
                    fearRiskLeft--;
                    cardIndex++;
                    continue;
                }
                fearRiskLeft--;
            }

            // Cập nhật emotion
            emometer.ShiftEmotion(card.emotionValue);

            bool isPositiveCard = (card.emotionType == EmotionType.Funny || card.emotionType == EmotionType.Happy);
            bool isNegativeCard = !isPositiveCard;

            bool positiveBurnout = (emometer.isBurnedOut && emometer.isPositiveBurnout && isPositiveCard);
            bool negativeBurnout = (emometer.isBurnedOut && !emometer.isPositiveBurnout && isNegativeCard);

            float dmg = card.damageValue;

            // Xử lý hiệu ứng emotion
            switch (card.emotionType)
            {
                case EmotionType.Funny:
                    if (positiveBurnout)
                    {
                        //MessageLogSystem.Instance.AddMessage("Vui vẻ bị burnout, heal bị vô hiệu");
                    }
                    else
                    {
                        playerStats.Heal(playerStats.data.maxHP * 0.10f);
                        //MessageLogSystem.Instance.AddMessage("Vui vẻ: heal 10% HP");
                    }
                    break;

                case EmotionType.Bored:
                    if (negativeBurnout)
                    {
                        damageMultiplier *= 0.70f;
                        //MessageLogSystem.Instance.AddMessage("Buồn bã bị burnout giảm 30% dame");
                    }
                    else
                    {
                        damageMultiplier *= 0.85f;
                        //MessageLogSystem.Instance.AddMessage("Buồn bã giảm 15% dame");
                    }
                    break;

                case EmotionType.Scared:
                    if (negativeBurnout)
                    {
                        if (Random.value < 0.5f)
                        {
                            playerStats.SetDodgeChance(100f);
                            MessageLogSystem.Instance.AddMessage("Sợ hãi tăng 100% né đòn");
                        }
                        else
                        {
                            discardCard = 2;
                            //MessageLogSystem.Instance.AddMessage("Sợ hãi 2 lá sau bị loại");
                        }
                    }
                    else
                    {
                        if (Random.value < 0.5f)
                        {
                            playerStats.SetDodgeChance(50f);
                            MessageLogSystem.Instance.AddMessage("Sợ hãi tăng 50% né đòn");
                        }
                        else
                        {
                            fearRiskLeft = 2;
                            //MessageLogSystem.Instance.AddMessage("Sợ hãi 2 lá sau có khả năng lỗi");
                        }
                    }
                    break;

                case EmotionType.Happy:
                    //MessageLogSystem.Instance.AddMessage("Happy: khong ton stamina");
                    break;

                case EmotionType.Angry:
                    float hpLoss = playerStats.data.maxHP * (negativeBurnout ? 0.10f : 0.05f);
                    dmg *= 1.5f;
                    playerStats.TakeDamageByAngry(hpLoss);
                    //MessageLogSystem.Instance.AddMessage("Angry: damage x1.5, mat " + hpLoss + " HP");
                    break;
            }

            if (positiveBurnout || negativeBurnout)
            {
                dmg *= 0.5f;
                //MessageLogSystem.Instance.AddMessage("Burnout: damage la x0.5");
            }

            totalDamage += dmg;
            cardIndex++;
        }

        float finalDamage = totalDamage * damageMultiplier;

        MessageLogSystem.Instance.AddMessage("Tổng dame gây ra: " + Mathf.RoundToInt(finalDamage));

        playerStats.Attack(Mathf.RoundToInt(finalDamage));
        currentMonster.TakeDamage(Mathf.RoundToInt(finalDamage));
    }
}

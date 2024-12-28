using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PartsCounter : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text_partsNum;
    [SerializeField] private GameObject startButton;
    private int entire_parts_count = 0;
    private int jet_parts_count = 0;
    private bool is_game_started = false;

    private Player player;

    private void Start()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (!is_game_started)
        {
            JetPartsCount();
        }
        else
        {
            PartsCount();
            if (entire_parts_count == 1)
            {
                Debug.Log("Game Over");
                is_game_started = false;
            }
        }
        
        
    }

    public void PartsCount()
    {
        entire_parts_count = player.PartsList.Count;
        text_partsNum.text = $"残りパーツ¥n" +
                             $"{entire_parts_count}個";
    }
    
    public void JetPartsCount()
    {
        jet_parts_count = player.PartsList.OfType<Part_Power>().Count();
        if (!is_game_started && jet_parts_count == 0)
        {
            Debug.Log("Jetを1つ以上つけてください");
        }
        else if (!is_game_started && jet_parts_count > 0)
        {
            startButton.SetActive(true);
        }
    }
}

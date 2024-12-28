using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PartsCounter : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject text_jet_warning;
    [SerializeField] private EnemySpawner enemySpawner;
    private TextMeshProUGUI text_partsNum;
    private int entire_parts_count = 0;
    private int jet_parts_count = 0;
    private bool is_game_started = false;

    private Player player;

    private void Start()
    {
        player = GameManager.instance.player;
        text_partsNum = this.gameObject.GetComponent<TextMeshProUGUI>();
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
            if (entire_parts_count - 1 == 0)
            {
                Debug.Log("Game Over");
                is_game_started = false;
            }
        }
        
        
    }

    public void PartsCount()
    {
        entire_parts_count = player.PartsList.Count;
        text_partsNum.text = $"残りパーツ" +
                             $"{entire_parts_count - 1}個";
    }
    
    public void JetPartsCount()
    {
        jet_parts_count = player.PartsList.OfType<Part_Power>().Count();
        if (!is_game_started && jet_parts_count == 0)
        {
            text_jet_warning.SetActive(true);
        }
        else if (!is_game_started && jet_parts_count > 0)
        {
            text_jet_warning.SetActive(false);
            startButton.SetActive(true);
        }
    }

    public void PushGameStartButton()
    {
        is_game_started = true;
        startButton.SetActive(false);
        enemySpawner.StartWave();
    }
}

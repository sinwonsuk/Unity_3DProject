using Fusion;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class SurvivorPlayerCountUI : NetworkBehaviour
{

    public TMP_Text survivorText;

    void OnEnable()
    {
        EventBus<SurvivorPlayerCount>.OnEvent += UpdateUI;
    }

    void OnDisable()
    {
        EventBus<SurvivorPlayerCount>.OnEvent -= UpdateUI;
    }

    void UpdateUI(SurvivorPlayerCount evt)
    {

        survivorText.text = ($"{evt.pCount}");
    }
}

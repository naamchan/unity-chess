using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionButton : MonoBehaviour
{
    [SerializeField]
    private ChessPieceType spawnType;

    // Use this for initialization
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnPromotionClicked);
    }

    private void OnPromotionClicked()
    {
        GameplayManager.Instance.Promote(GameplayManager.Instance.GetPromotedPiece(), spawnType);
    }
}

using System;
using UnityEngine;

public class Item : MonoBehaviour {
    [SerializeField] private GameObject interactIndicator;
    Player player;
    private void Start() {
        player = Player.Instance;
    }

    private void Update() {
        if (player.forwardTransform == transform && !interactIndicator.active) {
            ShowInteractIndicator();
        } else if (player.forwardTransform != transform && interactIndicator.active) {
            HideInteractIndicator();
        }
    }

    public void ShowInteractIndicator() {
        interactIndicator.SetActive(true);
    }
    public void HideInteractIndicator() {
        interactIndicator.SetActive(false);
    }
}

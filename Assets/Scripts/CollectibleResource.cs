using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleResource : MonoBehaviour
{
    [SerializeField]
    private CollectibleResourceSO collectibleResource;
    [SerializeField]
    private int currentAmount;
    [SerializeField]
    private InventorySO inventory;
    [SerializeField]
    private Text resourceAmountText;

    void Start()
    {
        currentAmount = collectibleResource.maxAmount;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickaxe") && currentAmount > 0)
        {
            var damage = 5;

            if (currentAmount < damage) damage = currentAmount;

            currentAmount -= damage;

            inventory.resourceAmount += damage;
            resourceAmountText.text = inventory.resourceAmount.ToString();

            if (currentAmount <= 0) Destroy(gameObject);
        }
    }
}

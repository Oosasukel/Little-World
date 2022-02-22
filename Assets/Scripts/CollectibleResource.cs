using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleResource : MonoBehaviour
{
    [SerializeField]
    private CollectibleResourceSO collectibleResource;
    [SerializeField]
    private int currentAmount;

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
            Debug.Log($"+{damage}");

            if (currentAmount <= 0) Destroy(gameObject);
        }
    }
}

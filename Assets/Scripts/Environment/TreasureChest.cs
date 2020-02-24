using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class DropProbability
{
    public GameObject ObjectToDrop;
    /// <summary>
    /// Weight influences the probability of the drop. If one object has weight 6 and second weight 2, the first one will be 3x as likely to drop.
    /// </summary>
    public int Weight;
}

class TreasureChest : MonoBehaviour
{
    public DropProbability[] DropTable;
    public Sprite OpenedSprite;
    public Sprite ClosedSprite;
    GameObject droppedPowerup;
    bool isOpened;

    private void Awake()
    {
        GetComponent<InteractableObject>().OnInteractionTriggered += OnChestClicked;
        UpdateSprite();
    }

    private void OnChestClicked(object chest, Hero openingHero)
    {
        if (!isOpened)
        {
            isOpened = true;
            UpdateSprite();
            SpawnDrop();
        }
        else if (droppedPowerup != null)
        {
            // The chest is opened, and powerup not applied - apply it.
            droppedPowerup.GetComponent<PowerUp>().ApplyPowerup(openingHero);
            Destroy(droppedPowerup);
        }
    }

    void SpawnDrop()
    {
        var random = new System.Random();
        var totalProbability = DropTable.Sum(dropDefinition => dropDefinition.Weight);
        var itemToDrop = random.Next(totalProbability);
        // We simulate each element being in the array Weight times.
        var itemToDropIndex = 0;
        while (itemToDrop >= DropTable[itemToDropIndex].Weight)
        {
            itemToDrop -= DropTable[itemToDropIndex].Weight;
            itemToDropIndex++;
        }
        SpawnDrop(DropTable[itemToDropIndex].ObjectToDrop);
    }

    void SpawnDrop(GameObject toSpawn)
    {
        droppedPowerup = Instantiate(toSpawn, transform, false);
    }

    void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = isOpened ? OpenedSprite : ClosedSprite;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class InventoryCtr
{
    public InventoryCtr(Player player)
    {
          this.player = player;
          items = new List<BuildableData>();
          itemHotKeySlots = new Dictionary<int, BuildableData>();
          maxWeight = 0;
          maxVolume = 0;
    }
   public List<BuildableData> items;
   public Dictionary<int,BuildableData> itemHotKeySlots;
   public float maxWeight;
   public float maxVolume;

   private Player player;

    public float currWeight
    {
         get
         {
              float weight = 0;
              foreach (var item in items)
              {
                weight += item.weight;
              }
              return weight;
         }
    }
    public float currVolume
    {
         get
         {
              float volume = 0;
              foreach (var item in items)
              {
                volume += item.volume;
              }
              return volume;
         }
    }

    public bool CanAddItem(BuildableData item)
    {
          if (currWeight + item.weight < maxWeight && currVolume + item.volume < maxVolume)
          {
              return true;
          }
          else if (currWeight + item.weight >= maxWeight)
          {
               Debug.Log("Inventory is too much heavy, cannot add item: " + item.name + ", weight: " + item.weight + ", current weight: " + currWeight);
               return false;
          }
          else if (currVolume + item.volume >= maxVolume)
          {
               Debug.Log("Inventory is full, cannot add item: " + item.name + ", volume: " + item.volume + ", current volume: " + currVolume);
               return false;
          }
          return false;
    }

    private void AddItem(BuildableData item)
    {
          if(CanAddItem(item))
          {
               items.Add(item);
          }
          else
          {
               Debug.Log("Cannot add item: " + item.name + ", weight: " + item.weight + ", current weight: " + currWeight);
          }
    }
    public void PickUpItem(Entity itemEntity)
    {
          BuildableData item = itemEntity.dataDef;
          if (item != null && ( item is ItemData || typeof(ItemData).IsAssignableFrom(itemEntity.dataDef.GetType())))
          {
               if (currWeight + item.weight < maxWeight && currVolume + item.volume < maxVolume)
               {
                    AddItem(item);
                    itemEntity.DeSpawm();
               }
               else if (currWeight + item.weight >= maxWeight)
               {
                    Debug.Log("Inventory is too much heavy, cannot add item: " + item.name + ", weight: " + item.weight + ", current weight: " + currWeight);
               }
               else if (currVolume + item.volume >= maxVolume)
               {
                    Debug.Log("Inventory is full, cannot add item: " + item.name + ", volume: " + item.volume + ", current volume: " + currVolume);
               }
          }
    }
     public void DropItem(BuildableData item)
     {
          if (items.Contains(item))
          {
               items.Remove(item);

               GenSpawn.Spawn(item, player.mousePlayerCtr.mousePos, Quaternion.identity);
          }
          else
          {
               Debug.Log("Item not found in inventory: " + item.name);
          }
     }
}

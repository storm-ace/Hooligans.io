using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Coins : MonoBehaviour
{
    [FormerlySerializedAs("spawn")] [HideInInspector] public MapSpawner mapSpawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            collision.GetComponent<Player>().Coins++;
            collision.GetComponent<Player>().UpdateCoins();
            mapSpawner.activeCoins.Remove(gameObject);
            mapSpawner.idleCoins.Add(gameObject);
            gameObject.SetActive(false);
        }
        else if (collision.GetComponent<SoldierUnit>())
        {
            if (collision.GetComponent<SoldierUnit>().IsAI)
            {
                collision.GetComponent<SoldierUnit>().ai.Coins++;
                mapSpawner.activeCoins.Remove(gameObject);
                mapSpawner.idleCoins.Add(gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                collision.GetComponent<SoldierUnit>().player.Coins++;
                collision.GetComponent<SoldierUnit>().player.UpdateCoins();
                mapSpawner.activeCoins.Remove(gameObject);
                mapSpawner.idleCoins.Add(gameObject);
                gameObject.SetActive(false);
            }
        }
        else if (collision.GetComponent<AI>())
        {
            var ai = collision.GetComponent<AI>();
            ai.TargetCoin = null;
            ai.Coins++;
            mapSpawner.activeCoins.Remove(gameObject);
            mapSpawner.idleCoins.Add(gameObject);
            gameObject.SetActive(false);
        }
    }
}

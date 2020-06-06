using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthHandler : MonoBehaviour
{
    public int totalLives = 3;
    public int currentLives;
    [SerializeField] float timeInvincible = 5f;
    private float currentTime;

    private LifeEnum lifeState = LifeEnum.HEALTHY;
    public bool recentlyHit = false;

    // Start is called before the first frame update
    void Start()
    {
        currentLives = totalLives;
        currentTime = timeInvincible;
    }

    // Update is called once per frame
    void Update()
    {
        HandleLifeState();
        HandleStateBehavior();
        HandleInvincibilityTime();

    }

    private void HandleInvincibilityTime()
    {
        if (recentlyHit)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = timeInvincible;
                recentlyHit = false;
            }
        }
    }

    private void HandleStateBehavior()
    {
        //Act accordingly to player life State
        //Currently only kills the player
        switch (lifeState)
        {
            case LifeEnum.DEAD:
                KillPlayer();
                break;
            case LifeEnum.LOWLIFE:
                break;
            case LifeEnum.MIDLIFE:
                break;
            default:
                break;
        }
    }

    private void KillPlayer()
    {

        Debug.Log("You died :(");
    }

    private void HandleLifeState()
    {
        //State of the player based on their current life amount
        if (currentLives <= 0)
        {
            lifeState = LifeEnum.DEAD;
        }
        else if (currentLives <= totalLives / 3)
        {
            lifeState = LifeEnum.LOWLIFE;
        }
        else if (currentLives <= totalLives / 2)
        {
            lifeState = LifeEnum.MIDLIFE;
        }
        else
        {
            lifeState = LifeEnum.HEALTHY;
        }
    }

    public void takeDamage(int damage)
    {
        
        currentLives -= damage;
        Debug.Log("Current health: " + currentLives);
    }
}

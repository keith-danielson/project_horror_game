using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEditorInternal;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("General")]
    [SerializeField] float health = 50f;
    [SerializeField] float regenerationFactor = 10f;
    [SerializeField] float damage = 0.5f;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed = 4f;
    [SerializeField] float slowedSpeed = 2f;
    [SerializeField] float slowedMostSpeed = 1f;


    [Header("Combat Settings")]
    [SerializeField] float attackRange = 5f;
    [SerializeField] float sneakRange = 200f;
    [SerializeField] float pursueRange = 50f;
    [SerializeField] int damagePerAttack = 1;
    [SerializeField] float attackRate = 5f;


    public enum CombatEnum { PURSUE, FLEE, ATTACK, FREEZE, SNEAK };

    private float brightnessOnSprite;
    private float currentHealth;
    private float lightDamageFactor;
    private float damageTakenPerFrame;
    private LightEffector lightEffector;
    private AIPath enemyPathSettings;
    private LifeEnum lifeState = LifeEnum.HEALTHY;
    private PlayerHealthHandler mainPlayerHealthHandler = null;
    private CombatEnum combatState = CombatEnum.SNEAK;
    private bool rechargedAttack = true;
    private float currentRechargeAttackTime;

    
    

    void Start()
    {
        currentHealth = health;
        lightEffector = GetComponent<LightEffector>();
        enemyPathSettings = GetComponent<AIPath>();
        mainPlayerHealthHandler = GameObject.Find("Player").GetComponent<PlayerHealthHandler>();
        currentRechargeAttackTime = attackRate;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentBrightness();
        TakeDamage();
        HandleSpeed();
        SetCombatState();
        HandleCombatStateBehavior();
        SetLifeState();
        HandleLifeStateBehavior();
        RechargeAttack();
    }

    private void RechargeAttack()
    {
        if (!rechargedAttack)
        {
            currentRechargeAttackTime -= Time.deltaTime;

            if (currentRechargeAttackTime <= 0)
            {
                currentRechargeAttackTime = attackRate;
                rechargedAttack = true;
            }
        }
    }

    private void HandleCombatStateBehavior()
    {
        switch (combatState)
        {
        
            case CombatEnum.ATTACK:
               
                mainPlayerHealthHandler.takeDamage(damagePerAttack);
                rechargedAttack = false;
                break;
            case CombatEnum.FLEE:
                break;
            case CombatEnum.FREEZE:
                break;
            case CombatEnum.PURSUE:
                break;
            case CombatEnum.SNEAK:
                break;
            default:
                break;
        }
    }

    private void SetCombatState()
    {
        //Currently only attacks, pursues, and sneaks
        float distanceFromPlayer = enemyPathSettings.remainingDistance;
        if (distanceFromPlayer <= attackRange && rechargedAttack)
        {
            combatState = CombatEnum.ATTACK;
        }
        else if (distanceFromPlayer <= pursueRange)
        {
            combatState = CombatEnum.PURSUE;
        }
        else if (distanceFromPlayer <= sneakRange)
        {
            combatState = CombatEnum.SNEAK;
        }
        //Debug.Log(combatState + "Distance from player: " + distanceFromPlayer);
    }

    private void HandleSpeed()
    {
        //Handle movement speed when in light
        if (brightnessOnSprite > 0.6)
        {
            enemyPathSettings.maxSpeed = slowedMostSpeed;
        }
        else if (brightnessOnSprite > 0.2)
        {
            enemyPathSettings.maxSpeed = slowedSpeed;
        }
        else
        {
            enemyPathSettings.maxSpeed = baseSpeed;
        }
    }

    private void GetCurrentBrightness()
    {
        //Getting brightness level from LightEffector
        brightnessOnSprite = lightEffector.brightness;
        lightDamageFactor = Mathf.Pow(brightnessOnSprite, 10) + 1 * 10;
    }

    private void SetLifeState()
    {
        //Set lifeState according to current health
        if (currentHealth <= 0f)
        {
            lifeState = LifeEnum.DEAD;
        }
        else if (currentHealth <= health / 3)
        {
            lifeState = LifeEnum.LOWLIFE;
        }
        else if (currentHealth <= health / 2)
        {
            lifeState = LifeEnum.MIDLIFE;
        }
        else
        {
            lifeState = LifeEnum.HEALTHY;
        }
        //debugging tool
        //Debug.Log(lifeState + " Health = " + currentHealth + " Light = " + brightnessOnSprite + " DTPF = " + damageTakenPerFrame);
    }

    private void HandleLifeStateBehavior()
    {
        switch (lifeState)
        {
            case LifeEnum.DEAD:
                Destroy(gameObject);
                break;
            case LifeEnum.LOWLIFE:
                break;
            case LifeEnum.MIDLIFE:
                break;
            default:
                break;
        }
    }

    private void TakeDamage()
    {
        //Do damage to enemy based on brightness
        if (brightnessOnSprite > 0.2)
        {
            damageTakenPerFrame = damage * lightDamageFactor * Time.deltaTime;
            currentHealth -= damageTakenPerFrame;
        }
        else
        {
            damageTakenPerFrame = 0;
            currentHealth = Mathf.Clamp(currentHealth + regenerationFactor * Time.deltaTime, 0, health);
        }
    }

   
}

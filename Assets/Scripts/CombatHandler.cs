
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
    public LayerMask obstacleMask;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed = 4f;
    [SerializeField] float slowedSpeed = 2f;
    [SerializeField] float slowedMostSpeed = 1f;
    [SerializeField] float chaseViewAngle = 100f;
    [SerializeField] float sneakViewAngle = 180f;
    [SerializeField] float teleportDistance = 20f;
    [SerializeField] float timeBetweenTeleports = 10f;




    [Header("Combat Settings")]
    [SerializeField] float attackRange = 5f;
    [SerializeField] float sneakRange = 200f;
    [SerializeField] float pursueRange = 50f;
    [SerializeField] int damagePerAttack = 1;
    [SerializeField] float attackRate = 5f;
    [SerializeField] bool canTakeDamage = true;





    public enum CombatEnum { PURSUE, FLEE, ATTACK, FREEZE, SNEAK };

    private float brightnessOnSprite;
    private float currentHealth;
    private float lightDamageFactor;
    private float damageTakenPerFrame;
    private bool rechargedAttack = true;
    private float currentRechargeAttackTime;

    private LightEffector lightEffector;
    private PlayerHealthHandler mainPlayerHealthHandler = null;
    private Transform mainPlayerPosition = null;
    
    
    private AIPath path;
    private Transform targetPosition;
    private AIDestinationSetter destinationSetter;
    private Transform currentPosition;
    private EnemyFieldOfView fov;
    private bool canSeePlayer = false;
    private float currentChaseTime;
    private bool teleportCharged = true;
    private float currentTeleportTime;
    private Vector3 teleportPosition;


    private LifeEnum lifeState = LifeEnum.HEALTHY;
    private CombatEnum combatState = CombatEnum.SNEAK;
    

    void Start()
    {
        currentHealth = health;
        lightEffector = GetComponent<LightEffector>();
        path = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        mainPlayerHealthHandler = GameObject.Find("Player").GetComponent<PlayerHealthHandler>();
        mainPlayerPosition = GameObject.Find("Player").GetComponent<Transform>();
        currentRechargeAttackTime = attackRate;
        currentPosition = GetComponent<Transform>();
        fov = GetComponent<EnemyFieldOfView>();
        currentTeleportTime = timeBetweenTeleports;

        

        
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
        ChangeTarget();

        
    }

    private void ChangeTarget()
    {
        destinationSetter.target = targetPosition;

        
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
        if (!teleportCharged)
        {
            currentTeleportTime -= Time.deltaTime;
            if (currentTeleportTime <= 0)
            {
                currentTeleportTime = timeBetweenTeleports;
                teleportCharged = true;
            }
        }
    }

    private void HandleCombatStateBehavior()
    {



        switch (combatState)
        {

        
            case CombatEnum.ATTACK:
                
                mainPlayerHealthHandler.takeDamage(damagePerAttack);
                
                break;
            case CombatEnum.FLEE:
                break;
            case CombatEnum.FREEZE:
                targetPosition = null;
                break;
            case CombatEnum.PURSUE:
                fov.viewAngle = chaseViewAngle;
                targetPosition = mainPlayerPosition;

                

                break;
            case CombatEnum.SNEAK:
                //Sneak logic here
                fov.viewAngle = sneakViewAngle;
                if (teleportCharged)
                {
                    bool shouldTeleport = false;
                    targetPosition = null;
                    do
                    {
                        
                        teleportPosition = mainPlayerPosition.position - (getRandomVector() * teleportDistance);

                        float dstToTarget = Vector3.Distance(teleportPosition, mainPlayerPosition.position);
                        Vector3 dirToTarget = (teleportPosition - mainPlayerPosition.position).normalized;
                        



                        if (!Physics2D.Raycast(mainPlayerPosition.position, dirToTarget, dstToTarget, obstacleMask))
                        {
                            print(Vector3.Distance(teleportPosition, mainPlayerPosition.position));
                            
                            shouldTeleport = true;
                        }

                        

                    } while (!shouldTeleport);


                    this.transform.position = teleportPosition;
                    teleportCharged = false;
                }
                
                //if (mainPlayerPosition.)
                //this.transform.position = 





                
                


                break;
            default:
                break;
        }
    }

    private Vector3 getRandomVector()
    {
        float x = (float)Random.Range(0, 100) / 100;
        float y = (float)Random.Range(0, 100) / 100;
        float z = 0;
        /*
        int makeNegative = Random.Range(0, 4);
        if (makeNegative < 2)
        {
            x = x * -1;
        }
        if (makeNegative < 2)
        {
            y = y * -1;
        }
        Debug.Log("xyz: " + x + y + z);
        */
        return new Vector3(x, y, z);
    }

    private void SetCombatState()
    {
        //Currently only attacks, pursues, and sneaks
        float distanceFromPlayer = Vector3.Distance(currentPosition.position, mainPlayerPosition.position);
        canSeePlayer = fov.canSeeTarget;
        if (distanceFromPlayer <= attackRange && rechargedAttack && canSeePlayer)
        {
            combatState = CombatEnum.ATTACK;
            rechargedAttack = false;
        }
        else if (distanceFromPlayer <= pursueRange && canSeePlayer)
        {
            combatState = CombatEnum.PURSUE;
        }
        else if (distanceFromPlayer <= sneakRange && canSeePlayer)
        {
            combatState = CombatEnum.SNEAK;
        }
        //Debug.Log(combatState + " Distance from player: " + distanceFromPlayer);
    }

    private void HandleSpeed()
    {
        //Handle movement speed when in light
        if (brightnessOnSprite > 0.6)
        {
            path.maxSpeed = slowedMostSpeed;
        }
        else if (brightnessOnSprite > 0.2)
        {
            path.maxSpeed = slowedSpeed;
        }
        else
        {
            path.maxSpeed = baseSpeed;
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
        if (canTakeDamage)
        {
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

   
}

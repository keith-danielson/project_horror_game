using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float health = 50f;
    [SerializeField] float regenerationFactor = 10f;
    [SerializeField] float regenderationTime = 5f;
    [SerializeField] float damage = 0.5f;
    

    private float brightnessOnSprite;
    private float currentHealth;
    private float lightDamageFactor;
    private float damageTakenPerFrame;
    private LightEffector lightEffector;
    

    void Start()
    {
       
        currentHealth = health;
        lightEffector = GetComponent<LightEffector>();
    }

    // Update is called once per frame
    void Update()
    {
        //Getting brightness level from LightEffector
        brightnessOnSprite = lightEffector.brightness;
        lightDamageFactor = Mathf.Pow(brightnessOnSprite, 10) + 1 * 10;

        //Kill object
        if (currentHealth <= 0f)
        {
            print("Dead!");
            Destroy(gameObject);
            return;
        }

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

        print("Alive! Health = " + currentHealth + " Light = " + brightnessOnSprite + " DTPF = " + damageTakenPerFrame);


    }
}

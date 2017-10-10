﻿/*This script was written by James | Last edited by James | Modified on September 28, 2017
 *This script piggybacks off of the Basic AI script but with extra functionality.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedAI : BasicAI
{
    [Header("Advanced stuff")]
    [Tooltip("Used to keep track of how many attacks have been done in a chain.")]
    public int current_attack_chain;
    [Tooltip("The cooldown timer after the AI has finished his 3 chain combo.")]
    public float attack_chain_cooldown_time = 1f;
    [Tooltip("This is the cooldown inbetween the chain attacks.")]
    public float original_attack_chain_cd_time = 0.2f;
    [Tooltip("The least amount of damage before the AI gets staggered.")]
    public int stagger_threshold = 20;
    [Tooltip("The sum of received damage. When this number surpasses the stagger threshold, it will stagger the AI.")]
    public int total_damage_received = 0;
    private bool reset_time = false;

    protected IEnumerator Dodge() //this is the dodge of the AI, it will randomly choose left or right dodges.
    {
        alerted = false; //set alert to false to make the AI stop chasing the player to allow dodging.
        current_attack_chain = 0;
        cooldown_action = original_attack_chain_cd_time;
        dodge = (dodge_direction)Random.Range(0, 2); //randomly picks a direction to dodge. 0 is left, 1 is right.
        yield return new WaitForSeconds(dodge_time);
        alerted = true; //set alert back to true to make the AI chase the player again.
        dodge = dodge_direction.not_dodging; //reset the dodge's direction to no direction otherwise known as not_dodging.
        yield return new WaitForSeconds(cooldown_action);
        performing_action = false;  //the AI is done with the action
    }


    protected IEnumerator Attack_1() //this is the right swing attack. starts from the left arm position
    {
        //move towards the player while the attack goes through.
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 5f;
        //cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
        //move for half a second, then attack
        yield return new WaitForSeconds(0.4f);
        attack_script.check_attack_left = true;
        current_attack_chain++;
        if (current_attack_chain >= 3)
        {
            cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
            current_attack_chain = 0;
            reset_time = true;
        }
        else
        {
            cooldown_action = original_attack_chain_cd_time;
        }
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 8f;
        yield return new WaitForSeconds(performing_time);
        //waits for performing time to be done... then add more things here if needed for after action
        yield return new WaitForSeconds(cooldown_action);
        //this is the time to wait for the next action to be performed.
        performing_action = false;  //the AI is done with the action
    }

    protected IEnumerator Attack_2() //this is the right swing attack. starts from the left arm position
    {
        //move towards the player while the attack goes through.
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 5f;
        //cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
        //move for half a second, then attack
        yield return new WaitForSeconds(0.4f);
        attack_script.check_attack_right = true;
        current_attack_chain++;
        if (current_attack_chain >= 3)
        {
            cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
            current_attack_chain = 0;
            reset_time = true;
        }
        else
        {
            cooldown_action = original_attack_chain_cd_time;
        }
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 8f;
        yield return new WaitForSeconds(performing_time);
        //waits for performing time to be done... then add more things here if needed for after action
        yield return new WaitForSeconds(cooldown_action);
        //this is the time to wait for the next action to be performed.
        performing_action = false;  //the AI is done with the action
    }

    protected IEnumerator Attack_3() //this is the diagonal swing attack. stops from the top right arm position 
    {
        //move towards the player while the attack goes through.
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 5f;
        //cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
        //move for half a second, then attack
        yield return new WaitForSeconds(0.4f);
        attack_script.check_attack_top = true;
        current_attack_chain++;
        if (current_attack_chain >= 3)
        {
            cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
            current_attack_chain = 0;
            reset_time = true;
        }
        else
        {
            cooldown_action = original_attack_chain_cd_time;
        }
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 8f;
        yield return new WaitForSeconds(performing_time);
        //waits for performing time to be done... then add more things here if needed for after action
        yield return new WaitForSeconds(cooldown_action);
        //this is the time to wait for the next action to be performed.
        performing_action = false;  //the AI is done with the action
    }

    protected IEnumerator MoveBack()
    {
        //set alert to false to make the AI stop chasing the player to allow moving backwards and set nav mesh speed/acceleration to 0 to stop the AI completely.
        alerted = false;
        current_attack_chain = 0;
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().acceleration = 0;
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity = Vector3.zero;
        cooldown_action = Random.Range(cooldown_action_min, cooldown_action_max);
        //start moving backwards
        getting_knockback = true;
        yield return new WaitForSeconds(.3f);
        getting_knockback = false;
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = 8f;
        alerted = true; //set alert back to true to make the AI chase the player again.
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().acceleration = acceleration;
        transform.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = movement_speed;
        yield return new WaitForSeconds(cooldown_action);
        performing_action = false;  //the AI is done with the action
    }

    public IEnumerator DamageEnemy(int incoming_damage) //first will apply damage, and then stagger the enemy for a certain duration
    {

        //apply damage and checks if the enemy dies from the damage.
        enemy_health -= incoming_damage;
        total_damage_received += incoming_damage;
        if(total_damage_received >= stagger_threshold)
        {
            //reset the total damage received so it doesn't get stunned repeatedly afterwards being hit.
            total_damage_received = 0;
            //set the bools to allow knockback and prevent actions/movements.
            getting_knockback = true;
            //attack script's staggered is set to true to uninstantiate any attacks that are already created or about to be instantiated.
            attack_script.staggered = true;
            //change the AI state to staggered for animations.
            current_state = ai_state.staggered;
            //set staggering to true to affect fixedupdate to prevent the ai from doing any actions.
            staggering = true;
            //set alerted to false to stop the AI from following the player with the navmesh.
            alerted = false;
            //stop the ai from moving by setting the acceleration, speed, and velocity to 0.
            transform.GetComponent<UnityEngine.AI.NavMeshAgent>().acceleration = 0;
            transform.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
            transform.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity = Vector3.zero;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
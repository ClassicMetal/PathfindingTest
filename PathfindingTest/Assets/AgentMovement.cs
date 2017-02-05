using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AgentMovement : MonoBehaviour {

    public int health;
    public float cooldown;
    public int attackDmg;
    public float speed;

    public Transform target;
    private NavMeshAgent agent;

    private enum e_unitStates { stationary, moving, attackmoving};
    private e_unitStates unitState;

    private Vector3 targetPosition;
    private bool enemyTarget = false;
    private List<GameObject> enemiesInRange;

    private float timer;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        unitState = e_unitStates.stationary;
        enemiesInRange = new List<GameObject>();
        agent.speed = speed;
	}
	
	// Update is called once per frame
	void Update () {
        if (unitState != e_unitStates.moving && enemiesInRange.Count > 0)
        {
            if (enemiesInRange[0] != null)
            {
                print("attacking: " + enemiesInRange[0].name);
                timer -= Time.deltaTime;
                if (timer <= 0.0f)
                    Attack(enemiesInRange[0].GetComponent<AgentMovement>());
                //attack(enemiesInRange[0]);
            }
            else
                enemiesInRange.RemoveAt(0);
        }
        else if (unitState == e_unitStates.moving || enemiesInRange.Count == 0)
            agent.Resume();

        if (unitState != e_unitStates.stationary && agent.remainingDistance == 0)
            unitState = e_unitStates.stationary;

        if (Input.GetMouseButtonDown(1)) {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    moveCommand(hit.point, null, true);
                }
            }
            else { 
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    moveCommand(hit.point);
                }
            }
        }
	}

    void moveCommand(Vector3 targetPos, GameObject targetEnemy = null, bool attackMove = false)
    {
        target.position = targetPos;
        enemyTarget = targetEnemy;
        //set this unit's state
        if (!attackMove)
            unitState = e_unitStates.moving;
        else
            unitState = e_unitStates.attackmoving;

        agent.SetDestination(targetPos);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyUnit")
        {
            //if (unitState != e_unitStates.moving)
            
            enemiesInRange.Add(other.gameObject);
            print("Enemy Contact: " + other.name);
            
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "EnemyUnit")
        {
            //if (unitState != e_unitStates.moving)

            enemiesInRange.Remove(other.gameObject);
            print("Enemy lost: " + other.name);
        }
    }

    public void Attack(AgentMovement enemy)
    {
        transform.rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
        agent.Stop();
        GetComponent<ParticleSystem>().Play();
        timer = cooldown;
        enemy.TakeDamage(attackDmg);
    }

    public void TakeDamage(int dmgAmount)
    {
        health -= dmgAmount;
        if(health <= 0)
        {
            //Die
            Destroy(gameObject);
        }
    }
}

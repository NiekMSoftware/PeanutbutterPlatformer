using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts
{
    public class ScoutBehavior : MonoBehaviour
    {
        private enum Behavior
        {
            PATROLLING,
            DETECTING,
            DETECTED,
            ATTACKING
        }

        [SerializeField] private EnemyData enemyData;
        
        [Header("Patrol Properties")]
        [SerializeField] private Behavior enemyBehavior;
        
        [Space(5)]
        [SerializeField] private Transform[] patrolPoints;
        private int currentPatrolIndex;

        [Space(10)]
        public float TotalTime = 5f;
        private float currentTime;
        
        [Range(0.5f, 5f)]
        [SerializeField] private float distanceToPlayer;

        [Header("Detection Range Properties")]
        [Range(1f, 15f)]
        [SerializeField] private float detectionRadius;
        
        [Range(1f, 120f)]
        [SerializeField] private float viewAngle;
        [SerializeField] private LayerMask playerLayer;
        
        private readonly List<GameObject> visibleTargets = new();

        private float moveSpeed;

        void Start()
        {
            enemyBehavior = Behavior.PATROLLING;
            moveSpeed = enemyData.Speed;
            currentTime = TotalTime;
        }

        void Update()
        {
            SwitchBehavior(enemyBehavior);
        }

        private void SwitchBehavior(Behavior behavior)
        {
            switch (behavior)
            {
                case Behavior.PATROLLING:
                    Patrol();
                    break;
                
                case Behavior.DETECTING:
                    StartCoroutine(nameof(FindPlayerWithDelay), TotalTime);
                    break;
                
                case Behavior.DETECTED:
                    MoveToPlayer();
                    break;
                
                case Behavior.ATTACKING:
                    enemyData.Attack();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
            }
        }

        /// <summary>
        /// <c>Patrol</c> will make the enemy patrol around. The enemy will move to point A to B, once arrived
        /// at their point they will look around and try to detect the player. If it was successful, their state
        /// switching to Detected. If false, they return to the other point and continue the cycle. 
        /// </summary>
        private void Patrol()
        {
            if (currentTime <= 0)
                currentTime = TotalTime;
            
            // Move towards the current patrol point
            Vector3 targetPosition = patrolPoints[currentPatrolIndex].position;
            
            transform.position =
                Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            
            // If reached patrol point, start Detecting for the player.
            if (!(Vector3.Distance(transform.position, targetPosition) < 0.1f)) return;

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            enemyBehavior = Behavior.DETECTING;
        }

        private IEnumerator FindPlayerWithDelay(float delay)
        {
            while (enemyBehavior == Behavior.DETECTING)
            {
                visibleTargets.Clear();
                DetectPlayer();
                yield return new WaitForSeconds(delay);
            }
        }

        private void DetectPlayer()
        {
            currentTime -= Time.deltaTime;
            Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            
            foreach (var target in targetsInRadius)
            {
                GameObject goTarget = target.gameObject;
                Vector3 dirToTarget = (goTarget.transform.position - transform.position).normalized;

                if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)) continue;

                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                    
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, ~playerLayer))
                {
                    visibleTargets.Add(goTarget);
                }
            }
            
            if (currentTime <= 0f)
            {
                // switch back to patrolling
                Debug.Log("Player was not found.. I go back to patrolling!");
                enemyBehavior = Behavior.PATROLLING;
            }
            else if (visibleTargets.Count > 0)
            {
                // found player
                Debug.Log("Found player!");
                enemyBehavior = Behavior.DETECTED;
            }
        }

        private void MoveToPlayer()
        {
            // this can always be 0, cuz there is only one player.
            Vector3 targetPosition = visibleTargets[0].transform.position;

            // gaming.
            transform.position = 
                Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            
            // if the distance has been checked, attack!
            if (Vector3.Distance(transform.position, targetPosition) < distanceToPlayer)
                enemyBehavior = Behavior.ATTACKING;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward * detectionRadius;
            Vector3 fovLine2 = Quaternion.AngleAxis(-viewAngle / 2, transform.up) * transform.forward * detectionRadius;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
        }
    }
}
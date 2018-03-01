using System;
using UnityEngine;
using TankGame.Systems;

namespace TankGame.AI
{
    public class ShootState : AIStateBase
    {
        public float SqrShootingDistance
        {
            get { return Owner.ShootingDistance * Owner.ShootingDistance; }
        }

        public float SqrMinimumDistanceToPlayer
        {
            get { return Owner.MinimumDistanceToPlayer * Owner.MinimumDistanceToPlayer; }
        }

        /// <summary>
        /// I added the following accessor to the Weapon class, I need it for the ShootingPointRaycaster method.
        /// 
        /// public Transform ReturnShootingPoint { get { return _shootingPoint; } }
        /// </summary>
        public override void Update()
        {
            //Debug.Log("ShootState Update()");

            //if (Owner.Target == null)
            //    Debug.Log("Target is null!");

            //Debug.Log("ChangeState() == " + ChangeState());

            if (!ChangeState())
            {
                //Debug.Log("!ChangeState()");
                Owner.Mover.Turn(Owner.Target.transform.position); // Always turn towards the target.

                Vector3 toPlayerVector =
                    Owner.transform.position - Owner.Target.transform.position;
                float sqrDistanceToPlayer = toPlayerVector.sqrMagnitude;

                // Move closer if not too close to the enemy.
                if (sqrDistanceToPlayer > SqrMinimumDistanceToPlayer)
                {
                    Owner.Mover.Move(Owner.transform.forward);
                }

                // If the Raycast hits the player, shoot the weapon.
                if (ShootingPointRaycaster(Owner.Weapon.ReturnShootingPoint))
                {
                    //Debug.Log("PlayerUnit in front of the shooting point!");
                    Owner.Weapon.Shoot();
                }
            }
        }

        public ShootState(EnemyUnit owner)
            : base(owner, AIStateType.Shoot)
        {
            State = AIStateType.Shoot;
            Owner = owner;
            
            AddTransition(AIStateType.Patrol);
            AddTransition(AIStateType.FollowTarget);
        }

        /// <summary>
        /// Check if the PlayerUnit is still within shooting range and if the PlayerUnit is still alive.
        /// </summary>
        /// <returns></returns>
        private bool ChangeState()
        {
            // Check to see if the target is enabled. If not, return to Patrol state.
            if (!Owner.Target.gameObject.activeInHierarchy)
            {
                return Owner.PerformTransition(AIStateType.Patrol);
            }

            Vector3 toPlayerVector =
                Owner.transform.position - Owner.Target.transform.position;
            float sqrDistanceToPlayer = toPlayerVector.sqrMagnitude;

            // If the PlayerUnit is greater than shooting distance, return to FollowTarget state.
            if (sqrDistanceToPlayer > SqrShootingDistance)
            {
                return Owner.PerformTransition(AIStateType.FollowTarget);
            }

            return false;
        }

        /// <summary>
        /// This method checks if the PlayerUnit is located in front of the EnemyUnit's shooting point using a Raycast.
        /// </summary>
        /// <param name="shootingPointTransform">The Transform of the EnemyUnit's shooting point.</param>
        /// <returns>True, if the PlayerUnit is hit by the Raycast. False, if not.</returns>
        private bool ShootingPointRaycaster(Transform shootingPointTransform)
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            int mask = Flags.CreateMask(playerLayer);

            Ray shootingPointRay = new Ray(shootingPointTransform.position, shootingPointTransform.transform.up);
            if (Physics.Raycast(shootingPointTransform.position,
                shootingPointTransform.transform.up,
                Owner.ShootingDistance,
                mask))
                return true;

            return false;
        }
    }
}
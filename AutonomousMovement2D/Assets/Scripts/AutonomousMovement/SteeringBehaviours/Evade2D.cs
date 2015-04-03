using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Evade2D : SteeringBehaviour2D, ITargettedSteeringBehaviour {
        public SteeringAgent2D Pursuer = null;
        public float panicDistance = -1;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.EvadeWeight;
                Probability = World2D.Instance.DefaultSettings.EvadeProb;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(this.agent, Pursuer, panicDistance);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, SteeringAgent2D pursuer, float panicDistance = -1) {
            if (pursuer == null) return Vector2.zero;
            var toPursuer = pursuer.GetComponent<Rigidbody2D>().position - agent.GetComponent<Rigidbody2D>().position;

            if (panicDistance != -1 && toPursuer.sqrMagnitude > panicDistance * panicDistance) {
                return Vector2.zero;
            }

            //Not considered ahead so we predict where the evader will be
            float lookAheadTime = toPursuer.magnitude / (agent.MaxSpeed + pursuer.GetComponent<Rigidbody2D>().velocity.magnitude);

            return Flee2D.GetVelocity(agent, pursuer.GetComponent<Rigidbody2D>().position + pursuer.GetComponent<Rigidbody2D>().velocity * lookAheadTime);
        }

        public override int CalculationOrder {
            get { return 3; }
        }

        public SteeringAgent2D TargetAgent1 {
            get {
                return Pursuer;
            }
            set {
                Pursuer = value;
            }
        }

        public SteeringAgent2D TargetAgent2 {
            get {
                return null;
            }
            set {
                //Do nothing
            }
        }
    }
}

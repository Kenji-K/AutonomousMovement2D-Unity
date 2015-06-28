using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kensai.AutonomousMovement {
    public class Interpose2D : SteeringBehaviour2D, ITargettedSteeringBehaviour {
        public SteeringAgent2D AgentA = null;
        public SteeringAgent2D AgentB = null;
        public float SlowingDistance = 1f;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.InterposeWeight;
                SlowingDistance = World2D.Instance.DefaultSettings.ArriveSlowingDistance;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, AgentA, AgentB, SlowingDistance);
        }

        public static Vector2 GetVelocity(SteeringAgent2D thisAgent, SteeringAgent2D agentA, SteeringAgent2D agentB, float slowingDistance = 1f) {
            if (agentA == null || agentB == null) return Vector2.zero;
            if (slowingDistance < 0) throw new InvalidOperationException("The slowing distance can't be negative.");

            Vector2 midPoint = (agentA.GetComponent<Rigidbody2D>().position + agentB.GetComponent<Rigidbody2D>().position) / 2;

            float timeToReachMidPoint = (thisAgent.Rigidbody2D.position - midPoint).magnitude / thisAgent.MaxSpeed;

            //Position in the future
            Vector2 AFuturePos = agentA.GetComponent<Rigidbody2D>().position + agentA.GetComponent<Rigidbody2D>().velocity * timeToReachMidPoint;
            Vector2 BFuturePos = agentB.GetComponent<Rigidbody2D>().position + agentB.GetComponent<Rigidbody2D>().velocity * timeToReachMidPoint;

            midPoint = (AFuturePos + BFuturePos) / 2;

            return Arrive2D.GetVelocity(thisAgent, midPoint, slowingDistance);
        }

        public override int CalculationOrder {
            get { return 13; }
        }

        public SteeringAgent2D TargetAgent1 {
            get {
                return AgentA;
            }
            set {
                AgentA = value;
            }
        }

        public SteeringAgent2D TargetAgent2 {
            get {
                return AgentB;
            }
            set {
                AgentB = value;
            }
        }
    }
}
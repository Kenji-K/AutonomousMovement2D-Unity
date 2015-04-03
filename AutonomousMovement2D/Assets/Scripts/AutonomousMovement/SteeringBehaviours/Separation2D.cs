using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kensai.AutonomousMovement {
    public class Separation2D : SteeringBehaviour2D {
        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.SeparationWeight;
                Probability = World2D.Instance.DefaultSettings.SeparationProb;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, agent.Neighbors);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, IEnumerable<SteeringAgent2D> neighbors) {
            Vector2 steeringForce = Vector2.zero;
            foreach (var neighbor in neighbors) {
                if (neighbor == agent) continue;
                if (agent.TargetAgents.Contains(neighbor)) continue; //Ignore the target of evasion or pursue

                var toAgent = agent.GetComponent<Rigidbody2D>().position - neighbor.GetComponent<Rigidbody2D>().position;
                steeringForce += toAgent.normalized / toAgent.magnitude;
            }

            return steeringForce;
        }

        public override bool RequiresNeighborList {
            get {
                return true;
            }
        }

        public override int CalculationOrder {
            get { return 4; }
        }
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kensai.AutonomousMovement {
    public class Separation2D : SteeringBehaviour2D {
        Vector2 steeringForce = Vector2.zero;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.SeparationWeight;
                Probability = World2D.Instance.DefaultSettings.SeparationProb;
            }
        }

        public override Vector2 GetVelocity() {
            //return GetVelocity(agent, agent.Neighbors);

            Profiler.BeginSample("Separation2D");
            steeringForce = Vector2.zero;
            if (agent.Neighbors.Count() != 0)
            foreach (var neighbor in agent.Neighbors) {
                if (neighbor == agent) continue;
                if (agent.TargetAgents.Contains(neighbor)) continue; //Ignore the target of evasion or pursue

                var toAgent = agent.Rigidbody2D.position - neighbor.Rigidbody2D.position;
                steeringForce += toAgent.normalized * (agent.NeighborRadius / 8) / toAgent.magnitude;
            }
            Profiler.EndSample();

            return steeringForce;
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, IEnumerable<SteeringAgent2D> neighbors) {
            Vector2 steeringForce = Vector2.zero;
            foreach (var neighbor in neighbors) {
                if (neighbor == agent) continue;
                if (agent.TargetAgents.Contains(neighbor)) continue; //Ignore the target of evasion or pursue

                var toAgent = agent.Rigidbody2D.position - neighbor.Rigidbody2D.position;
                steeringForce += toAgent.normalized * (agent.NeighborRadius / 8) / toAgent.magnitude;
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

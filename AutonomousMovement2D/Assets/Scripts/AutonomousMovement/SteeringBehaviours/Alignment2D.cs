using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kensai.AutonomousMovement {
    public class Alignment2D : SteeringBehaviour2D {
        Vector2 avgHeading = Vector2.zero;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.AlignmentWeight;
                Probability = World2D.Instance.DefaultSettings.AlignmentProb;
            }
        }
        
        public override Vector2 GetVelocity() {
            //return GetVelocity(agent, agent.Neighbors);

            Profiler.BeginSample("Alignment2D");
            avgHeading = Vector2.zero;
            foreach (var neighbor in agent.Neighbors) {
                if (neighbor == agent) continue; //Ignore the same agent
                if (agent.TargetAgents.Contains(neighbor)) continue; //Ignore the target of evasion or pursue

                avgHeading += neighbor.Heading;
            }

            if (agent.Neighbors.Count() > 0) {
                avgHeading = avgHeading / (float)agent.Neighbors.Count() - agent.Heading;
            }
            Profiler.EndSample();

            return avgHeading;
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, IEnumerable<SteeringAgent2D> neighbors) {
            Vector2 avgHeading = Vector2.zero;
            foreach (var neighbor in neighbors) {
                if (neighbor == agent) continue; //Ignore the same agent
                if (agent.TargetAgents.Contains(neighbor)) continue; //Ignore the target of evasion or pursue
                
                avgHeading += neighbor.Heading;
            }

            if (neighbors.Count() > 0) {
                avgHeading = avgHeading / (float)agent.Neighbors.Count() - agent.Heading;
            }

            return avgHeading;
        }

        public override bool RequiresNeighborList {
            get {
                return true;
            }
        }

        public override int CalculationOrder {
            get { return 5; }
        }
    }
}

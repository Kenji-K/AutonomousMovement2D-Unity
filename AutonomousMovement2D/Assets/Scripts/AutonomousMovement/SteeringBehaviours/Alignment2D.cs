using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kensai.AutonomousMovement {
    public class Alignment2D : SteeringBehaviour2D {
        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.AlignmentWeight;
                Probability = World2D.Instance.DefaultSettings.AlignmentProb;
            }
        }
        
        public override Vector2 GetVelocity() {
            return GetVelocity(agent, agent.Neighbors);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, IEnumerable<SteeringAgent2D> neighbors) {
            Vector2 avgHeading = Vector2.zero;
            foreach (var neighbor in neighbors) {
                if (neighbor == agent) continue;

                avgHeading += neighbor.Heading;
            }

            int neighborCount = neighbors.Count();
            if (neighborCount > 0) {
                avgHeading /= (float)neighborCount;
                avgHeading -= agent.Heading;
            }

            return avgHeading;
        }

        public override bool RequiresNeighborList {
            get {
                return true;
            }
        }
    }
}

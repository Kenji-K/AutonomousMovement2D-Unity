using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace Kensai.AutonomousMovement {
    /// <summary>
    /// Abstraction of a navigation path for 2D objects
    /// </summary>
    public class NavMesh2D {
        private LinkedListNode<Vector2> current;
        public bool IsClosedNavMesh { get; set; }

        private LinkedList<Vector2> waypointList;
        public LinkedList<Vector2> WaypointList {
            get {
                if (waypointList == null) waypointList = new LinkedList<Vector2>();
                return waypointList; 
            }
            set {
                if (value.Count < 2 && IsClosedNavMesh) 
                    throw new InvalidOperationException("Can't create a single waypoint loopable path.");

                //The Navmesh should point towards the first target immediately after initalization or assignment
                if (value.Count > 0) {
                    current = value.First;
                } else {
                    current = null;
                }

                waypointList = value; 
            }
        }

        public NavMesh2D(bool isClosedNavMesh = false) {
            IsClosedNavMesh = isClosedNavMesh;
            WaypointList = new LinkedList<Vector2>();
        }

        public NavMesh2D(IEnumerable<Vector2> initialWaypointList, bool isClosedNavMesh = false) {
            IsClosedNavMesh = isClosedNavMesh;
            WaypointList = new LinkedList<Vector2>(initialWaypointList);
        }

        public Vector2? CurrentWaypoint {
            get {
                if (current != null) {
                    return current.Value;
                } else { 
                    return null;
                }
            }
        }

        /// <summary>
        /// Sets the next waypoint as the current objective.
        /// </summary>
        /// <returns>True if the next waypoint was set to a valid target, false otherwise.</returns>
        public bool SetNextWaypoint() {
            var nextWaypoint = PeekNext();
            if (!IsEmpty()) { 
                if (nextWaypoint != null) {
                    current = nextWaypoint;
                    return true;
                } else {
                    return false;
                }
            } else {
                current = null;
                return false;
            }
        }

        /// <summary>
        /// Checks if the navmesh has been completely traversed.
        /// </summary>
        public bool IsFinished() {
            return (current == waypointList.Last && !IsClosedNavMesh) || current == null;
        }

        /// <summary>
        /// Checks if the waypoint list is empty or null.
        /// </summary>
        public bool IsEmpty() {
            return WaypointList == null || !WaypointList.Any();
        }

        /// <summary>
        /// Adds a waypoint at the end of the current path
        /// </summary>
        /// <param name="targetPosition">The target position in world coordinates</param>
        public void AddWaypointLast(Vector2 targetPosition) {
            WaypointList.AddLast(targetPosition);
        }

        /// <summary>
        /// Adds a waypoint before the current destination
        /// </summary>
        /// <param name="targetPosition">The target position in world coordinates</param>
        public void AddWaypointPrev(Vector2 targetPosition) {
            if (current == null) {
                WaypointList.AddFirst(targetPosition);
            } else {
                WaypointList.AddBefore(current, targetPosition);
            }
        }

        /// <summary>
        /// Adds a waypoint after the current destination
        /// </summary>
        /// <param name="targetPosition">The target position in world coordinates</param>
        public void AddWaypointNext(Vector2 targetPosition) {
            if (current == null) {
                WaypointList.AddLast(targetPosition);
            } else { 
                WaypointList.AddAfter(current, targetPosition);
            }
        }

        /// <summary>
        /// Adds a waypoint at the given index position in the path
        /// </summary>
        /// <param name="targetPosition">The target position in world coordinates</param>
        /// <param name="index">Position in the waypoint list where the target should be inserted at</param>
        public void AddWaypointAt(Vector2 targetPosition, int index) {
            if (index < 0) throw new IndexOutOfRangeException();
            
            if (!WaypointList.Any()) WaypointList.AddLast(targetPosition);

            var currentIndex = 0;
            var currentNode = WaypointList.First;
            while (currentIndex < index) {
                if (currentNode == WaypointList.Last) {
                    if (index > currentIndex + 1) throw new IndexOutOfRangeException();

                    WaypointList.AddLast(targetPosition);
                    return;
                } else {
                    currentNode = currentNode.Next;
                    currentIndex++;
                }
            }

            WaypointList.AddBefore(currentNode, targetPosition);
        }

        /// <summary>
        /// Removes the last waypoint in the list. Will loop if navmesh is closed.
        /// Throws an InvalidOperationException if empty.
        /// </summary>
        public void RemoveWaypointLast() {
            if (!IsEmpty()) {
                WaypointList.RemoveLast();
                SetNextWaypoint();
            } else {
                throw new InvalidOperationException("No waypoints in navmesh.");
            }
        }

        /// <summary>
        /// Removes the current waypoint. Will loop if navmesh is closed.
        /// Throws an InvalidOperationException if empty.
        /// </summary>
        public void RemoveWaypointCurrent() {
            if (!IsEmpty()) {
                RemoveWaypoint(current);
            } else {
                throw new InvalidOperationException("No waypoints in navmesh.");
            }
        }

        /// <summary>
        /// Removes the previous waypoint. Will loop if navmesh is closed.
        /// Throws an InvalidOperationException if empty.
        /// Throws an IndexOutOfRangeException if navmesh is not closed and there is no previous waypoint.
        /// </summary>
        public void RemoveWaypointPrev() {
            var prevNode = PeekPrev();
            if (!IsEmpty() && prevNode != null) {
                RemoveWaypoint(prevNode);
            } else if (IsEmpty()) {
                throw new InvalidOperationException("No waypoints in navmesh.");
            } else if (prevNode == null) {
                throw new IndexOutOfRangeException("There is no previous waypoint");
            }
        }

        /// <summary>
        /// Removes the next waypoint. Will loop if navmesh is closed.
        /// Throws an InvalidOperationException if empty.
        /// Throws an IndexOutOfRangeException if navmesh is not closed and there is no next waypoint.
        /// </summary>
        public void RemoveWaypointNext() {
            var nextNode = PeekNext();
            if (!IsEmpty() && nextNode != null) {
                RemoveWaypoint(nextNode);
            } else if (IsEmpty()) {
                throw new InvalidOperationException("No waypoints in navmesh.");
            } else if (nextNode == null) {
                throw new IndexOutOfRangeException("There is no next waypoint");
            }
        }

        /// <summary>
        /// Removes the waypoint at the specified position.
        /// Throws an IndexOutOfRangeException if there is no waypoint at specified position.
        /// </summary>
        public void RemoveWaypointAt(int index) {
            if (index < 0 || IsEmpty()) throw new IndexOutOfRangeException();

            var currentIndex = 0;
            var currentLoopingNode = WaypointList.First;
            while (currentIndex < index) {
                if (currentLoopingNode == WaypointList.Last) {
                    if (index > currentIndex) throw new IndexOutOfRangeException();
                } else {
                    currentLoopingNode = currentLoopingNode.Next;
                    currentIndex++;
                }
            }

            RemoveWaypoint(currentLoopingNode);
        }


        private LinkedListNode<Vector2> PeekNext() {
            if (!WaypointList.Any()) {
                return null;
            }

            if (current != WaypointList.Last) {
                return current.Next;
            } else if (current == WaypointList.Last && IsClosedNavMesh) {
                return waypointList.First;
            } else /*if (current == WaypointList.Last && !IsClosedNavMesh)*/ {
                return null;
            }
        }

        private LinkedListNode<Vector2> PeekPrev() {
            if (!WaypointList.Any()) {
                return null;
            }

            if (current != WaypointList.First) {
                return current.Next;
            } else if (current == WaypointList.First && IsClosedNavMesh) {
                return waypointList.Last;
            } else /*if (current == WaypointList.First && !IsClosedNavMesh)*/ {
                return null;
            }
        }

        private void RemoveWaypoint(LinkedListNode<Vector2> node) {
            if (!IsEmpty()) {
                if (node == current) {
                    SetNextWaypoint();
                    WaypointList.Remove(node);
                    if (IsEmpty()) current = null;
                } else {
                    WaypointList.Remove(node);
                }
            } else {
                throw new InvalidOperationException();
            }
        }
    }
}

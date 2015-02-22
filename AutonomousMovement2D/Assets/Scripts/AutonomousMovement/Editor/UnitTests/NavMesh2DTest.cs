using Kensai.AutonomousMovement;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.AutonomousMovement.UnitTests {
    [TestFixture]
    public class NavMesh2DTest {
        NavMesh2D navMesh;
        NavMesh2D emptyNavMesh;
        Vector2 waypointToAdd;

        [SetUp]
        public void Init() {
            navMesh = new NavMesh2D(new List<Vector2> { 
                new Vector2(0, 0), 
                new Vector2(1, 1), 
                new Vector2(2, 2), 
                new Vector2(3, 3) });

            emptyNavMesh = new NavMesh2D();

            waypointToAdd = new Vector2(500, 500);
        }

        /***************************** ADDITION OF WAYPOINTS ***************************/

        [Test]
        public void AddWaypointLast_Appends_A_Node() {
            navMesh.AddWaypointLast(waypointToAdd);

            Assert.AreEqual(waypointToAdd, navMesh.WaypointList.Last.Value);
        }

        [Test]
        public void AddWaypointLast_Appends_A_Node_in_empty_NavMesh2D() {
            emptyNavMesh.AddWaypointLast(waypointToAdd);

            Assert.AreEqual(waypointToAdd, emptyNavMesh.WaypointList.Last.Value);
        }

        [Test]
        public void AddWaypointNext_Inserts_a_node_at_correct_position() {
            navMesh.SetNextWaypoint();
            navMesh.AddWaypointNext(waypointToAdd);

            Assert.AreEqual(waypointToAdd, navMesh.WaypointList.ElementAt(2));
        }

        [Test]
        public void AddWaypointNext_Inserts_a_node_in_empty_NavMesh2D() {
            emptyNavMesh.AddWaypointNext(waypointToAdd);

            Assert.AreEqual(waypointToAdd, emptyNavMesh.WaypointList.Last.Value);
        }

        [Test]
        public void AddWaypointAt_Inserts_node_at_correct_position() {
            navMesh.AddWaypointAt(waypointToAdd, 3);
            Assert.AreEqual(waypointToAdd, navMesh.WaypointList.ElementAt(3));
        }

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void AddWaypointAt_Throws_exception_if_negative_index() {
            navMesh.AddWaypointAt(waypointToAdd, -1);
        }

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void AddWaypointAt_Throws_exception_if_index_greater_than_count() {
            navMesh.AddWaypointAt(waypointToAdd, navMesh.WaypointList.Count + 1);
        }

        /***************************** REMOVAL OF WAYPOINTS ******************************/

        [Test]
        public void RemoveWaypointCurrent_Sets_next_point_as_current() {
            var secondValue = navMesh.WaypointList.First.Next;
            navMesh.RemoveWaypointCurrent();
            Assert.AreEqual(secondValue.Value, navMesh.CurrentWaypoint.Value);
        }

        [Test]
        public void RemoveWaypointCurrent_Leaves_current_as_null_when_removing_last_remaining_element() {
            var navMesh = new NavMesh2D(new List<Vector2> { new Vector2(0, 0) });
            navMesh.RemoveWaypointCurrent();
            Assert.AreEqual(null, navMesh.CurrentWaypoint);
        }

        [Test]
        public void RemoveWaypointCurrent_Loops_over_when_removing_last_element() {
            navMesh.IsClosedNavMesh = true;
            for (int i = 0; i < 3; i++) {
                navMesh.SetNextWaypoint();
            }

            navMesh.RemoveWaypointCurrent();
            Assert.AreEqual(navMesh.WaypointList.First.Value, navMesh.CurrentWaypoint.Value);
        }

        [Test]
        public void RemoveWaypointNext_Removes_next_in_list() {
            var thirdValue = navMesh.WaypointList.First.Next.Next;
            navMesh.RemoveWaypointNext();
            navMesh.SetNextWaypoint();
            Assert.AreEqual(thirdValue.Value, navMesh.CurrentWaypoint.Value);
        }

        [Test]
        public void RemoveWaypointNext_Loops_over_when_removing_next_of_last_element() {
            var secondValue = navMesh.WaypointList.First.Next;
            navMesh.IsClosedNavMesh = true;
            for (int i = 0; i < 3; i++) {
                navMesh.SetNextWaypoint();
            }

            navMesh.RemoveWaypointNext();
            Assert.AreEqual(secondValue.Value, navMesh.WaypointList.First.Value);
        }

        [Test]
        public void RemoveWaypointNext_Leaves_current_as_null_when_removing_last_remaining_element() {
            var navMesh = new NavMesh2D(new List<Vector2> { new Vector2(0, 0) });
            navMesh.IsClosedNavMesh = true;
            navMesh.RemoveWaypointNext();
            Assert.AreEqual(null, navMesh.CurrentWaypoint);
        }

        [Test]
        public void RemoveWaypointPrev_Removes_next_in_list() {
            var secondValue = navMesh.WaypointList.First.Next;
            navMesh.SetNextWaypoint();
            navMesh.RemoveWaypointPrev();
            Assert.AreEqual(secondValue.Value, navMesh.CurrentWaypoint);
        }

        [Test]
        public void RemoveWaypointPrev_Loops_over_when_removing_prev_of_first_element() {
            var secondToLastValue = navMesh.WaypointList.Last.Previous;
            navMesh.IsClosedNavMesh = true;

            navMesh.RemoveWaypointPrev();
            Assert.AreEqual(secondToLastValue.Value, navMesh.WaypointList.Last.Value);
        }

        [Test]
        public void RemoveWaypointPrev_Leaves_current_as_null_when_removing_last_remaining_element() {
            var navMesh = new NavMesh2D(new List<Vector2> { new Vector2(0, 0) });
            navMesh.IsClosedNavMesh = true;
            navMesh.RemoveWaypointPrev();
            Assert.AreEqual(null, navMesh.CurrentWaypoint);
        }

        [Test]
        public void RemoveWaypointAt_Removes_node_at_correct_position() {
            int indexToRemove = 2;
            var nextInLine = navMesh.WaypointList.ElementAt(indexToRemove + 1);
            navMesh.RemoveWaypointAt(indexToRemove);
            Assert.AreEqual(navMesh.WaypointList.ElementAt(indexToRemove), nextInLine);
        }

        [Test]
        public void RemoveWaypointAt_Maintains_current_when_removed_is_not_current() {
            var previousCurrent = navMesh.WaypointList.First.Value;
            navMesh.RemoveWaypointAt(2);
            Assert.AreEqual(previousCurrent, navMesh.CurrentWaypoint.Value);
        }

        [Test]
        public void RemoveWaypointAt_Sets_next_waypoint_if_current_is_deleted() {
            var secondValue = navMesh.WaypointList.First.Next;
            navMesh.RemoveWaypointAt(0);
            Assert.AreEqual(secondValue.Value, navMesh.CurrentWaypoint.Value);
        }

        [Test]
        public void RemoveWaypointAt_Loops_over_when_removing_last_element() {
            navMesh.IsClosedNavMesh = true;
            for (int i = 0; i < 3; i++) {
                navMesh.SetNextWaypoint();
            }

            navMesh.RemoveWaypointAt(3);
            Assert.AreEqual(navMesh.WaypointList.First.Value, navMesh.CurrentWaypoint.Value);
        }

        [Test]
        public void RemoveWaypointAt_Leaves_current_as_null_when_removing_last_remaining_element() {
            var navMesh = new NavMesh2D(new List<Vector2> { new Vector2(0, 0) });
            navMesh.RemoveWaypointAt(0);
            Assert.IsFalse(navMesh.CurrentWaypoint.HasValue);
        }
        
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void RemoveWaypointCurrent_On_empty_navmesh_throws_exception() {
            emptyNavMesh.RemoveWaypointCurrent();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void RemoveWaypointNext_On_empty_navmesh_throws_exception() {
            emptyNavMesh.RemoveWaypointNext();
        }

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void RemoveWaypointNext_When_last_waypoint_and_not_closed_throws_exception() {
            for (int i = 0; i < 3; i++) {
                navMesh.SetNextWaypoint();
            }
            navMesh.RemoveWaypointNext();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void RemoveWaypointLast_On_empty_navmesh_throws_exception() {
            emptyNavMesh.RemoveWaypointLast();
        }

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void RemoveWaypointAt_throws_exception_if_negative_index() {
            navMesh.RemoveWaypointAt(-1);
        }

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void RemoveWaypointAt_throws_exception_if_index_greater_than_count() {
            navMesh.RemoveWaypointAt(-1);
        }
    }
}

using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class StareNode : Sequence
    {
        private const float TIMER_SECONDS = 5;
        private readonly Fiddlesticks fiddlesticks;

        // Wait 5 seconds
        // Kill player

        private float timer;
        private bool hasStartedStaring;

        public StareNode(Fiddlesticks fiddlesticks)
        {
            this.fiddlesticks = fiddlesticks;
            timer = TIMER_SECONDS;

            Attach(new CallbackNode(IsStaring));

            // Wait X seconds
            Attach(new CallbackNode(StareTimer));

            // Kill player
            Attach(new CallbackNode(KillTarget));
        }

        private NodeState IsStaring()
        {
            // If already staring, skip
            if (hasStartedStaring)
                return NodeState.SUCCESS;

            // If not staring, skip
            if (!IsCurrentlyStaring())
                return NodeState.FAILURE;

            StartStaring();
            return NodeState.SUCCESS;
        }
        private NodeState StareTimer()
        {
            if (IsCurrentlyStaring())
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer += Time.deltaTime * 0.5f;
            }

            timer = Mathf.Clamp(timer, 0, TIMER_SECONDS);
            StareUpdate(1 - (timer / TIMER_SECONDS));

            if (timer >= TIMER_SECONDS)
            {
                EndStaring();
                return NodeState.FAILURE;
            }

            if (timer <= 0)
                return NodeState.SUCCESS;
            return NodeState.RUNNING;
        }
        private NodeState KillTarget()
        {
            fiddlesticks.Kill();
            return NodeState.SUCCESS;
        }

        private bool IsCurrentlyStaring() => fiddlesticks.IsVisible;

        private void StartStaring()
        {
            hasStartedStaring = true;
            timer = TIMER_SECONDS;

            fiddlesticks.StartStare();
        }

        private void StareUpdate(float percent)
        {
            fiddlesticks.StareUpdate(percent);
        }

        private void EndStaring()
        {
            hasStartedStaring = false;

            fiddlesticks.EndStare();
        }

        #region Node

        public override string GetText() => "Stare";

        #endregion
    }
}
namespace BehaviourTree.Nodes.Generic
{
    /// <summary>
    /// Node that helps to wait for animations
    /// </summary>
    public class AnimationNode : Node
    {
        /// <summary>
        /// The animation is currently playing
        /// </summary>
        public bool isActing { get; private set; }
        
        /// <summary>
        /// The animation has played and ended
        /// </summary>
        public bool hasActed { get; private set; }
        
        private readonly System.Action callback;
        
        public AnimationNode(System.Action callback)
        {
            this.callback = callback;
        }
        
        public void OnEnded()
        {
            this.isActing = false;
            this.hasActed = true;
        }

        public void ResetAnim()
        {
            this.isActing = false;
            this.hasActed = false;
        }

        #region Node

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            // Completed the animation
            if (this.hasActed)
                return NodeState.SUCCESS;
            
            // ReSharper disable once InvertIf
            // If the animation is not playing
            if (!this.isActing)
            {
                this.callback?.Invoke();
                this.isActing = true;
            }

            // Running the animation
            return NodeState.RUNNING;
        }

        /// <inheritdoc/>
        public override string GetText() => "Animation";

        #endregion
    }
}
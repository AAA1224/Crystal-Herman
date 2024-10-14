namespace KoboldTools
{
    /// <summary>
    /// Characterises the flow (e.g. sequence of game states) of a game.
    /// The class implements the singleton pattern and needs to be 
    /// inherited by a specific gameflow class for each game.
    /// </summary>
    public abstract class GameFlow : FlowBehaviour
    {
        /// <summary>
        /// Stores a reference to the singleton instance.
        /// </summary>
        private static GameFlow _instance = null;

        /// <summary>
        /// Provides access to the singleton instance.
        /// </summary>
        /// <value>The singleton instance of <see cref="GameFlow"/>.</value>
        public static GameFlow instance
        {
            get
            {
                // Retrieve a reference to the singleton instance.
                if (GameFlow._instance == null)
                {
                    GameFlow._instance = FindObjectOfType<GameFlow>();
                }

                return GameFlow._instance;
            }
        }
        /// <summary>
        /// Factory method that creates a game flow phase (<see cref="IFlowPhase"/>).
        /// </summary>
        /// <returns>The new game flow phase.</returns>
        protected override abstract IFlowPhase createFlow();

    }
}

namespace Assets.Scripts
{
    using ModApi.Mods;
    using UnityEngine;

    /// <summary>
    /// A singleton object representing this mod that is instantiated and initialize when the mod is loaded.
    /// </summary>
    public class Mod : ModApi.Mods.GameMod
    {
        private GameObject _senderObject;

        /// <summary>
        /// Prevents a default instance of the <see cref="Mod"/> class from being created.
        /// </summary>
        private Mod() : base()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the mod object.
        /// </summary>
        /// <value>The singleton instance of the mod object.</value>
        public static Mod Instance { get; } = GetModInstance<Mod>();

        /// <summary>
        /// Creates persistent runtime objects after the mod has been initialized by Juno.
        /// </summary>
        protected override void OnModInitialized()
        {
            base.OnModInitialized();

            if (_senderObject != null)
            {
                return;
            }

            _senderObject = new GameObject("JunoIO Bridge");
            Object.DontDestroyOnLoad(_senderObject);
            _senderObject.AddComponent<JunoIoBridge>();

            Debug.Log("JunoIO mod initialized");
        }
    }
}

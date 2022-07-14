namespace TsMap.Jagfx.Shared {
    public sealed class Store {
        public TsGame Game;

        static Store() { }
        private Store() { }

        public static Store Instance { get; } = new Store();
    }
}
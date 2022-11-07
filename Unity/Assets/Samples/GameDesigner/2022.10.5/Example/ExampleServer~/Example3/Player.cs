using Net.Server;

namespace LockStep.Server
{
    public class Player : UdxPlayer
    {
        internal bool readyBattle;
        private Scene scene;
        public Scene Scene { get { return scene; } set { scene = value; } }

        public override void OnExit()
        {
            scene = null;
        }
    }
}

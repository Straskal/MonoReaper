namespace Adventure.Core.States
{
    public class IntroScene : Scene
    {
        public IntroScene(Session session, Campaign campaign) 
        {
            Session = session;
            Campaign = campaign;
        }

        public Session Session { get; }
        public Campaign Campaign { get; }
    }
}

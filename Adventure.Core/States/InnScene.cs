using System;

namespace Adventure.Core.States;

public class InnScene : Scene
{
    public InnScene(Session session) 
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public Session Session { get; }
    public Campaign SelectedCampaign { get; private set; }

    public override void Start()
    {
    }

    public void OnCampaignSelected(Campaign campaign) 
    {
        SelectedCampaign = campaign;
    }

    public void OnCampaignStarted() 
    {
        Session.ChangeState(new IntroScene(Session, SelectedCampaign));
    }
}

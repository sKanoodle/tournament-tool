using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentTool.Tournaments;

namespace TournamentTool
{
    class WebServer : WebServerBase
    {
        // TODO: possible to update form on change of inputs, onchange="this.form.submit()" for inputs and action="/endpoint/" for form, but lose focus without active element handling client side
        protected override string BaseUrl => "http://127.0.0.1:22542/";

        private List<SignupPerson> Signups = new List<SignupPerson>();
        private List<List<MatchPairing>> Pairings = new List<List<MatchPairing>>();
        private ITournament Tournament;

        public WebServer()
        {
            //Signups.Add(new SignupPerson());


            //TESTING ONLY
            Signups.Add(new SignupPerson { RawValue = "Max" });
            Signups.Add(new SignupPerson { RawValue = "Klaus" });
            Signups.Add(new SignupPerson { RawValue = "Detlef" });
            Signups.Add(new SignupPerson { RawValue = "Heinz" });
            Signups.Add(new SignupPerson { RawValue = "Karl" });
            Signups.Add(new SignupPerson { RawValue = "Guenther" });
            Signups.Add(new SignupPerson { RawValue = "Lars" });
        }

        [RouteHandler(Routes.START)]
        public async Task<string> GetLandingPage()
        {
            return $@"{String.Join('\n', Signups.Select(s => s.RenderInput + "<br />"))}
<button form=""{FORM_ID}"" type=""submit"" formaction=""{Routes.START_ROUND_ROBIN}"">Round-Robin</button>
<button form=""{FORM_ID}"" type=""submit"" formaction=""{Routes.SIGNUP_ADD}"">More people!</button>";
        }

        [RouteHandler(Routes.AUTO_REFRESH, RouteHandlerAttribute.HttpMethodType.POST)]
        public async Task<string> AutoRefresh()
        {
            // if other sites than tournament use auto-refresh, there needs to be an active-site mechanic to determine the redirect target
            return Routes.TOURNAMENT;
        }

        [RouteHandler(Routes.SIGNUP_ADD, RouteHandlerAttribute.HttpMethodType.POST)]
        public async Task<string> AddMoreSignups()
        {
            Signups.Add(new SignupPerson());
            return Routes.START;
        }

        [RouteHandler(Routes.START_ROUND_ROBIN, RouteHandlerAttribute.HttpMethodType.POST)]
        public async Task<string> StartRoundRobin()
        {
            Tournament = new RoundRobinTournament(Signups.Where(s => !String.IsNullOrWhiteSpace(s.Name)).Select(s => s.Name).ToArray(), 3);
            return Routes.TOURNAMENT;
        }

        [RouteHandler(Routes.TOURNAMENT)]
        public async Task<string> RenderTournament()
        {
            return Tournament.Render();
        }

        private static class Routes
        {
            public const string START = "/";
            public const string AUTO_REFRESH = "/auto-refresh";
            public const string SIGNUP_ADD = "/signup/add";
            public const string START_ROUND_ROBIN = "/start/round-robin";
            public const string TOURNAMENT = "/tournament";
        }
    }

    class SignupPerson : InputObject
    {
        public string Name => RawValue;

        public string RenderInput => $"<input form=\"{WebServerBase.FORM_ID}\" type=\"text\" name=\"{ID}\" value=\"{RawValue}\" placeholder=\"Name ...\" />";
    }

    class MatchPairing
    {
        private readonly PairingPart Name1;
        private readonly PairingPart Name2;

        public string Winner => Name1.Score == Name2.Score ? String.Empty : Name1.Score > Name2.Score ? Name1.Name : Name2.Name;

        public MatchPairing(PairingPart part1, PairingPart part2)
        {
            Name1 = part1;
            Name2 = part2;
        }

        public string RenderPairing => 
$@"<div class=""pairing"">
    <div>
        <div>{Name1.Name}</div>
        <input form=""{WebServerBase.FORM_ID}"" type=""number"" step=""1"" min=""0"" value=""{Name1.RawValue}"" name=""{Name1.ID}"" />
    </div>
    <div>
        <div>{Name2.Name}</div>
        <input form=""{WebServerBase.FORM_ID}"" type=""number"" step=""1"" min=""0"" value=""{Name2.RawValue}"" name=""{Name2.ID}"" />
    </div>
</div>";
    }

    class EmptyPairingPart : PairingPart
    {
        public EmptyPairingPart()
        {
            Score = -1;
        }

        public override string Name => String.Empty;
        public override string RawValue
        {
            get => String.Empty;
            set => Score = -1;
        }
    }

    class InitialPairingPart : PairingPart
    {
        private readonly string InitialName;
        public override string Name => InitialName;

        public InitialPairingPart(string name)
        {
            InitialName = name;
        }
    }

    class PairingPart : InputObject
    {
        private int _Score;
        public int Score
        {
            get => _Score;
            set => _Score = value;
        }

        public override string RawValue
        {
            get => Score.ToString();
            set => int.TryParse(value, out _Score);
        }

        private readonly MatchPairing PreviousPairing;
        public virtual string Name => PreviousPairing.Winner;

        protected PairingPart() { }

        public PairingPart(MatchPairing previousPairing)
        {
            PreviousPairing = previousPairing;
        }
    }

    //class IntInput : BaseInput
    //{

    //}

    //class StringInput : BaseInput
    //{
    //    protected override string Type { get; } = "text";
    //    public string Value => RawValue;
    //    public override string RawValue { get; set; }
    //}

    //abstract class BaseInput
    //{
    //    protected virtual string AdditionalAttributes
    //    protected abstract string Type { get; }
    //    protected virtual string PlaceHolder { get; } = String.Empty;
    //    public string ID { get; } = Guid.NewGuid().ToString();
    //    public abstract string RawValue { get; set; }
    //    public string RenderInput => $"<input form=\"{WebServerBase.FORM_ID}\" type=\"{Type}\" name=\"{ID}\" value=\"{RawValue}\" placeholder=\"{PlaceHolder}\" /><br />";
    //}
}

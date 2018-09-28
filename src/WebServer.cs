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
<button form=""{FORM_ID}"" type=""submit"" formaction=""{Routes.START_SINGLE_ELIMINATION}"">Single-Elimination</button>
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

        [RouteHandler(Routes.START_SINGLE_ELIMINATION, RouteHandlerAttribute.HttpMethodType.POST)]
        public async Task<string> StartSingleElimination()
        {
            Tournament = new SingleEliminationTournament(Signups.Where(s => !String.IsNullOrWhiteSpace(s.Name)).Select(s => s.Name).ToArray(), 3);
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
            public const string START_SINGLE_ELIMINATION = "/start/single-elimination";
            public const string TOURNAMENT = "/tournament";
        }
    }

    class SignupPerson : InputObject
    {
        public string Name => RawValue;

        public string RenderInput => $"<input form=\"{WebServerBase.FORM_ID}\" type=\"text\" name=\"{ID}\" value=\"{RawValue}\" placeholder=\"Name ...\" />";
    }
}

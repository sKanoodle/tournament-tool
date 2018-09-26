using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class InputObject
    {
        private static Dictionary<string, InputObject> Objects = new Dictionary<string, InputObject>();

        public InputObject()
        {
            Objects.Add(ID, this);
        }

        public static void Update(Dictionary<string, string> values)
        {
            foreach (var kvp in values)
            {
                Objects[kvp.Key].RawValue = kvp.Value;
            }
        }

        public string ID { get; } = Guid.NewGuid().ToString();
        public virtual string RawValue { get; set; }

        public string DefaultAttributes => $@"form=""{WebServerBase.FORM_ID}"" value=""{RawValue}"" name=""{ID}""";
        public string DefaultAttributesAutoSubmit => $@"{DefaultAttributes}  onfocus=""gotFocus(this);"" onblur=""lostFocus(this);"" onchange=""changed(this);""";
    }
}

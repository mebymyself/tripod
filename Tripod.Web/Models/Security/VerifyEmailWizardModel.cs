﻿using System.Collections.Generic;

namespace Tripod.Web.Models
{
    public class VerifyEmailWizardModel
    {
        public VerifyEmailWizardModel(int stepNumber, string stepThree = "Register")
        {
            StepNumber = stepNumber;
            StepContents = new Dictionary<string, string>
            {
                { "Submit email", null },
                { "Verify email", null },
                { stepThree, null },
                { "Collect underpants", "http://www.youtube.com/watch?v=tO5sxLapAts" },
                { "???", "http://knowyourmeme.com/memes/profit" },
                { "Profit", "https://www.google.com/search?q=collect+underpants+profit" },
            };
        }

        public int StepNumber { get; private set; }
        public IDictionary<string, string> StepContents { get; private set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PolicyRequestStatus
    {
        PendingAdmin = 0,
        AgentAssigned = 1,
        FormSent = 2,
        FormSubmitted = 3,
        RiskCalculated = 4,
        CustomerConfirmed = 5,
        PolicyApproved = 6
    }
}

namespace EventSourcingPoc.API.Events.Endorsements
{
    public record EndorsementRiskEvalutionRequested(
        DateTime RequestedAt,
        string Reason,
        string? Observations
    );
    
}

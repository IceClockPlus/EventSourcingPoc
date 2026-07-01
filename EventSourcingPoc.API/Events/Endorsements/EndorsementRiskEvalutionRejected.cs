namespace EventSourcingPoc.API.Events.Endorsements
{
    public record EndorsementRiskEvalutionRejected(
        DateTime RejectedAt,
        string Reason,
        string? Observations,
        string EvaluatorId,
        string EvaluatorName
    );
}

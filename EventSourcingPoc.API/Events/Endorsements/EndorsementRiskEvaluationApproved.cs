namespace EventSourcingPoc.API.Events.Endorsements
{
    public record EndorsementRiskEvaluationApproved(
        DateTime ApprovedAt,
        string Reason,
        string? Observations,
        string EvaluatorId,
        string EvaluatorName
    );
}

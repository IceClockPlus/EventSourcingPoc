namespace EventSourcingPoc.API.Events.Endorsements
{
    /// <summary>
    /// Event triggered when an endorsement is paid, indicating that the payment for the endorsement has been completed.
    /// </summary>
    public record EndorsementPaid(
        decimal PaidPrice,
        string PaymentCode,
        DateTime PaidDate
    );
    
}

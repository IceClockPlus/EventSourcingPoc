using EventSourcingPoc.API.Domain;

namespace EventSourcingPoc.API.Events
{
    /// <summary>
    /// Event triggered when a guarantee is requested, containing all the necessary details for the guarantee request.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="TenderId"></param>
    /// <param name="Gloss"></param>
    /// <param name="Bond"></param>
    /// <param name="Supplier"></param>
    /// <param name="Beneficiary"></param>
    /// <param name="InitialDateCoverage"></param>
    /// <param name="InitialAmountCoverage"></param>
    /// <param name="Price"></param>
    public record GuaranteeRequested(
        Guid Id,
        string TenderId,
        string Gloss,
        GuaranteeBond Bond,
        LegalPartyInfo Supplier,
        LegalPartyInfo Beneficiary,
        DateTime Start,
        DateTime End,
        Money InitialAmountCoverage,
        Money Price
    );

    public record LegalPartyInfo(string TaxId, 
        string Name,
        string AddressStreet,
        string AddressLocation,
        string AddressRegion
    );

    /// <summary>
    /// Event triggered when a guarantee is abandoned, containing the reason for abandonment and the date of abandonment.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="AbandonmentDate"></param>
    public record GuaranteeAbandoned(string Reason, DateTime AbandonmentDate);

    /// <summary>
    /// Event triggered when a guarantee risk evaluation is requested, containing the reason for the evaluation request.
    /// </summary>
    /// <param name="Reason"></param>
    public record GuaranteeRiskEvaluationRequested(string Reason);

    /// <summary>
    /// Event triggered when a guarantee risk evaluation is approved, containing the reason for approval.
    /// </summary>
    /// <param name="Reason"></param>
    public record GuaranteeRiskEvaluationApproved(string Reason);
    /// <summary>
    /// Event triggered when a guarantee risk evaluation is rejected, containing the reason for rejection.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="RejectionDate"></param>
    public record GuaranteeRiskEvaluationRejected(string Reason, DateTime RejectionDate);
    /// <summary>
    /// Event triggered when a guarantee payment is confirmed, containing the amount paid and the date of payment.
    /// </summary>
    /// <param name="PaidAmount"></param>
    /// <param name="PaidDate"></param>
    public record GuaranteePaymentConfirmed(decimal PaidAmount, DateTime PaidDate);

    /// <summary>
    /// Event triggered when a guarantee is issued, containing the issue date, certificate number, and code for the issued guarantee.
    /// </summary>
    /// <param name="IssueDate"></param>
    /// <param name="CertificateNumber"></param>
    /// <param name="Code"></param>
    public record GuaranteeIssued(DateTime IssueDate, long CertificateNumber, string Code);

    /// <summary>
    /// Event triggered when a guarantee expires, containing the expiration date of the guarantee.
    /// </summary>
    /// <param name="ExpirationDate"></param>
    public record GuaranteeExpired(DateTime ExpirationDate);

    /// <summary>
    /// Event triggered when a guarantee claim is requested, containing the reason for the claim and the date of the claim request.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="ClaimDate"></param>
    public record GuaranteeClaimRequested(string Reason, DateTime ClaimDate);

    /// <summary>
    /// Event triggered when a guarantee claim is approved, containing the reason for approval and the date of approval.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="ApprovalDate"></param>
    public record GuaranteeClaimApproved(string Reason, DateTime ApprovalDate);

    /// <summary>
    /// Event triggered when a guarantee claim is rejected, containing the reason for rejection and the date of rejection.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="RejectionDate"></param>
    public record GuaranteeClaimRejected(string Reason, DateTime RejectionDate);

    /// <summary>
    /// Event triggered when a guarantee claim is paid, containing the amount paid and the date of payment.
    /// </summary>
    /// <param name="PaidAmount"></param>
    /// <param name="PaidDate"></param>
    public record GuaranteeClaimPaid(decimal PaidAmount, DateTime PaidDate);

    /// <summary>
    /// Event triggered when a guarantee is cancelled, containing the reason for cancellation and the date of cancellation.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="CancellationDate"></param>
    public record GuaranteeCancelled(string Reason, DateTime CancellationDate);

    /// <summary>
    /// Event triggered when a guarantee endorsement is requested, containing the new coverage details and the price for the endorsement.
    /// </summary>
    /// <param name="EndDateCoverage"></param>
    /// <param name="AmountCoverage"></param>
    /// <param name="Price"></param>
    public record GuaranteeEndorsementRequested(
        DateTime? EndDateCoverage,
        decimal? AmountCoverage,
        decimal Price
    );

    public record Evaluator(string Id, string Name);

    /// <summary>
    /// Event triggered when a guarantee endorsement evaluation is requested, containing the reason for the evaluation request.
    /// </summary>
    /// <param name="Reason"></param>
    public record GuaranteeEndorsementEvaluationRequested(string Reason);

    /// <summary>
    /// Event triggered when a guarantee endorsement evaluation is approved, containing the reason for approval and the date of approval.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="ApprovalDate"></param>
    public record GuaranteeEndorsementEvaluationApproved(string Reason, DateTime ApprovalDate, Evaluator Evaluator);

    /// <summary>
    /// Event triggered when a guarantee endorsement evaluation is rejected, containing the reason for rejection and the date of rejection.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="RejectionDate"></param>
    public record GuaranteeEndorsementEvaluationRejected(string Reason, DateTime RejectionDate, Evaluator Evaluator);

    /// <summary>
    /// Event triggered when a guarantee endorsement is abandoned, containing the reason for abandonment and the date of abandonment.
    /// </summary>
    /// <param name="Reason"></param>
    /// <param name="AbandonmentDate"></param>
    public record GuaranteeEndorsementAbandoned(string Reason, DateTime AbandonmentDate);

    /// <summary>
    /// Event triggered when a guarantee endorsement payment is confirmed, containing the amount paid and the date of payment.
    /// </summary>
    /// <param name="PaidAmount"></param>
    /// <param name="PaidDate"></param>
    public record GuaranteeEndorsementPaymentConfirmed(decimal PaidAmount, DateTime PaidDate);

    /// <summary>
    /// Event triggered when a guarantee endorsement is issued, containing the issue date and the endorsement number for the issued endorsement.
    /// </summary>
    /// <param name="IssueDate"></param>
    /// <param name="EndorsementNumber"></param>
    public record GuaranteeEndorsementIssued(DateTime IssueDate, string EndorsementNumber);

    /// <summary>
    /// Event triggered when a guarantee information is updated, containing the updated tender ID and gloss for the guarantee.
    /// </summary>
    /// <param name="TenderId"></param>
    /// <param name="Gloss"></param>
    public record GuaranteeInformationUpdated(
        string? TenderId,
        string? Gloss
    );

    /// <summary>
    /// Event triggered when a guarantee beneficiary information is updated, containing the updated beneficiary details for the guarantee.
    /// </summary>
    /// <remarks>The beneficiary update does not allow update its TaxId, as it is a unique identifier for the beneficiary and should not be changed once set.</remarks>
    /// <param name="BeneficiaryName"></param>
    /// <param name="BeneficiaryStreet"></param>
    /// <param name="BeneficiaryLocation"></param>
    /// <param name="BeneficiaryArea"></param>
    public record GuaranteeBeneficiaryInformationUpdated(
        string? BeneficiaryName,
        string? BeneficiaryStreet,
        string? BeneficiaryLocation,
        string? BeneficiaryArea
    );

    /// <summary>
    /// Event triggered when a guarantee client information is updated, containing the updated client details for the guarantee.
    /// </summary>
    /// <remarks>The client update does not allow update its TaxId, as it is a unique identifier for the client and should not be changed once set.</remarks>
    /// <param name="ClientName"></param>
    /// <param name="ClientStreet"></param>
    /// <param name="ClientLocation"></param>
    /// <param name="ClientArea"></param>
    public record GuaranteeClientInformationUpdated(
        string? ClientName,
        string? ClientStreet,
        string? ClientLocation,
        string? ClientArea
    );
}

namespace LoanManagementApi.Model
{
    //public enum LoanStatus
    //{
    //    Pending = 0,
    //    Approved = 1,
    //    Rejected = 2
    //}
    public enum LoanStatus
    {
        ApplicationSubmitted = 1,
        DocumentsUploaded = 2,
        DocumentsVerified = 3,
        CreditCheckCompleted = 4,
        UnderManagerApproval = 5,
        Approved = 6,
        Rejected = 7,
        DisbursementPending = 8,
    }
}

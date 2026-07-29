namespace LoanManagementApi.Model
{
    public class Response
    {
        public T GetResponse<T>(T data)
        {
            return data;
        }

    }
    public class ErrorResponse
    {
        int Id {  get; set; }
        string Message { get; set; }
    }
}

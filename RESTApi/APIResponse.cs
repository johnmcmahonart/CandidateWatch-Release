
namespace RESTApi

{
    public class APIResponse<T> : IAPIResponse<T>

    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public void NextPage()
        {
            throw new NotImplementedException();
        }

        public void PreviousPage()
        {
            throw new NotImplementedException();
        }
    }
}

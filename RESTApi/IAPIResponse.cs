namespace RESTApi
{
    public interface IAPIResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
        public void NextPage();
        public void PreviousPage();
        public CancellationToken CancellationToken { get; set; }    
    }
}

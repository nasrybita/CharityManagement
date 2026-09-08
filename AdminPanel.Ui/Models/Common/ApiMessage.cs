namespace AdminPanel.Ui.Models.Common
{
    public class ApiMessage<T>
    {
       
            public bool HasError { get; set; }
            public string? ErrorMessage { get; set; }
            public T? Value { get; set; }

       
    }
}

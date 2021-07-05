namespace Alura.CoisasAFazer.Services.Handlers
{
    public class CommandResult
    {
        public bool IsSuccess { get; set; }

        public CommandResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}

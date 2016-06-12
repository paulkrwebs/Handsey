namespace Handsey
{
    using System;
    using System.Threading.Tasks;

    public interface IApplicaton
    {
        bool Invoke<THandler>(Action<THandler> trigger);

        Task<bool> InvokeAsync<THandler>(Func<THandler, Task> trigger);

        void RegisterAll<THandler>();
    }
}
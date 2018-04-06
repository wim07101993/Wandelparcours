using DatabaseImporter.Services;

namespace DatabaseImporter.Helpers.Extensions
{
    public static class StateManagerExtensions
    {
        public static void SetState<T>(this IStateManager This, EState state, T value) 
            => This.SetState(state.ToString(), value);

        public static T GetState<T>(this IStateManager This, EState state)
            => This.GetState<T>(state.ToString());
    }
}
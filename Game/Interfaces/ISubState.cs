public interface ISubState
{
    // Делегат, который переключает состояния внутри текущей иерархии
    System.Action<IState> Switch { get; set; }
}
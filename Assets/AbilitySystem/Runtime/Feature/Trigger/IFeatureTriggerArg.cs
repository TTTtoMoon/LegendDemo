namespace Abilities
{
    /// <summary>
    /// 功能触发器参数
    /// </summary>
    public interface IFeatureTriggerArg
    {
    }

    /// <summary>
    /// 空参数，用来处理无参FeatureTrigger
    /// </summary>
    internal readonly struct EmptyArg : IFeatureTriggerArg
    {
        public static readonly EmptyArg Empty = new EmptyArg();
    }
}
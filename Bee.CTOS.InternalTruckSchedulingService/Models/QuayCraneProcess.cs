namespace Bee.CTOS.InternalTruckSchedulingService.Models;

/// <summary>
/// 岸桥工艺
/// </summary>
public enum QuayCraneProcess
{
    /// <summary>
    /// 双箱吊（单箱或一对小箱OneTime装卸）
    /// </summary>
    TwinLift,

    /// <summary>
    /// 单箱吊（单箱OneTime装卸或一对小箱OneByOne装卸）
    /// </summary>
    SingleLift,

    /// <summary>
    /// 双吊具(两大箱并排或两对小箱并排OneTimer装卸)（本版本暂不支持）
    /// </summary>
    DualSpreader,
}

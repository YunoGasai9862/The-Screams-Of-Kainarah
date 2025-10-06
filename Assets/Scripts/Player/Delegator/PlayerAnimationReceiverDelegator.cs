using System.Collections.Generic;

public class PlayerAnimationReceiverDelegator : BaseDelegator<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<IReceiverEnhancedAsync<PlayerAnimationController, ControllerPackage<PlayerAnimationExecutionState, bool>>>>>>();
    }
}
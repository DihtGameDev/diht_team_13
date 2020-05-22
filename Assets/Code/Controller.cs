using UnityEngine;

public class Controller : MonoBehaviour
{
    protected GameMode WorkingGameMode = GameMode.Default;

    void Update()
    {
        if (Game.CurrentGameMode == WorkingGameMode)
            Tick();
    }

    private void FixedUpdate()
    {
        if (Game.CurrentGameMode == WorkingGameMode)
            PhysicsTick();
    }
    
    void LateUpdate()
    {
        if (Game.CurrentGameMode == WorkingGameMode)
            LateTick();
    }

    public virtual void Tick()
    {
        
    }
    
    public virtual void LateTick()
    {
        
    }
    
    public virtual void PhysicsTick()
    {
        
    }
}

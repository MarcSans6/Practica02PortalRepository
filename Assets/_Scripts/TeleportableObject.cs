public class TeleportableObject : Teleportable
{
    public override bool CanTeleport(Portal _Portal)
    {
        m_Forward = transform.forward;
        return base.CanTeleport(_Portal);
    }

    public override void Teleport(Portal _Portal)
    {
        m_Forward = transform.forward;
        base.Teleport(_Portal);
    }
}

